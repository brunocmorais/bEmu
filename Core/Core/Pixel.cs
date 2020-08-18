namespace bEmu.Core
{
    public struct Pixel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public Pixel(byte r, byte g, byte b, byte a = 0xFF)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Pixel FromUint(uint value)
        {
            return new Pixel()
            {
                R = (byte)((value >> 24) & 0xFF),
                G = (byte)((value >> 16) & 0xFF),
                B = (byte)((value >>  8) & 0xFF),
                A = (byte)((value      ) & 0xFF)
            };
        }

        public uint ToUint()
        {
            return (uint)((R << 24) | (G << 16) | (B << 8) | A);
        }
    }
}