using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using bEmu.Core.Extensions;
using bEmu.Core.Util;
using bEmu.Core.Video;

namespace bEmu.Core.Video
{
    public class Bitmap
    {
        private const int BMPHeaderSize = 0xE;
        private const int DIBHeaderSize = 0x28;
        private const int HeaderSize = BMPHeaderSize + DIBHeaderSize;

        public int Width { get; }
        public int Height { get; }
        private Pixel[,] pixels;
        public int Length => Width * Height;

        public Pixel this[int x, int y] 
        {
            get => pixels[x, y];
            set => pixels[x, y] = value;
        }

        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            pixels = new Pixel[Width, Height];
        }

        public static Bitmap Read(byte[] bytes)
        {
            int width = (int) LittleEndian.GetDWordFrom4Bytes(bytes[0x12], bytes[0x13], bytes[0x14], bytes[0x15]);
            int height = (int) LittleEndian.GetDWordFrom4Bytes(bytes[0x16], bytes[0x17], bytes[0x18], bytes[0x19]);

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

                    uint v = LittleEndian.GetDWordFrom4Bytes(a, b, g, r);
                    bitmap[i, j] = v;
                }
            }

            return bitmap;
        }

        public static Bitmap From(IFrameBuffer frameBuffer)
        {
            var bitmap = new Bitmap(frameBuffer.Width, frameBuffer.Height);

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                    bitmap[i, j] = frameBuffer[i, j];

            return bitmap;
        }

        public byte[] ToBytes()
        {            
            using (var stream = new MemoryStream())
            {
                // BMP Header
                stream.WriteUShort(0x4D42);
                stream.WriteUInt((uint)(HeaderSize + (Length * 4)));
                stream.WriteUInt(0);
                stream.WriteUInt(HeaderSize);

                // DIB Header

                stream.WriteUInt(DIBHeaderSize);
                stream.WriteUInt((uint) Width);
                stream.WriteUInt((uint) Height);
                stream.WriteUShort(1);
                stream.WriteUShort(32);
                stream.WriteUInt(0);
                stream.WriteUInt((uint) (Length * 4));
                stream.WriteUInt((uint) 0x0B13);
                stream.WriteUInt((uint) 0x0B13);
                stream.WriteUInt(0);
                stream.WriteUInt(0);

                // Data

                for (int j = Height - 1; j >= 0; j--)
                {
                    for (int i = 0; i < Width; i++)
                    {
                        var bytes = LittleEndian.ToBytes(this[i, j].ToUInt()).ToArray();
                        stream.WriteUInt(LittleEndian.GetDWordFrom4Bytes(bytes[2], bytes[1], bytes[0], bytes[3]));
                    }
                }

                return stream.ToArray();
            }
        }
    }
}