using bEmu.Core;

namespace bEmu.Core.Video.Scalers
{
    public class BilinearScaler : Scaler
    {
        public BilinearScaler(IFrameBuffer frameBuffer, int scaleFactor) : base(frameBuffer, scaleFactor) { }

        public override void Update(int x, int y)
        {
            int xScale = Framebuffer.Width * ScaleFactor;
            int yScale = Framebuffer.Height * ScaleFactor;

            for (int i = 0; i < ScaleFactor; i++)
            {
                for (int j = 0; j < ScaleFactor; j++)
                {        
                    float gx = ((float) i + (x * ScaleFactor)) / xScale * (Framebuffer.Width - 1);
                    float gy = ((float) j + (y * ScaleFactor)) / yScale * (Framebuffer.Height - 1);
                    int gxi = (int) gx;
                    int gyi = (int) gy;
                    var c00 = Framebuffer[gxi, gyi].ToUInt();
                    var c10 = Framebuffer[gxi + 1, gyi].ToUInt();
                    var c01 = Framebuffer[gxi, gyi + 1].ToUInt();
                    var c11 = Framebuffer[gxi + 1, gyi + 1].ToUInt();
                    float tx = gx - gxi;
                    float ty = gy - gyi;
                    var r = (uint) ScalerHelpers.Blerp((c00 >> 24) & 0xFF, (c10 >> 24) & 0xFF, (c01 >> 24) & 0xFF, (c11 >> 24) & 0xFF, tx, ty);
                    var g = (uint) ScalerHelpers.Blerp((c00 >> 16) & 0xFF, (c10 >> 16) & 0xFF, (c01 >> 16) & 0xFF, (c11 >> 16) & 0xFF, tx, ty);
                    var b = (uint) ScalerHelpers.Blerp((c00 >>  8) & 0xFF, (c10 >>  8) & 0xFF, (c01 >>  8) & 0xFF, (c11 >>  8) & 0xFF, tx, ty);

                    ScaledFramebuffer[i + (x * ScaleFactor), j + (y * ScaleFactor)] = ((r << 24) | (g << 16) | (b << 8) | 0xFF);
                }
            }
        }
    }
}