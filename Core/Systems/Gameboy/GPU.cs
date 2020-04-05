using System;
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

        public GPU(ISystem system, int width, int height) : base(system, width, height) { }

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
                        State.LCD.LY++;
                        cycles = 0;

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

                        cycles = 0;
                        State.LCD.LY++;

                        if (State.LCD.LY > 153)
                        {
                            State.LCD.LY = 0;
                            Mode = GPUMode.ScanlineOAM;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (cycles >= 80)
                    {
                        Mode = GPUMode.ScanlineVRAM;
                        cycles = 0;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (cycles >= 172)
                    {
                        Mode = GPUMode.HBlank;
                        cycles = 0;
                        Renderscan();
                    }

                    break;
            }
        }

        private void Renderscan()
        {
            RenderBackgroundScanline();
            RenderOAMScanline();
        }

        private void RenderOAMScanline()
        {
            if (!State.LCD.GetLCDCFlag(LCDC.SpriteDisplayEnable))
                return;

            int spriteSize = State.LCD.GetLCDCFlag(LCDC.SpriteSize) ? 16 : 8;

            for (int i = 0; i < 160; i += 4)
            {
                int y = System.MMU[0xFE00 + i + 0] - 16;
                int x = System.MMU[0xFE00 + i + 1] - 8;
                byte tile = System.MMU[0xFE00 + i + 2];
                byte attr = System.MMU[0xFE00 + i + 3];
                PaletteType paletteType = (attr & 0x10) == 0x10 ? PaletteType.OPB1 : PaletteType.OBP0;
                int lineOffset = State.LCD.LY - y;

                if ((attr & 0x40) == 0x40)
                    lineOffset = spriteSize - lineOffset - 1;

                if (lineOffset >= 0 && lineOffset < spriteSize)
                {
                    int paletteAddr = 0x8000 + (tile << 4) + (2 * ((lineOffset) % spriteSize));

                    var palette = GetPaletteFromBytes(paletteType, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

                    for (int j = 0; j < palette.Length; j++)
                    {
                        if (palette[j].HasValue)
                        {
                            int coordX;

                            if ((attr & 0x20) == 0x20)
                                coordX = ((palette.Length - j) + x) - 1;
                            else
                                coordX = j + x;
                            
                            if (coordX >= 0 && coordX < 160)
                                frameBuffer[coordX, State.LCD.LY] = palette[j].Value;
                        }
                    }
                }
            }
        }

        private void RenderBackgroundScanline()
        {
            if (!State.LCD.GetLCDCFlag(LCDC.BGDisplay))
                return;

            int tileStartAddress = State.LCD.GetLCDCFlag(LCDC.BGWindowTileDataSelect) ? 0x8000 : 0x8800;
            int mapSelect = State.LCD.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) ? 0x9C00 : 0x9800;

            for (int i = 0; i <= 20; i++)
            {
                int paletteAddr = 0;
                byte line = (byte) ((State.LCD.LY + State.LCD.SCY));

                int addr = mapSelect + ((i + (State.LCD.SCX / 8)) % 32) + (line / 8 * 32);

                byte tileNumber = System.MMU[addr];

                if (tileStartAddress == 0x8000)
                    paletteAddr = tileStartAddress + (tileNumber << 4) + (2 * (line % 8));
                else
                    paletteAddr = ((tileNumber & 0x80) == 0x80 ? 0x8800 : 0x9000) + ((tileNumber & 0x7F) << 4) + (2 * (line % 8));

                var palette = GetPaletteFromBytes(PaletteType.BGP, System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                {
                    if (palette[j].HasValue)
                    {
                        int x = (j + (i * 8)) - (State.LCD.SCX % 8);
                        int y = State.LCD.LY;

                        if (x >= 0 && x < 160 && y >= 0 && y < 144)
                            frameBuffer[x, y] = palette[j].Value;
                    }
                }
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