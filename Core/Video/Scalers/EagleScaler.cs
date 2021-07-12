using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class EagleScaler : BaseScaler
    {
        public EagleScaler() : base(2) { }

        public override void Update(int frame)
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
                    
                    var adj = new NeighborPixels()
                    { 
                        S = this[x - 1, y - 1], T = this[x, y - 1], U = this[x + 1, y - 1],
                        V = this[x - 1, y + 0], C = this[x, y + 0], W = this[x + 1, y + 0],
                        X = this[x - 1, y + 1], Y = this[x, y + 1], Z = this[x + 1, y + 1],
                    };

                    pixel2x.P1 = (adj.V == adj.S && adj.S == adj.T && adj.S != 0) ? adj.S : adj.C;
                    pixel2x.P2 = (adj.T == adj.U && adj.U == adj.W && adj.U != 0) ? adj.U : adj.C;
                    pixel2x.P3 = (adj.V == adj.X && adj.X == adj.Y && adj.X != 0) ? adj.X : adj.C;
                    pixel2x.P4 = (adj.W == adj.Z && adj.Z == adj.Y && adj.Z != 0) ? adj.Z : adj.C;

                    ScaledFramebuffer.SetPixel2x(pixel2x, xScale, yScale);

                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            }

            base.Update(frame);
        }

        struct NeighborPixels
        {
            public uint S, T, U, V, C, W, X, Y, Z;
        }
    }
}