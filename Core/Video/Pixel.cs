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

        public Pixel(uint value)
        {
            this.value = value;
        }

        public uint ToUInt() => value;

        public override bool Equals(object obj) => obj is Pixel pixel && value == pixel.value;

        public override int GetHashCode() => HashCode.Combine(value, R, G, B, A);

        public static bool operator ==(Pixel left, Pixel right) => left.Equals(right);
        public static bool operator !=(Pixel left, Pixel right) => !left.Equals(right);
        public static bool operator ==(Pixel left, uint right) => left.ToUInt() == right;
        public static bool operator !=(Pixel left, uint right) => left.ToUInt() != right;
        public static Pixel operator &(Pixel left, Pixel right) => new Pixel(left.ToUInt() & right.ToUInt());
        public static uint operator &(Pixel left, uint right) => (left.ToUInt() & right);
    }
}