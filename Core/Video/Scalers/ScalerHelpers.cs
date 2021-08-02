namespace bEmu.Core.Video.Scalers
{
    public static class ScalerHelpers
    {
        public const uint ColorMask = 0xFEFEFEFF;
        public const uint LowPixelMask = 0x010101FF;
        public const uint QcolorMask = 0xFCFCFCFF;
        public const uint QlowpixelMask = 0x030303FF;

        public static int GetResult1(Pixel a, Pixel b, Pixel c, Pixel d)
        {
            int x = 0; 
            int y = 0;
            int r = 0;

            if (a == c) x++; 
            else if (b == c) y++;
            
            if (a == d) x++; 
            else if (b == d) y++;

            if (x <= 1) r++; 
            if (y <= 1) r--;

            return r;
        }

        public static int GetResult2(Pixel a, Pixel b, Pixel c, Pixel d)
        {
            int x = 0; 
            int y = 0;
            int r = 0;

            if (a == c) x++; 
            else if (b == c) y++;
            
            if (a == d) x++; 
            else if (b == d) y++;
            
            if (x <= 1) r--; 
            if (y <= 1) r++;
            
            return r;
        }

        public static Pixel Interpolate(Pixel a, Pixel b)
        {
            if (a != b)
                return new Pixel((((a & ColorMask) >> 1) + ((b & ColorMask) >> 1) + (a & b & LowPixelMask)));

            return a;
        }

        public static Pixel QInterpolate(Pixel a, Pixel b, Pixel c, Pixel d)
        {
            uint x = ((a & QcolorMask) >> 2) +
                ((b & QcolorMask) >> 2) +
                ((c & QcolorMask) >> 2) +
                ((d & QcolorMask) >> 2);
            uint y = (a & QlowpixelMask) +
                (b & QlowpixelMask) +
                (c & QlowpixelMask) +
                (d & QlowpixelMask);
            y = (y >> 2) & QlowpixelMask;
            return new Pixel(x + y);
        }

        public static float Lerp(float s, float e, float t) => s + (e - s) * t; 
        public static float Blerp(float c00, float c10, float c01, float c11, float tx, float ty) => 
            Lerp(Lerp(c00, c10, tx), Lerp(c01, c11, tx), ty);
    }
}