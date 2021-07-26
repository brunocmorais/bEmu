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

        public ScaledPixel(int scale, uint defaultValue) : this(scale)
        {
            Array.Fill(pixels, defaultValue);
        }

        public uint this[int x, int y]
        {
            get => pixels[y * Scale + x];
            set => pixels[y * Scale + x] = value;
        }
    }
}