using System;

namespace bEmu.Core.Video
{
    public struct ScaledPixel
    {
        private Pixel[] pixels;
        public int Scale { get; }

        public ScaledPixel(int scale)
        {
            Scale = scale;
            pixels = new Pixel[Scale * Scale];
        }

        public ScaledPixel(int scale, Pixel defaultValue) : this(scale)
        {
            Array.Fill(pixels, defaultValue);
        }

        public Pixel this[int x, int y]
        {
            get => pixels[y * Scale + x];
            set => pixels[y * Scale + x] = value;
        }
    }
}