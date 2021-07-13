using System;

namespace bEmu.Core.Video
{
    public struct ScaledPixel
    {
        private uint[] pixels;
        public int Scale { get; }

        public ScaledPixel(int scale)
        {
            Scale = scale;
            pixels = new uint[Scale * Scale];
        }

        public uint this[int x, int y]
        {
            get => pixels[y * Scale + x];
            set => pixels[y * Scale + x] = value;
        }

        public void Fill(uint value)
        {
            Array.Fill(pixels, value);
        }
    }
}