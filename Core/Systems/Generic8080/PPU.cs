using bEmu.Core;

namespace bEmu.Core.Systems.Generic8080
{
    public class PPU : Core.PPU
    {
        public PPU(ISystem system, int width, int height) : base(system, width, height) { }

        public override Pixel this[int x, int y] 
        {
            get
            {
                int coordX = (x / 8) * 8;
                int coordY = y;
                int offsetX = x - coordX;

                byte sprite = System.MMU [0x2400 + ((coordY * 256 / 8) + coordX / 8)];

                if ((sprite & (1 << offsetX)) > 0)
                    return Pixel.White;
                else
                    return Pixel.Black;
            }
        }
    }
}