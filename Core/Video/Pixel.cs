using System;

namespace bEmu.Core.Video
{
    public struct Pixel
    {
        private uint value;
        public byte R => ((byte)(value & (0xFF << 24) >> 24));
        public byte G => ((byte)(value & (0xFF << 16) >> 16));
        public byte B => ((byte)(value & (0xFF <<  8) >>  8));
        public byte A => ((byte)(value & 0xFF));

        Pixel(uint value)
        {
            this.value = value;
        }

        public uint ToUInt() => value;
        public static implicit operator Pixel(uint value) => new Pixel(value);
        public static implicit operator uint(Pixel value) => value.value;
    }
}