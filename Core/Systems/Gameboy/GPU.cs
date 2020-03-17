using System;
using System.Diagnostics;
using bEmu.Core;

namespace bEmu.Core.Systems.Gameboy
{
    public class GPU : Core.PPU
    {
        int cycles = 0;
        int line = 0;
        GPUMode mode = GPUMode.HBlank;
        public Color[,] frameBuffer = new Color[160, 144];
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

        private Color[] GetPaletteFromBytes(byte byte1, byte byte2)
        {
            Color[] palette = new Color[8];

            for (int i = 0; i < 8; i++)
            {
                int color = 0;
                int v = (7 - i);

                if (((byte2 & (1 << v)) >> v) == 1)
                    color += 2;

                if (((byte1 & (1 << v)) >> v) == 1)
                    color += 1;

                palette[i] = (Color) color;
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

            switch (mode)
            {
                case GPUMode.HBlank:
                    if (cycles >= 204)
                    {
                        line++;
                        cycles = 0;

                        if (line == 143)
                            mode = GPUMode.VBlank;
                        else
                            mode = GPUMode.ScanlineOAM;
                    }

                    break;
                case GPUMode.VBlank:

                    if (cycles >= 456)
                    {
                        cycles = 0;
                        line++;

                        if (line > 153)
                        {
                            line = 0;
                            mode = GPUMode.ScanlineOAM;
                        }
                    }
                    break;
                case GPUMode.ScanlineOAM:
                    if (cycles >= 80)
                    {
                        mode = GPUMode.ScanlineVRAM;
                        cycles = 0;
                    }
                    break;
                case GPUMode.ScanlineVRAM:
                    if (cycles >= 172)
                    {
                        mode = GPUMode.HBlank;
                        cycles = 0;
                        Renderscan();
                    }

                    break;
            }
        }

        public void Renderscan()
        {            
            // for (int x = 0; x < 20; x++)
            // {
            //     var tileAddr = 0x8000 + (2 * line) + (x * 16) + ((line / 8) * 320);
            //     var palette = GetPaletteFromBytes(System.MMU[tileAddr], System.MMU[tileAddr + 1]);

            //     for (int j = 0; j < palette.Length; j++)
            //         frameBuffer[j + (x * 8), line] = palette[j];
            // }
            int scx = State.SCX / 8;
            int scy = State.SCY / 2;

            for (int i = 0; i < 20; i++)
            {
                var tileNumber = System.MMU[0x9800 + i + scx + (((line + scy) / 8) * 32)];
                var paletteAddr = 0x8000 + (tileNumber << 4) + (2 * ((line + scy) % 8));
                var palette = GetPaletteFromBytes(System.MMU[paletteAddr], System.MMU[paletteAddr + 1]);

                for (int j = 0; j < palette.Length; j++)
                    frameBuffer[j + (i * 8), line] = palette[j];
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
}