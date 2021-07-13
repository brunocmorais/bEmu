using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Systems.Generic8080
{
    public class DirectFrameBuffer : FrameBuffer
    {
        private readonly byte[] data;
        private readonly MMU mmu;

        public DirectFrameBuffer(int width, int height, MMU mmu) : base(width, height)
        {
            this.mmu = mmu;
            this.data = new byte[Width * Height * 4];
        }

        public override byte[] Data
        {
            get
            {
                int counter = 0;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        uint value = this[j, i];
                        data[counter++] = (byte)((value & 0xFF000000) >> 24);
                        data[counter++] = (byte)((value & 0x00FF0000) >> 16);
                        data[counter++] = (byte)((value & 0x0000FF00) >> 8);
                        data[counter++] = (byte)((value & 0x000000FF));
                    }
                }

                return data;
            }
        }

        public override uint this[int x, int y]
        {
            get 
            {
                y = (Height - 1 - y);
                byte sprite = mmu[PPU.VRAMAddress + ((x * Height / 8) + y / 8)];

                if ((sprite & (1 << y % 8)) > 0)
                    return 0xFFFFFFFF;
                else
                    return 0x00000000;
            }
        }
    }
}