using bEmu.Core;

namespace bEmu.Core.Video.Scalers
{
    public class NearestNeighborScaler : Scaler
    {
        public NearestNeighborScaler(int scaleFactor) : base(scaleFactor) { }

        public override void Update(int x, int y)
        {
            var scaledPixel = new ScaledPixel(ScaleFactor);
            scaledPixel.Fill(this[x, y]);

            ScaledFramebuffer.SetScaledPixel(scaledPixel, x, y);
        }
    }
}