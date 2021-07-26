using System;
using System.IO;
using bEmu.Core.Util;

namespace bEmu.Core.Image
{
    public class BitmapReader
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private uint[,] pixels;


        public BitmapReader(string file) : this(File.ReadAllBytes(file))
        {
        }

        public BitmapReader(byte[] bytes)
        {
            Width = (int) ByteOperations.GetDWordFrom4Bytes(bytes[0x12], bytes[0x13], bytes[0x14], bytes[0x15]);
            Height = (int) ByteOperations.GetDWordFrom4Bytes(bytes[0x16], bytes[0x17], bytes[0x18], bytes[0x19]);

            pixels = new uint[Width, Height];
            Parse(bytes);
        }

        public uint this[int x, int y] => pixels[x, y];

        private void Parse(byte[] bytes)
        {
            var pixelArrayStart = bytes[0xA];

            for (int i = Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < Width; j++)
                {
                    pixels[i, j] = ByteOperations.GetDWordFrom4Bytes(bytes[pixelArrayStart++], bytes[pixelArrayStart++], bytes[pixelArrayStart++], 0xFF);

                    if (j % 2 == 1)
                        pixelArrayStart += 2;
                }
            }
        }
    }
}