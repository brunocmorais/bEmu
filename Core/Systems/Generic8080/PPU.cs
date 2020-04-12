using bEmu.Core;

namespace bEmu.Core.Systems.Generic8080
{
    public class PPU : Core.PPU
    {
        public PPU(ISystem system, int width, int height) : base(system, width, height) { }

        public void UpdateFramebuffer()
        {
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 224; y++)
                {
                    int coordX = (x / 8) * 8;
                    int coordY = y;
                    int offsetX = x - coordX;

                    byte sprite = System.MMU [0x2400 + ((coordY * 256 / 8) + coordX / 8)];

                    //Vector2 coor = new Vector2(y, Width) - (x));

                    try
                    {
                        if ((sprite & (1 << offsetX)) > 0)
                            SetPixel(y, 255 - x, 0xFFFFFFFF);
                        else
                            SetPixel(y, 255 - x, 0x00000000);
                    }
                    catch {}
                }
            }
        }
    }
}