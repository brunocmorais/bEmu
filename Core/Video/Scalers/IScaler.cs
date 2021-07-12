using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public interface IScaler
    {
        int ScaleFactor { get; set; }
        Framebuffer Framebuffer { get; set; }
        Framebuffer ScaledFramebuffer { get; }
        int Frame { get; set; }
        void Update(int frame);
    }
}