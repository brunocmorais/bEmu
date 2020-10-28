using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class Scale3xScaler : BaseScaler
    {
        struct NeighborPixels
        {
            public uint A, B, C, D, E, F, G, H, I;
        }

        public Scale3xScaler() : base(3) { }

        public override void Update()
        {
            int xScale = 0;
            int yScale = 0;
            var pixel3x = new Pixel3x();

            for (int x = 0; x < Framebuffer.Width; x++)
            {
                xScale = 0;

                for (int y = 0; y < Framebuffer.Height; y++)
                {
                    uint pixel = this[x, y];
                    
                    pixel3x.P1 = pixel3x.P2 = pixel3x.P3 = 
                    pixel3x.P4 = pixel3x.P5 = pixel3x.P6 =
                    pixel3x.P7 = pixel3x.P8 = pixel3x.P9 = pixel;

                    var adj = new NeighborPixels()
                    { 
                        A = this[x - 1, y - 1], B = this[x, y - 1], C = this[x + 1, y - 1],
                        D = this[x - 1, y + 0], E = this[x, y + 0], F = this[x + 1, y + 0],
                        G = this[x - 1, y + 1], H = this[x, y + 1], I = this[x + 1, y + 1],
                    };

                    if (adj.D == adj.B && adj.D != adj.H && adj.B != adj.F) 
                        pixel3x.P1 = adj.D;
                    if ((adj.D == adj.B && adj.D != adj.H && adj.B != adj.F && adj.E != adj.C) || (adj.B == adj.F && adj.B != adj.D && adj.F != adj.H && adj.E != adj.A)) 
                        pixel3x.P2 = adj.B;
                    if (adj.B == adj.F && adj.B != adj.D && adj.F != adj.H) 
                        pixel3x.P3 = adj.F;
                    if ((adj.H == adj.D && adj.H != adj.F && adj.D != adj.B && adj.E != adj.A) || (adj.D == adj.B && adj.D != adj.H && adj.B != adj.F && adj.E != adj.G)) 
                        pixel3x.P4 = adj.D;
                    pixel3x.P5 = pixel;
                    if ((adj.B == adj.F && adj.B != adj.D && adj.F != adj.H && adj.E != adj.I) || (adj.F == adj.H && adj.F != adj.B && adj.H != adj.D && adj.E != adj.C)) 
                        pixel3x.P6 = adj.F;
                    if (adj.H == adj.D && adj.H != adj.F && adj.D != adj.B) 
                        pixel3x.P7 = adj.D;
                    if ((adj.F == adj.H && adj.F != adj.B && adj.H != adj.D && adj.E != adj.G) || (adj.H == adj.D && adj.H != adj.F && adj.D != adj.B && adj.E != adj.I)) 
                        pixel3x.P8 = adj.H;
                    if (adj.F == adj.H && adj.F != adj.B && adj.H != adj.D) 
                        pixel3x.P9 = adj.F;
                    
                    ScaledFramebuffer.SetPixel3x(pixel3x, xScale, yScale);

                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            }
        }
    }
}