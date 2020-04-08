using System;
using System.Collections.Generic;
using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class GPU : Core.PPU
    {
        int cycles = 0;
        GPUMode Mode = GPUMode.HBlank;
        Color[,] frameBuffer = new Color[160, 144];
        State State => System.State as bEmu.Core.Systems.Gameboy.State;
        IEnumerable<Sprite> spritesCurrentLine;
        OAM oam;
        public int Frame { get; private set; } = 0;

        public GPU(ISystem system, int width, int height) : base(system, width, height) 
        { 
            oam = new OAM(system);
            oam.UpdateSprites();
        }

        private Pixel GetPixel(Color color)
        {
            switch (color)
            {
                case Color.White: return Pixel.White;
                case Color.Gray33: return new Pixel(192, 192, 192, 255);
                case Color.Gray66: return new Pixel(96, 96, 96, 255);
                case Color.Black: return Pixel.Black;
                default: throw new ArgumentException();
            }
        }

        private Color GetColorFrom(PaletteType type, int colorNumber)
        {
            byte val;

            switch (type)
            {
                case PaletteType.BGP: val  = State.LCD.BGP;  break;
                case PaletteType.OBP0: val = State.LCD.OBP0; break;
                case PaletteType.OPB1: val = State.LCD.OBP1; break;
                default: return default(Color);
            }

            int bitOffset = (colorNumber * 2);
            return (Color) ((val & (3 << bitOffset)) >> bitOffset);
        }

        private Color?[] GetPaletteFromBytes(PaletteType paletteType, byte byte1, byte byte2)
        {
            Color?[] palette = new Color?[8];

            for (int i = 0; i < 8; i++)
            {
                int color = 0;
                int v = (7 - i);

                if (((byte2 & (1 << v)) >> v) == 1)
                    color += 2;

                if (((byte1 & (1 << v)) >> v) == 1)
                    color += 1;

                if (paletteType == PaletteType.BGP || color > 0)
                    palette[i] = GetColorFrom(paletteType, color);
            }

            return palette;
        }

        public override Pixel this[int x, int y] 
        {
            get
            {
                return GetPixel(frameBuffer[x, y]);
            }   
        }

        public void StepCycle(int cyclesLastOperation)
        {
            cycles += cyclesLastOperation;
            State.LCD.SetSTATMode((int) Mode);
            bool lycCoincidence = State.LCD.LY == State.LCD.LYC;
            State.LCD.SetSTATFlag(STAT.CoincidenceFlag, lycCoincidence);

            if (lycCoincidence && State.LCD.GetSTATFlag(STAT.LYCoincidenceInterrupt) == 1)
                State.RequestInterrupt(InterruptType.LcdStat);

            switch (Mode)
            {
                case GPUMode.HBlank:
                    if (cycles >= 204)
                    {
                        Renderscan();
                        State.LCD.LY++;
                        cycles -= 204;

                        if (State.LCD.LY == 144)
                            Mode = GPUMode.VBlank;
                        else
                            Mode = GPUMode.ScanlineOAM;
                    }

                    break;
                case GPUMode.VBlank:

                    if (cycles >= 456)
                    {
                        if (State.LCD.LY == 144)
                            State.RequestInterrupt(InterruptType.VBlank);

                        cycles -= 456;
                        State.LCD.LY++;

                        if (State.LCD.LY > 153)
                        {
                            State.LCD.LY = 0;
                            Mode = GPUMode.ScanlineOAM;
                            oam.UpdateSprites();
                            Frame++;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (cycles >= 80)
                    {
                        Mode = GPUMode.ScanlineVRAM;
                        spritesCurrentLine = oam.GetSpritesOnLine(State.LCD.LY, State.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8);
                        cycles -= 80;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (cycles >= 172)
                    {
                        Mode = GPUMode.HBlank;
                        cycles -= 172;
                    }

                    break;
            }
        }

        private void Renderscan()
        {
            if (State.LCD.LY < 0 && State.LCD.LY > 143)
                return;

            bool bgDisplay = State.LCD.GetLCDCFlag(LCDC.BGDisplay);
            bool spriteDisplay = State.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable);
            int tileStartAddress = State.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x8000 : 0x8800;
            int mapSelect = State.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x9C00 : 0x9800;

            byte line = (byte) ((State.LCD.LY + State.LCD.SCY));

            if (bgDisplay)
                for (int i = 0; i <= 20; i++) // 20 tiles 8x8
                    RenderBackgroundScanline(tileStartAddress, mapSelect, i, line);
            
            if (spriteDisplay)
                RenderOAMScanline();
        }

        private void RenderOAMScanline()
        {
            foreach (var sprite in spritesCurrentLine)
            {
                int paletteAddr = 0x8000 + (sprite.Address << 4) + (2 * ((sprite.LineOffset) % sprite.Size));
                var palette = GetPaletteFromBytes(sprite.PaletteType, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    if (palette[j].HasValue)
                    {
                        int coordX;

                        if (sprite.XFlip)
                            coordX = ((palette.Length - j) + sprite.X) - 1;
                        else
                            coordX = j + sprite.X;
                        
                        if (coordX >= 0 && coordX < 160)
                            frameBuffer[coordX, State.LCD.LY] = palette[j].Value;
                    }
                }
            }
        }

        private void RenderBackgroundScanline(int tileStartAddress, int mapSelect, int i, byte line)
        {
            int addr = mapSelect + ((i + (State.LCD.SCX / 8)) % 32) + (line / 8 * 32);
            byte tileNumber = System.MMU[addr];
            int paletteAddr;

            if (tileStartAddress == 0x8000)
                paletteAddr = tileStartAddress + (tileNumber << 4) + (2 * (line % 8));
            else
                paletteAddr = ((tileNumber & 0x80) == 0x80 ? 0x8800 : 0x9000) + ((tileNumber & 0x7F) << 4) + (2 * (line % 8));

            var palette = GetPaletteFromBytes(PaletteType.BGP, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

            for (int j = 0; j < palette.Length; j++)
            {
                int x = (j + (i * 8)) - (State.LCD.SCX % 8);

                if (x >= 0 && x < 160)
                    frameBuffer[x, State.LCD.LY] = palette[j].Value;
            }
        }
    }

    public enum GPUMode
    {
        HBlank = 0,
        VBlank = 1,
        ScanlineOAM = 2,
        ScanlineVRAM = 3
    }

    public enum PaletteType
    {
        BGP = 0,
        OBP0 = 1,
        OPB1 = 2
    }
}