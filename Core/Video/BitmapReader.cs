using System.IO;
using bEmu.Core.CPU;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.Util;

namespace bEmu.Core.Video
{
    public class BitmapReader : Singleton<BitmapReader>, IReader<Bitmap>
    {
        public Bitmap Read(FileStream fileStream)
        {
            return Read(fileStream.ToBytes());
        }

        public Bitmap Read(byte[] bytes)
        {
            int width = (int) BitFacade.GetDWordFrom4Bytes(bytes[0x12], bytes[0x13], bytes[0x14], bytes[0x15]);
            int height = (int) BitFacade.GetDWordFrom4Bytes(bytes[0x16], bytes[0x17], bytes[0x18], bytes[0x19]);

            var bitmap = new Bitmap(width, height);
            int pointer = bytes[0xA];

            for (int j = bitmap.Height - 1; j >= 0; j--)
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    byte b = bytes[pointer++];
                    byte g = bytes[pointer++];
                    byte r = bytes[pointer++];
                    byte a = bytes[pointer++];

                    uint v = BitFacade.GetDWordFrom4Bytes(a, b, g, r);
                    bitmap[i, j] = v;
                }
            }

            return bitmap;
        }
    }
}