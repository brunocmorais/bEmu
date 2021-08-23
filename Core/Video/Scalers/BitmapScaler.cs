using bEmu.Core.Enums;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Video
{
    public static class BitmapScaler
    {
        public static Bitmap Scale(string fileName, ScalerType scalerType, int passes)
        {
            var bitmap = BitmapReader.Instance.Read(FileManager.Read(fileName));

            for (int i = 0; i < passes; i++)
            {
                var frameBuffer = FrameBuffer.From(bitmap);
                var scaler = ScalerFactory.Get(scalerType, 2, frameBuffer);
                bitmap = Scale(scaler);
            }

            return bitmap;
        }

        private static Bitmap Scale(IScaler scaler)
        {
            scaler.Update(0);
            return Bitmap.From(scaler.ScaledFramebuffer);
        }
    }
}