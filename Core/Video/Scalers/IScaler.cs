using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public interface IScaler
    {
        int ScaleFactor { get; }
        IFrameBuffer Framebuffer { get; }
        IFrameBuffer ScaledFramebuffer { get; }
        int Frame { get; set; }
        void Update(int frame);
        void Update(int x, int y);
    }
}