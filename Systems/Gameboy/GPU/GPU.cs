using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bEmu.Core;
using bEmu.Core.CPU.LR35902;
using bEmu.Core.System;
using bEmu.Core.Video;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy.GPU
{
    public class GPU : Core.Video.PPU
    {
        private State state;
        private MMU mmu;
        private int spriteSize;
        private bool lcdEnabled;
        private bool bgDisplay;
        private bool windowDisplay;
        private bool spriteDisplay;
        private IEnumerable<Sprite> spritesCurrentLine;
        private bool GBCMode => (System as System).GBCMode;
        private BackgroundMap backgroundMap;
        private uint[] currentLine;
        private IPaletteData paletteData => (GBCMode ? (IPaletteData) mmu.ColorPaletteData : (IPaletteData) mmu.MonochromePaletteData);
        private IColorPalette colorPalette => (System as System).ColorPalette;

        public GPU(IGBSystem system) : base(system as IVideoSystem, 160, 144)
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();

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

        private void TurnOffLCD()
        {
            state.LCD.Mode = GPUMode.HBlank;
            Cycles = 0;
            lcdEnabled = false;
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

                if (!System.SkipFrame && lcdEnabled && spriteDisplay)
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

                    IncreaseFrame();
                }
            }
        }

        private void HBlank()
        {
            if (Cycles >= 204)
            {
                if (state.LCD.LY < 144)
                    mmu.VRAM.ExecuteHBlankDMATransfer();
                    
                if (state.LCD.GetSTATFlag(STAT.Mode0HBlankInterrupt))
                    state.RequestInterrupt(InterruptType.LcdStat);

                if (!System.SkipFrame && lcdEnabled && state.LCD.LY < 144)
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
                Framebuffer[i, state.LCD.LY] = (currentLine[i]);
        }

        private void Push(Palette palette, int padding, int x)
        {
            for (int i = 0; i < palette.Length; i++)
            {
                int x2 = (i + (x * 8)) - (padding % 8);

                if (x2 >= 0 && x2 < Width)
                    currentLine[x2] = palette[i];
            }
        }

        private void RenderBGWindowScanline()
        {
            bool windowEnabled = windowDisplay && state.LCD.LY >= state.LCD.WY;
            backgroundMap.WindowMapSelect = state.LCD.GetLCDCFlag(LCDC.WindowTileMapDisplaySelect) ? 0x1C00 : 0x1800;
            backgroundMap.BackgroundMapSelect = state.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x1C00 : 0x1800;
            backgroundMap.TileStartAddress = state.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x0000 : 0x0800;
            int wx = state.LCD.WX - 7;

            for (int x = 0; x <= (Width / Palette.PaletteSize); x++)
            {
                if (windowEnabled && x * Palette.PaletteSize >= wx)
                {
                    backgroundMap.Window = true;
                    int line = (state.LCD.LY - state.LCD.WY) & 0xFF;
                    var tile = GetTile(x - (wx / 8), line, 0);
                    Push(GetBGWindowPalette(tile, line), wx, x);
                }
                else if (bgDisplay)
                {
                    backgroundMap.Window = false;
                    int line = (state.LCD.LY + state.LCD.SCY) & 0xFF;
                    var tile = GetTile(x, line, state.LCD.SCX);
                    Push(GetBGWindowPalette(tile, line), state.LCD.SCX, x);
                }
            }
        }

        private Palette GetBGWindowPalette(Tile tile, int line)
        {
            var type = GBCMode ? tile.BackgroundPaletteNumber : PaletteType.BGP;
            int address;

            if (tile.VerticalFlip)
                address = tile.TileAddress + (2 * ((7 - (line % 8))));
            else
                address = tile.TileAddress + (2 * (line % 8));

            Palette palette = new Palette(type, address);

            ushort tileData = GetTileData(tile.TileVRAMBankNumber, palette.Address);
            palette.SetColors(paletteData, tileData, tile.HorizontalFlip, colorPalette);
            
            return palette;
        }

        private ushort GetTileData(int tileDataBankNumber, int address)
        {
            if (tileDataBankNumber == 0)
                return (ushort) ((mmu.VRAM.Bank0[address + 1] << 8) | mmu.VRAM.Bank0[address]);
            else
                return (ushort) ((mmu.VRAM.Bank1[address + 1] << 8) | mmu.VRAM.Bank1[address]);
        }
        
        private Tile GetTile(int x, int line, int padding)
        {
            int xb = backgroundMap.GetCoordinateFromPadding((8 * x) + padding);
            int yb = backgroundMap.GetCoordinateFromPadding((line));

            return backgroundMap[xb % 32, yb % 32];
        }

        private void RenderOAMScanline()
        {
            foreach (var sprite in spritesCurrentLine)
            {                    
                var type = GBCMode ? sprite.ColorPaletteType : sprite.PaletteType;
                Palette palette = new Palette(type, sprite.PaletteAddress);
                ushort tileData;

                if (GBCMode)
                {
                    if (sprite.TileVRAMBankNumber == 1)
                        tileData = (ushort) ((mmu.VRAM.Bank1[palette.Address + 1] << 8) | mmu.VRAM.Bank1[palette.Address]);
                    else
                        tileData = (ushort) ((mmu.VRAM.Bank0[palette.Address + 1] << 8) | mmu.VRAM.Bank0[palette.Address]);
                }
                else
                    tileData = (ushort) ((mmu.VRAM[palette.Address + 1] << 8) | mmu.VRAM[palette.Address]);

                palette.SetColors(paletteData, tileData, sprite.XFlip, colorPalette);
                DrawSpriteCurrentLine(sprite, palette);
            }
        }

        private void DrawSpriteCurrentLine(Sprite sprite, Palette palette)
        {
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i] == 0)
                    continue;

                int coordX = i + sprite.X;

                if (coordX >= 0 && coordX < 160)
                {
                    if (GBCMode)
                    {
                        if (!sprite.Priority || (Util.GetBGColors(mmu.ColorPaletteData).Contains(currentLine[coordX])))
                            currentLine[coordX] = palette[i];
                    }
                    else
                    {
                        var color = Util.ShadeToRGB(colorPalette, mmu.MonochromePaletteData.BGP, 0);

                        if (!sprite.Priority || (currentLine[coordX] == color))
                            currentLine[coordX] = palette[i];
                    }
                }
            }
        }
    }
}