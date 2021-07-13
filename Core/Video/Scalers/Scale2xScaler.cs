
namespace bEmu.Core.Video.Scalers
{
    public class Scale2xScaler : BaseScaler
    {
        public Scale2xScaler() : base(2) { }

        public override void Update(int x, int y)
        {
            var pixel2x = new ScaledPixel(2);
            uint pixel = this[x, y];
                    
            pixel2x[0, 0] = pixel2x[1, 0] = pixel2x[0, 1] = pixel2x[1, 1] = pixel;


            var adj = new NeighborPixels()
            { 
                A = this[x - 1, y - 1], B = this[x, y - 1], C = this[x + 1, y - 1],
                D = this[x - 1, y + 0], E = this[x, y + 0], F = this[x + 1, y + 0],
                G = this[x - 1, y + 1], H = this[x, y + 1], I = this[x + 1, y + 1],
            };

            if (adj.B != adj.H && adj.D != adj.F) 
            {
                pixel2x[0, 0] = adj.D == adj.B ? adj.D : adj.E;
                pixel2x[1, 0] = adj.B == adj.F ? adj.F : adj.E;
                pixel2x[0, 1] = adj.D == adj.H ? adj.D : adj.E;
                pixel2x[1, 1] = adj.H == adj.F ? adj.F : adj.E;
            }
            
            ScaledFramebuffer.SetScaledPixel(pixel2x, x, y);
        }

        struct NeighborPixels
        {
            public uint A, B, C, D, E, F, G, H, I;
        }
    }
}