using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class Scale3xScaler : BaseScaler
    {
        struct NeighborPixels
        {
            public uint A, B, C, D, E, F, G, H, I;
        }

        public Scale3xScaler() : base(3) { }

        public override void Update(int x, int y)
        {
            var pixel3x = new ScaledPixel(3);
            uint pixel = this[x, y];
                    
            pixel3x[0, 0] = pixel3x[1, 0] = pixel3x[2, 0] = 
            pixel3x[0, 1] = pixel3x[1, 1] = pixel3x[2, 1] =
            pixel3x[0, 2] = pixel3x[1, 2] = pixel3x[2, 2] = pixel;

            var adj = new NeighborPixels()
            { 
                A = this[x - 1, y - 1], B = this[x, y - 1], C = this[x + 1, y - 1],
                D = this[x - 1, y + 0], E = this[x, y + 0], F = this[x + 1, y + 0],
                G = this[x - 1, y + 1], H = this[x, y + 1], I = this[x + 1, y + 1],
            };

            if (adj.D == adj.B && adj.D != adj.H && adj.B != adj.F) 
                pixel3x[0, 0] = adj.D;
            if ((adj.D == adj.B && adj.D != adj.H && adj.B != adj.F && adj.E != adj.C) || (adj.B == adj.F && adj.B != adj.D && adj.F != adj.H && adj.E != adj.A)) 
                pixel3x[1, 0] = adj.B;
            if (adj.B == adj.F && adj.B != adj.D && adj.F != adj.H) 
                pixel3x[2, 0] = adj.F;
            if ((adj.H == adj.D && adj.H != adj.F && adj.D != adj.B && adj.E != adj.A) || (adj.D == adj.B && adj.D != adj.H && adj.B != adj.F && adj.E != adj.G)) 
                pixel3x[0, 1] = adj.D;

            pixel3x[1, 1] = pixel;

            if ((adj.B == adj.F && adj.B != adj.D && adj.F != adj.H && adj.E != adj.I) || (adj.F == adj.H && adj.F != adj.B && adj.H != adj.D && adj.E != adj.C)) 
                pixel3x[2, 1] = adj.F;
            if (adj.H == adj.D && adj.H != adj.F && adj.D != adj.B) 
                pixel3x[0, 2] = adj.D;
            if ((adj.F == adj.H && adj.F != adj.B && adj.H != adj.D && adj.E != adj.G) || (adj.H == adj.D && adj.H != adj.F && adj.D != adj.B && adj.E != adj.I)) 
                pixel3x[1, 2] = adj.H;
            if (adj.F == adj.H && adj.F != adj.B && adj.H != adj.D) 
                pixel3x[2, 2] = adj.F;
            
            ScaledFramebuffer.SetScaledPixel(pixel3x, x, y);
        }
    }
}