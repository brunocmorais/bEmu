using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class NearestNeighborScaler : BaseScaler
    {
        public NearestNeighborScaler(int scaleFactor) : base(scaleFactor) { }

        public override void Update()
        {
            int xScale = 0;
            int yScale = 0;
            
            for (int x = 0; x < Framebuffer.Width; x++)
            {
                xScale = 0;

                for (int y = 0; y < Framebuffer.Height; y++)
                {
                    var pixel = this[x, y];
                    
                    for (int i = 0; i < ScaleFactor; i++)
                        for (int j = 0; j < ScaleFactor; j++)
                            ScaledFramebuffer[i + yScale, j + xScale] = pixel;
                    
                    xScale += ScaleFactor;
                }

                yScale += ScaleFactor;
            }
        }
    }
}