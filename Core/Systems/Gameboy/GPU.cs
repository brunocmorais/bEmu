using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class GPU : Core.PPU
    {
        public int Cycles { get; set; } = 0;
        public int Frame { get; private set; } = 0;
        public int Frameskip { get; set; } = 0;
        GPUMode Mode = GPUMode.HBlank;
        State State => System.State as bEmu.Core.Systems.Gameboy.State;
        MMU MMU => System.MMU as bEmu.Core.Systems.Gameboy.MMU;
        IEnumerable<Sprite> spritesCurrentLine = Enumerable.Empty<Sprite>();
        OAM oam;
        int spriteSize = 8;
        bool lcdEnabled = true;
        bool bgDisplay;
        bool windowDisplay;
        bool spriteDisplay;
        int tileStartAddress;
        int windowMapSelect;
        int bgMapSelect;
        bool skipFrame => (Frameskip > 1 && Frame % Frameskip != 0) || (Frameskip == 1 && Frame % 2 == 0);
        readonly uint[] palette = new uint[8];

        public GPU(ISystem system, int width, int height) : base(system, width, height) 
        { 
            oam = new OAM(system.MMU as MMU);
            oam.UpdateSprites();
        }

        private uint GetColorFrom(PaletteType type, int colorNumber)
        {
            byte val = 0;

            switch (type)
            {
                case PaletteType.BGP: val = State.LCD.BGP; break;
                case PaletteType.OBP0: val = State.LCD.OBP0; break;
                case PaletteType.OPB1: val = State.LCD.OBP1; break;
            }

            int bitOffset = (colorNumber * 2);
            byte color = GetColor(((val & (3 << bitOffset)) >> bitOffset));

            return (uint)((color << 24) | (color << 16) | (color << 8) | 0xFF);
        }

        private static byte GetColor(int shadeNumber)
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
            bool lycCoincidence = ly == State.LCD.LYC;
            State.LCD.SetSTATFlag(STAT.CoincidenceFlag, lycCoincidence);

            if (lycCoincidence && State.LCD.GetSTATFlag(STAT.LYCoincidenceInterrupt) == 1)
                State.RequestInterrupt(InterruptType.LcdStat);
        }

        public void TurnOffLCD()
        {
            Mode = GPUMode.HBlank;
            State.LCD.SetSTATMode(0);
            Cycles = 0;
        }

        public void StepCycle()
        {
            if (!lcdEnabled)
            {
                lcdEnabled = State.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable);

                if (!lcdEnabled)
                    return;
            }

            switch (Mode)
            {
                case GPUMode.HBlank:
                    if (Cycles >= 204)
                    {
                        if (State.LCD.GetSTATFlag(STAT.Mode0HBlankInterrupt) == 1)
                            State.RequestInterrupt(InterruptType.LcdStat);

                        if (!skipFrame && lcdEnabled)
                        {
                            windowDisplay = State.LCD.GetLCDCFlag(LCDC.WindowDisplayEnable);
                            spriteDisplay = State.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable);
                            spriteSize = State.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8;
                            bgDisplay = State.LCD.GetLCDCFlag(LCDC.BGDisplayEnable);

                            Renderscan();
                        }

                        SetLCYRegisterCoincidence(++State.LCD.LY);

                        Cycles -= 204;

                        if (State.LCD.LY == 144)
                            Mode = GPUMode.VBlank;
                        else
                            Mode = GPUMode.ScanlineOAM;

                        State.LCD.SetSTATMode((int) Mode);
                    }

                    break;
                case GPUMode.VBlank:

                    if (Cycles >= 456)
                    {
                        if (State.LCD.LY == 144)
                            State.RequestInterrupt(InterruptType.VBlank);
                        
                        if (State.LCD.GetSTATFlag(STAT.Mode1VBlankInterrupt) == 1)
                            State.RequestInterrupt(InterruptType.LcdStat);

                        Cycles -= 456;
                        SetLCYRegisterCoincidence(++State.LCD.LY);

                        if (State.LCD.LY > 153)
                        {
                            State.LCD.LY = 0;
                            SetLCYRegisterCoincidence(0);
                            Mode = GPUMode.ScanlineOAM;
                            State.LCD.SetSTATMode((int) Mode);

                            lcdEnabled = State.LCD.GetLCDCFlag(LCDC.LCDDisplayEnable);

                            if (!lcdEnabled)
                                TurnOffLCD();
                            else if (!skipFrame)
                            {
                                tileStartAddress = State.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x0000 : 0x0800;
                                bgMapSelect = State.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x1C00 : 0x1800;
                                windowMapSelect = State.LCD.GetLCDCFlag(LCDC.WindowTileMapDisplaySelect) ? 0x1C00 : 0x1800;

                                if (spriteDisplay)
                                    oam.UpdateSprites();
                            }

                            Frame++;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (Cycles >= 80)
                    {
                        Mode = GPUMode.ScanlineVRAM;
                        State.LCD.SetSTATMode((int) Mode);

                        if (State.LCD.GetSTATFlag(STAT.Mode2OAMInterrupt) == 1)
                            State.RequestInterrupt(InterruptType.LcdStat);

                        if (!skipFrame && lcdEnabled && spriteDisplay)
                            spritesCurrentLine = oam.GetSpritesOnLine(State.LCD.LY, spriteSize);

                        Cycles -= 80;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (Cycles >= 172)
                    {
                        Mode = GPUMode.HBlank;
                        State.LCD.SetSTATMode((int) Mode);
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
            if (State.LCD.LY < State.LCD.WY)
                return;

            int line = State.LCD.LY - State.LCD.WY;

            for (int i = 0; i <= 20; i++)
            {
                var wx = State.LCD.WX - 7;

                if (i < wx)
                    continue;

                int addr = windowMapSelect + ((i + (wx / 8)) % 32) + (line / 8 * 32);
                
                byte tileNumber = MMU.VRAM[addr];
                int paletteAddr;

                if (tileStartAddress == 0)
                    paletteAddr = tileStartAddress + (tileNumber << 4) + (2 * (line % 8));
                else
                    paletteAddr = ((tileNumber & 0x80) == 0x80 ? 0x800 : 0x1000) + ((tileNumber & 0x7F) << 4) + (2 * (line % 8));

                GetPaletteFromBytes(PaletteType.BGP, MMU.VRAM[paletteAddr], MMU.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    int x = (j + (i * 8)) - (wx % 8);

                    if (x >= 0 && x < 160)
                        SetPixel(x, State.LCD.LY, palette[j]);
                }
            }
        }

        private void RenderBackgroundScanline()
        {
            byte line = (byte)((State.LCD.LY + State.LCD.SCY));
            int paletteAddr;

            for (int i = 0; i <= 20; i++)
            {
                int addr = bgMapSelect + ((i + (State.LCD.SCX / 8)) % 32) + (line / 8 * 32);

                if (tileStartAddress == 0)
                    paletteAddr = tileStartAddress + (MMU.VRAM[addr] << 4) + (2 * (line % 8));
                else
                    paletteAddr = ((MMU.VRAM[addr] & 0x80) == 0x80 ? 0x800 : 0x1000) + ((MMU.VRAM[addr] & 0x7F) << 4) + (2 * (line % 8));

                GetPaletteFromBytes(PaletteType.BGP, MMU.VRAM[paletteAddr], MMU.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    int x = (j + (i * 8)) - (State.LCD.SCX % 8);

                    if (x >= 0 && x < 160)
                        SetPixel(x, State.LCD.LY, palette[j]);
                }
            }
        }

        private void RenderOAMScanline()
        {
            foreach (var sprite in spritesCurrentLine)
            {
                int paletteAddr = (sprite.Address << 4) + (2 * ((sprite.LineOffset) % sprite.Size));
                GetPaletteFromBytes(sprite.PaletteType, MMU.VRAM[paletteAddr], MMU.VRAM[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    if (palette[j] == 0) // transparente
                        continue;

                    int coordX;

                    if (sprite.XFlip)
                        coordX = ((palette.Length - j) + sprite.X) - 1;
                    else
                        coordX = j + sprite.X;

                    if (coordX >= 0 && coordX < 160)
                    
                        if (!sprite.Priority || GetPixel(coordX, State.LCD.LY) == GetColorFrom(PaletteType.BGP, 0))
                            SetPixel(coordX, State.LCD.LY, palette[j]);
                }
            }
        }
    }
}