using bEmu.Core;

namespace bEmu.Scalers
{
    public class Scale2xScaler : BaseScaler
    {

        public Scale2xScaler() : base(2) { }

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
                    uint pixel = this[x, y];
                    
                    pixel2x.P1 = pixel2x.P2 = pixel2x.P3 = pixel2x.P4 = pixel;

                    var adj = new NeighborPixels()
                    { 
                        A = this[x - 1, y - 1], B = this[x, y - 1], C = this[x + 1, y - 1],
                        D = this[x - 1, y + 0], E = this[x, y + 0], F = this[x + 1, y + 0],
                        G = this[x - 1, y + 1], H = this[x, y + 1], I = this[x + 1, y + 1],
                    };

                    if (adj.B != adj.H && adj.D != adj.F) 
                    {
                        pixel2x.P1 = adj.D == adj.B ? adj.D : adj.E;
                        pixel2x.P2 = adj.B == adj.F ? adj.F : adj.E;
                        pixel2x.P3 = adj.D == adj.H ? adj.D : adj.E;
                        pixel2x.P4 = adj.H == adj.F ? adj.F : adj.E;
                    }
                    
                    ScaledFramebuffer.SetPixel2x(pixel2x, xScale, yScale);

                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            }
        }

        struct NeighborPixels
        {
            public uint A, B, C, D, E, F, G, H, I;
        }
    }
}