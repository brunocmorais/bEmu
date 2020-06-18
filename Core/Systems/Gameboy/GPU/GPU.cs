using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class GPU : Core.PPU
    {
        private readonly uint[] palette;
        private readonly State state;
        private readonly MMU mmu;
        private GPUMode Mode;
        private OAM oam;
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

        public GPU(System system) : base(system, 160, 144) 
        {
            Mode = GPUMode.HBlank;
            Cycles = 0;
            Frame = 0;
            Frameskip = 0;
            state = System.State as State;
            state.LCD = new LCD(System.MMU as MMU);
            mmu = System.MMU as MMU;
            spriteSize = 8;
            lcdEnabled = true;
            palette = new uint[8];
            spritesCurrentLine = Enumerable.Empty<Sprite>();
            oam = new OAM(mmu);
            oam.UpdateSprites();
        }

        private uint GetColorFrom(PaletteType type, int colorNumber)
        {
            byte val = 0;

            switch (type)
            {
                case PaletteType.BGP: val = state.LCD.BGP; break;
                case PaletteType.OBP0: val = state.LCD.OBP0; break;
                case PaletteType.OPB1: val = state.LCD.OBP1; break;
            }

            int bitOffset = (colorNumber * 2);
            byte color = GetColor(((val & (3 << bitOffset)) >> bitOffset));

            return (uint)((color << 24) | (color << 16) | (color << 8) | 0xFF);
        }

        private byte GetColor(int shadeNumber)
        {
            switch (shadeNumber)
            {
                case 0: return 0xFF;
                case 1: return 0xC0;
                case 2: return 0x60;
                case 3: return 0x00;
                default: throw new Exception();
            }
        }

        private void GetPaletteFromBytes(PaletteType paletteType, byte byte1, byte byte2)
        {
            for (int i = 0; i < 8; i++)
            {
                byte colorNumber = 0;
                int v1 = (1 << (7 - i));
                
                if ((byte2 & v1) == v1)
                    colorNumber += 2;

                if ((byte1 & v1) == v1)
                    colorNumber += 1;

                if (paletteType == PaletteType.BGP || colorNumber > 0)
                    palette[i] = GetColorFrom(paletteType, colorNumber);
                else
                    palette[i] = 0;
            }
        }

        public void SetLCYRegisterCoincidence(int ly)
        {
            bool lycCoincidence = ly == state.LCD.LYC;
            state.LCD.SetSTATFlag(STAT.CoincidenceFlag, lycCoincidence);

            if (lycCoincidence && state.LCD.GetSTATFlag(STAT.LYCoincidenceInterrupt) == 1)
                state.RequestInterrupt(InterruptType.LcdStat);
        }

        public void TurnOffLCD()
        {
            Mode = GPUMode.HBlank;
            state.LCD.SetSTATMode(0);
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

            switch (Mode)
            {
                case GPUMode.HBlank:
                    if (Cycles >= 204)
                    {
                        if (state.LCD.GetSTATFlag(STAT.Mode0HBlankInterrupt) == 1)
                            state.RequestInterrupt(InterruptType.LcdStat);

                        if (!SkipFrame && lcdEnabled)
                        {
                            windowDisplay = state.LCD.GetLCDCFlag(LCDC.WindowDisplayEnable);
                            spriteDisplay = state.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable);
                            spriteSize = state.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8;
                            bgDisplay = state.LCD.GetLCDCFlag(LCDC.BGDisplayEnable);

                            Renderscan();
                        }

                        SetLCYRegisterCoincidence(++state.LCD.LY);

                        Cycles -= 204;

                        if (state.LCD.LY == 144)
                            Mode = GPUMode.VBlank;
                        else
                            Mode = GPUMode.ScanlineOAM;

                        state.LCD.SetSTATMode((int) Mode);
                    }

                    break;
                case GPUMode.VBlank:

                    if (Cycles >= 456)
                    {
                        if (state.LCD.LY == 144)
                            state.RequestInterrupt(InterruptType.VBlank);
                        
                        if (state.LCD.GetSTATFlag(STAT.Mode1VBlankInterrupt) == 1)
                            state.RequestInterrupt(InterruptType.LcdStat);

                        Cycles -= 456;
                        SetLCYRegisterCoincidence(++state.LCD.LY);

                        if (state.LCD.LY > 153)
                        {
                            state.LCD.LY = 0;
                            SetLCYRegisterCoincidence(0);
                            Mode = GPUMode.ScanlineOAM;
                            state.LCD.SetSTATMode((int) Mode);

                            lcdEnabled = state.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable);

                            if (!lcdEnabled)
                                TurnOffLCD();
                            else if (!SkipFrame)
                            {
                                tileStartAddress = state.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x0000 : 0x0800;
                                bgMapSelect = state.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x1C00 : 0x1800;
                                windowMapSelect = state.LCD.GetLCDCFlag(LCDC.WindowTileMapDisplaySelect) ? 0x1C00 : 0x1800;
                            }

                            Frame++;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (Cycles >= 80)
                    {
                        Mode = GPUMode.ScanlineVRAM;
                        state.LCD.SetSTATMode((int) Mode);

                        if (state.LCD.GetSTATFlag(STAT.Mode2OAMInterrupt) == 1)
                            state.RequestInterrupt(InterruptType.LcdStat);

                        if (!SkipFrame && lcdEnabled && spriteDisplay)
                        {
                            spritesCurrentLine = oam.GetSpritesForScanline(state.LCD.LY, spriteSize);
                            oam.UpdateSprites();
                        }
                        
                        Cycles -= 80;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (Cycles >= 172)
                    {
                        Mode = GPUMode.HBlank;
                        state.LCD.SetSTATMode((int) Mode);
                        Cycles -= 172;
                    }

                    break;
            }
        }

        private void Renderscan()
        {
            if (bgDisplay)
            {
                RenderBackgroundScanline();

                if (windowDisplay)
                    RenderWindowScanline();
            }
            
            if (spriteDisplay)
                RenderOAMScanline();
        }

        private void RenderWindowScanline()
        {
            if (state.LCD.LY < state.LCD.WY)
                return;

            int line = state.LCD.LY - state.LCD.WY;

            for (int i = 0; i <= 20; i++)
            {
                var wx = state.LCD.WX - 7;

                if (i < wx)
                    continue;

                int addr = windowMapSelect + ((i + (wx / 8)) % 32) + (line / 8 * 32);
                
                byte tileNumber = mmu.VRAM[addr];
                int paletteAddr;

                if (tileStartAddress == 0)
                    paletteAddr = tileStartAddress + (tileNumber << 4) + (2 * (line % 8));
                else
                    paletteAddr = ((tileNumber & 0x80) == 0x80 ? 0x800 : 0x1000) + ((tileNumber & 0x7F) << 4) + (2 * (line % 8));

                GetPaletteFromBytes(PaletteType.BGP, mmu.VRAM[paletteAddr], mmu.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    int x = (j + (i * 8)) - (wx % 8);

                    if (x >= 0 && x < 160)
                        SetPixel(x, state.LCD.LY, palette[j]);
                }
            }
        }

        private void RenderBackgroundScanline()
        {
            byte line = (byte)((state.LCD.LY + state.LCD.SCY));
            int paletteAddr;

            for (int i = 0; i <= 20; i++)
            {
                int addr = bgMapSelect + ((i + (state.LCD.SCX / 8)) % 32) + (line / 8 * 32);

                if (tileStartAddress == 0)
                    paletteAddr = tileStartAddress + (mmu.VRAM[addr] << 4) + (2 * (line % 8));
                else
                    paletteAddr = ((mmu.VRAM[addr] & 0x80) == 0x80 ? 0x800 : 0x1000) + ((mmu.VRAM[addr] & 0x7F) << 4) + (2 * (line % 8));

                GetPaletteFromBytes(PaletteType.BGP, mmu.VRAM[paletteAddr], mmu.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    int x = (j + (i * 8)) - (state.LCD.SCX % 8);

                    if (x >= 0 && x < 160)
                        SetPixel(x, state.LCD.LY, palette[j]);
                }
            }
        }

        private void RenderOAMScanline()
        {
            foreach (var sprite in spritesCurrentLine)
            {
                int paletteAddr = (sprite.Address << 4) + (2 * ((sprite.LineOffset) % sprite.Size));
                GetPaletteFromBytes(sprite.PaletteType, mmu.VRAM[paletteAddr], mmu.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    if (palette[j] == 0)
                        continue;

                    int coordX;

                    if (sprite.XFlip)
                        coordX = ((palette.Length - j) + sprite.X) - 1;
                    else
                        coordX = j + sprite.X;

                    if (coordX >= 0 && coordX < 160)
                    
                        if (!sprite.Priority || GetPixel(coordX, state.LCD.LY) == GetColorFrom(PaletteType.BGP, 0))
                            SetPixel(coordX, state.LCD.LY, palette[j]);
                }
            }
        }
    }
}