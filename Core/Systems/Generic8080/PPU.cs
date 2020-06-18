using bEmu.Core;

namespace bEmu.Core.Systems.Generic8080
{
    public class PPU : Core.PPU
    {
        private const int VRAMAddress = 0x2400;

        public PPU(System system, int width, int height) : base(system, width, height) { }

        public override void StepCycle()
        {
            
        }

        public void UpdateFrameBuffer()
        {
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    int coordX = (x / 8) * 8;
                    int offsetX = x - coordX;

                    byte sprite = System.MMU [VRAMAddress + ((y * Height / 8) + coordX / 8)];

                    if ((sprite & (1 << offsetX)) > 0)
                        SetPixel(y, Height - 1 - x, 0xFFFFFFFF);
                    else
                        SetPixel(y, Height - 1 - x, 0x00000000);
                }
            }
        }
    }
}