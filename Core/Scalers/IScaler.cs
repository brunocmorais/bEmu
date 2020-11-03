using bEmu.Core;

namespace bEmu.Core.Scalers
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