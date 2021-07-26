using System.Linq;
using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class EPXScaler : Scaler
    {
        public EPXScaler(IFrameBuffer frameBuffer) : base(frameBuffer, 2) { }

        public override void Update(int x, int y)
        {
            var pixel2x = new ScaledPixel(2);
            var pixel = this[x, y];
                    
            var adj = new NeighborPixels()
            { 
                                    A = this[x, y - 1],
                C = this[x - 1, y], P = this[  x, y  ], B = this[x + 1, y],
                                    D = this[x, y + 1],
            };

            pixel2x[0, 0] = pixel2x[1, 0] = pixel2x[0, 1] = pixel2x[1, 1] = pixel;

            if (adj.C == adj.A && adj.C != adj.D && adj.A != adj.B) pixel2x[0, 0] = adj.A;
            if (adj.A == adj.B && adj.A != adj.C && adj.B != adj.D) pixel2x[1, 0] = adj.B;
            if (adj.D == adj.C && adj.D != adj.B && adj.C != adj.A) pixel2x[0, 1] = adj.C;
            if (adj.B == adj.D && adj.B != adj.A && adj.D != adj.C) pixel2x[1, 1] = adj.D;

            ScaledFramebuffer.SetScaledPixel(pixel2x, x, y);
        }

        struct NeighborPixels
        {
            public uint A, 
                     C, P, B,
                        D;
        }
    }
}