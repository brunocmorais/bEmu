using System.Linq;
using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class EPXScaler : BaseScaler
    {
        public EPXScaler() : base(2) { }

        public override void Update()
        {
            int xScale = 0;
            int yScale = 0;
            var pixel2x = new Pixel2x();

            for (int x = 0; x < Framebuffer.Width; x++)
            {
                xScale = 0;

                for (int y = 0; y < Framebuffer.Height; y++)
                {
                    var pixel = this[x, y];
                    
                    var adj = new NeighborPixels()
                    { 
                                            A = this[x, y - 1],
                        C = this[x - 1, y], P = this[  x, y  ], B = this[x + 1, y],
                                            D = this[x, y + 1],
                    };

                    pixel2x.P1 = pixel2x.P2 = pixel2x.P3 = pixel2x.P4 = pixel;

                    if (adj.C == adj.A && adj.C != adj.D && adj.A != adj.B) pixel2x.P1 = adj.A;
                    if (adj.A == adj.B && adj.A != adj.C && adj.B != adj.D) pixel2x.P2 = adj.B;
                    if (adj.D == adj.C && adj.D != adj.B && adj.C != adj.A) pixel2x.P3 = adj.C;
                    if (adj.B == adj.D && adj.B != adj.A && adj.D != adj.C) pixel2x.P4 = adj.D;

                    ScaledFramebuffer.SetPixel2x(pixel2x, xScale, yScale);

                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            }
        }

        struct NeighborPixels
        {
            public uint A, 
                     C, P, B,
                        D;
        }
    }
}