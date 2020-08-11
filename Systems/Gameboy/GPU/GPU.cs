using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Systems.Gameboy.GPU
{
    public class GPU : Core.PPU
    {
        private readonly State state;
        private readonly MMU mmu;
        private int spriteSize;
        private bool lcdEnabled;
        private bool bgDisplay;
        private bool windowDisplay;
        private bool spriteDisplay;
        private int tileStartAddress;
        private int windowMapSelect;
        private int bgMapSelect;
        private IEnumerable<Sprite> spritesCurrentLine;
        private bool SkipFrame => (Frameskip >= 1 && Frame % (Frameskip + 1) != 0);
        private bool GBCMode => (System as System).GBCMode;
        private BackgroundMap backgroundMap;
        private uint[] currentLine;

        public GPU(System system) : base(system, 160, 144) 
        {
            Cycles = 0;
            Frame = 0;
            Frameskip = 0;
            state = System.State as State;
            state.LCD = new LCD(System.MMU as MMU, System.State as State);
            state.LCD.Mode = GPUMode.HBlank;
            mmu = System.MMU as MMU;
            spriteSize = 8;
            lcdEnabled = true;
            spritesCurrentLine = Enumerable.Empty<Sprite>();
            backgroundMap = new BackgroundMap(mmu);
            currentLine = new uint[Width];
        }

        public void TurnOffLCD()
        {
            state.LCD.Mode = GPUMode.HBlank;
            Cycles = 0;
        }

        public override void StepCycle()
        {
            if (!lcdEnabled)
            {
                lcdEnabled = state.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable);

                if (!lcdEnabled)
                    return;
            }

            switch (state.LCD.Mode)
            {
                case GPUMode.HBlank: HBlank(); break;
                case GPUMode.VBlank: VBlank(); break;
                case GPUMode.ScanlineOAM: ScanlineOAM(); break;
                case GPUMode.ScanlineVRAM: ScanlineVRAM(); break;
            }
        }

        private void ScanlineVRAM()
        {
            if (Cycles >= 172)
            {
                state.LCD.Mode = GPUMode.HBlank;
                Cycles -= 172;
            }
        }

        private void ScanlineOAM()
        {
            if (Cycles >= 80)
            {
                state.LCD.Mode = GPUMode.ScanlineVRAM;

                if (state.LCD.GetSTATFlag(STAT.Mode2OAMInterrupt))
                    state.RequestInterrupt(InterruptType.LcdStat);

                if (!SkipFrame && lcdEnabled && spriteDisplay)
                    spritesCurrentLine = mmu.OAM.GetSpritesForScanline(state.LCD.LY, spriteSize);

                Cycles -= 80;
            }
        }

        private void VBlank()
        {
            if (Cycles >= 456)
            {
                if (state.LCD.LY == 144)
                    state.RequestInterrupt(InterruptType.VBlank);

                if (state.LCD.GetSTATFlag(STAT.Mode1VBlankInterrupt))
                    state.RequestInterrupt(InterruptType.LcdStat);

                Cycles -= 456;
                state.LCD.LY = (byte)((state.LCD.LY + 1) % 154);

                if (state.LCD.LY == 0)
                {
                    state.LCD.Mode = GPUMode.ScanlineOAM;
                    lcdEnabled = state.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable);

                    if (!lcdEnabled)
                        TurnOffLCD();

                    Frame++;
                }
            }
        }

        private void HBlank()
        {
            if (Cycles >= 204)
            {
                mmu.VRAM.ExecuteHBlankDMATransfer();

                if (state.LCD.GetSTATFlag(STAT.Mode0HBlankInterrupt))
                    state.RequestInterrupt(InterruptType.LcdStat);

                if (!SkipFrame && lcdEnabled)
                {
                    windowDisplay = state.LCD.GetLCDCFlag(LCDC.WindowDisplayEnable);
                    spriteDisplay = state.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable);
                    spriteSize = state.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8;
                    bgDisplay = state.LCD.GetLCDCFlag(LCDC.BGDisplayEnable);

                    Renderscan();
                }

                state.LCD.LY++;
                Cycles -= 204;
                state.LCD.Mode = state.LCD.LY == 144 ? GPUMode.VBlank : GPUMode.ScanlineOAM;
            }
        }

        private void Renderscan()
        {
            if (bgDisplay || GBCMode)
                RenderBGWindowScanline();
            
            if (spriteDisplay)
                RenderOAMScanline();

            UpdateFramebuffer();
        }

        private void UpdateFramebuffer()
        {
            for (int i = 0; i < currentLine.Length; i++)
                SetPixel(i, state.LCD.LY, currentLine[i]);
        }

        private void SetBGWindowPixel(Palette palette, int padding, int i, bool horizontalFlip)
        {
            int x;

            for (int j = 0; j < palette.Length; j++)
            {
                if (horizontalFlip)
                    x = ((7 - j) + (i * 8)) - (padding % 8);
                else
                    x = (j + (i * 8)) - (padding % 8);

                if (x >= 0 && x < Width)
                    currentLine[x] = palette[j];
            }
        }

        private void RenderBGWindowScanline()
        {
            byte line = (byte)((state.LCD.LY + state.LCD.SCY));
            byte windowLine = (byte)(state.LCD.LY - state.LCD.WY);
            bool windowEnabled = windowDisplay && state.LCD.LY >= state.LCD.WY;
            Palette windowPalette = new Palette();
            Palette bgPalette = new Palette();
            Background background = default;
            tileStartAddress = state.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x0000 : 0x0800;
            bgMapSelect = state.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x1C00 : 0x1800;
            windowMapSelect = state.LCD.GetLCDCFlag(LCDC.WindowTileMapDisplaySelect) ? 0x1C00 : 0x1800;
            backgroundMap.WindowMapSelect = windowMapSelect;
            backgroundMap.BackgroundMapSelect = bgMapSelect;

            for (int i = 0; i <= Width / Palette.Size; i++)
            {
                bool drawWindow = false;
                int wx = state.LCD.WX - 7;

                //window
                if (windowEnabled && i >= wx)
                {
                    backgroundMap.Window = true;
                    background = SetBGWindowPalette(windowPalette, i, windowLine);
                    drawWindow = true;
                }

                if (!drawWindow && (bgDisplay))
                {
                    backgroundMap.Window = false;
                    background = SetBGWindowPalette(bgPalette, i, line, state.LCD.SCX);
                }

                if (drawWindow)
                    SetBGWindowPixel(windowPalette, wx, i, background.HorizontalFlip);
                else
                    SetBGWindowPixel(bgPalette, state.LCD.SCX, i, background.HorizontalFlip);
            }
        }

        private Background SetBGWindowPalette(Palette palette, int i, int line, int padding = 0)
        {
            backgroundMap.TileStartAddress = tileStartAddress;

            int xb = backgroundMap.GetCoordinateFromPadding((8 * i) + padding);
            int yb = backgroundMap.GetCoordinateFromPadding((line));
            Tile tile = backgroundMap[xb % 32, yb % 32];
            Background bg;

            if (GBCMode)
            {
                bg = mmu.VRAM.GetBackgroundPaletteType(tile.MapAddress);
                palette.Type = bg.BackgroundPaletteNumber;
            }
            else
            {
                bg = default;
                palette.Type = PaletteType.BGP;
            }

            if (bg.VerticalFlip)
                palette.Address = tile.TileAddress + (2 * ((7 - line) % 8));
            else
                palette.Address = tile.TileAddress + (2 * (line % 8));

            UpdatePaletteFromBytes(palette, mmu.VRAM[palette.Address], mmu.VRAM[palette.Address + 1]);
            return bg;
        }

        private void RenderOAMScanline()
        {
            Palette objPalette = new Palette();

            foreach (var sprite in spritesCurrentLine)
            {
                objPalette.Address = sprite.PaletteAddress;

                if (GBCMode)
                    objPalette.Type = sprite.ColorPaletteType;
                else
                    objPalette.Type = sprite.PaletteType;

                UpdatePaletteFromBytes(objPalette, mmu.VRAM[objPalette.Address], mmu.VRAM[objPalette.Address + 1]);
                DrawSpriteCurrentLine(sprite, objPalette);
            }
        }

        private void DrawSpriteCurrentLine(Sprite sprite, Palette objPalette)
        {
            int coordX;

            for (int j = 0; j < objPalette.Length; j++)
            {
                if (objPalette[j] == 0)
                    continue;

                if (sprite.XFlip)
                    coordX = ((objPalette.Length - j) + sprite.X) - 1;
                else
                    coordX = j + sprite.X;

                if (coordX >= 0 && coordX < 160)
                {
                    if (GBCMode)
                    {
                        if (!sprite.Priority || (GetBGColors().Contains(currentLine[coordX])))
                            currentLine[coordX] = objPalette[j];
                    }
                    else
                    {
                        var color = Palette.ShadeToRGB(state.LCD.BGP, 0);

                        if (!sprite.Priority || (currentLine[coordX] == color))
                            currentLine[coordX] = objPalette[j];
                    }
                }
            }
        }

        private uint GetShadeFrom(Palette palette, int colorNumber)
        {
            byte val = 0;

            switch (palette.Type)
            {
                case PaletteType.BGP: val = state.LCD.BGP; break;
                case PaletteType.OBP0: val = state.LCD.OBP0; break;
                case PaletteType.OPB1: val = state.LCD.OBP1; break;
            }

            return Palette.ShadeToRGB(val, colorNumber);
        }

        private IEnumerable<uint> GetBGColors()
        {
            foreach (var paletteType in Palette.BackgroundPalettes)
            {
                int pos = Palette.GetIndexFromPalette(paletteType);
                var firstByte = mmu.ColorPaletteData.BackgroundPalettes[pos];
                var secondByte = mmu.ColorPaletteData.BackgroundPalettes[pos + 1];
                yield return Palette.ColorToRGB((ushort)((secondByte << 8) | firstByte));
            }
        }

        private uint GetColorFrom(Palette palette, int colorNumber)
        {
            byte firstByte, secondByte;
            int pos = Palette.GetIndexFromPalette(palette.Type) + (colorNumber * 2);

            if (palette.IsBackgroundPalette)
            {
                firstByte = mmu.ColorPaletteData.BackgroundPalettes[pos];
                secondByte = mmu.ColorPaletteData.BackgroundPalettes[pos + 1];
            }
            else
            {
                firstByte = mmu.ColorPaletteData.SpritePalettes[pos];
                secondByte = mmu.ColorPaletteData.SpritePalettes[pos + 1];
            }

            return Palette.ColorToRGB((ushort)((secondByte << 8) | firstByte));
        }

        private void UpdatePaletteFromBytes(Palette palette, byte byte1, byte byte2)
        {
            for (int i = 0; i < Palette.Size; i++)
            {
                byte colorNumber = 0;
                int v1 = (1 << (7 - i));
                
                if ((byte2 & v1) == v1)
                    colorNumber += 2;

                if ((byte1 & v1) == v1)
                    colorNumber += 1;

                if (palette.IsBackgroundPalette || colorNumber > 0)
                    palette[i] = palette.IsColorPalette ? GetColorFrom(palette, colorNumber) : GetShadeFrom(palette, colorNumber);
                else
                    palette[i] = 0;
            }
        }
    }
}