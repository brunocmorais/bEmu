using bEmu.Core;

namespace bEmu.Scalers
{
    public class BilinearScaler : BaseScaler
    {
        public BilinearScaler(int scaleFactor) : base(scaleFactor) { }

        public override void Update()
        {
            int xScale = Framebuffer.Width * ScaleFactor;
            int yScale = Framebuffer.Height * ScaleFactor;
 
            for (int x = 0; x < xScale; x++) 
            {
                for (int y = 0; y < yScale; y++) 
                {
                    float gx = ((float) x) / xScale * (Framebuffer.Width - 1);
                    float gy = ((float) y) / yScale * (Framebuffer.Height - 1);
                    int gxi = (int) gx;
                    int gyi = (int) gy;
                    var c00 = Pixel.FromUint(Framebuffer[gxi, gyi]);
                    var c10 = Pixel.FromUint(Framebuffer[gxi + 1, gyi]);
                    var c01 = Pixel.FromUint(Framebuffer[gxi, gyi + 1]);
                    var c11 = Pixel.FromUint(Framebuffer[gxi + 1, gyi + 1]);
                    byte r = (byte) Blerp(c00.R, c10.R, c01.R, c11.R, gx - gxi, gy - gyi);
                    byte g = (byte) Blerp(c00.G, c10.G, c01.G, c11.G, gx - gxi, gy - gyi);
                    byte b = (byte) Blerp(c00.B, c10.B, c01.B, c11.B, gx - gxi, gy - gyi);

                    ScaledFramebuffer[x, y] = new Pixel(r, g, b).ToUint();
                }
            }
        }

        private static float Lerp(float s, float e, float t) 
        {
            return s + (e - s) * t;
        }
 
        private static float Blerp(float c00, float c10, float c01, float c11, float tx, float ty) 
        {
            return Lerp(Lerp(c00, c10, tx), Lerp(c01, c11, tx), ty);
        }
    }
}