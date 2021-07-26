using bEmu.Core;

namespace bEmu.Core.Video.Scalers
{
    public class NearestNeighborScaler : Scaler
    {
        public NearestNeighborScaler(IFrameBuffer frameBuffer, int scaleFactor) : base(frameBuffer, scaleFactor) { }

        public override void Update(int x, int y)
        {
            ScaledFramebuffer.SetScaledPixel(new ScaledPixel(ScaleFactor, this[x, y]), x, y);
        }
    }
}