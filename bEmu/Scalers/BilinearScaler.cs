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
                    var c00 = Framebuffer[gxi, gyi];
                    var c10 = Framebuffer[gxi + 1, gyi];
                    var c01 = Framebuffer[gxi, gyi + 1];
                    var c11 = Framebuffer[gxi + 1, gyi + 1];
                    float tx = gx - gxi;
                    float ty = gy - gyi;
                    var r = (uint) ScalerHelpers.Blerp((c00 >> 24) & 0xFF, (c10 >> 24) & 0xFF, (c01 >> 24) & 0xFF, (c11 >> 24) & 0xFF, tx, ty);
                    var g = (uint) ScalerHelpers.Blerp((c00 >> 16) & 0xFF, (c10 >> 16) & 0xFF, (c01 >> 16) & 0xFF, (c11 >> 16) & 0xFF, tx, ty);
                    var b = (uint) ScalerHelpers.Blerp((c00 >>  8) & 0xFF, (c10 >>  8) & 0xFF, (c01 >>  8) & 0xFF, (c11 >>  8) & 0xFF, tx, ty);

                    ScaledFramebuffer[x, y] = (r << 24) | (g << 16) | (b << 8) | 0xFF;
                }
            }
        }
    }
}