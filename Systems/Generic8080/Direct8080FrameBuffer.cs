using bEmu.Core.Extensions;
using bEmu.Core.Video;
using bEmu.Core.IO;

namespace bEmu.Systems.Generic8080
{
    public class Direct8080FrameBuffer : FrameBuffer
    {
        private readonly byte[] data;
        private readonly MMU mmu;
        public bool CustomColors { get; set; }
        public bool UseBackdrop { get; set; }
        private Bitmap backdrop;

        public Direct8080FrameBuffer(int width, int height, MMU mmu) : base(width, height)
        {
            this.mmu = mmu;
            this.data = new byte[Width * Height * 4];
            backdrop = Bitmap.Read(AssetLoader.Load(mmu.System, "backdrop.bmp").ToBytes());
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
                        var value = this[j, i].ToUInt();
                            
                        data[counter++] = (byte)((value & 0xFF000000) >> 24);
                        data[counter++] = (byte)((value & 0x00FF0000) >> 16);
                        data[counter++] = (byte)((value & 0x0000FF00) >> 8);
                        data[counter++] = (byte)((value & 0x000000FF));
                    }
                }

                return data;
            }
        }

        public override Pixel this[int x, int y]
        {
            get 
            {
                y = (Height - 1 - y);
                byte sprite = mmu[PPU.VRAMAddress + ((x * Height / 8) + y / 8)];

                if ((sprite & (1 << y % 8)) > 0)
                    return GetColor(y);
                else
                {
                    if (UseBackdrop)
                        return backdrop[x, y];
                    else
                        return (0x000000FF);
                }
            }
        }

        private Pixel GetColor(int y)
        {
            if (!CustomColors)
                return (0xFFFFFFFF);

            if (y >= 0 && y <= 100)
                return (0x00FF00FF);
            else if (y > 100 && y <= 200)
                return (0xFFFFFFFF);
            else
                return (0xFF0000FF);
        }
    }
}