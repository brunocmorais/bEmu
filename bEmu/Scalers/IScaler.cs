using bEmu.Core;

namespace bEmu.Scalers
{
    public interface IScaler
    {
        int ScaleFactor { get; set; }
        Framebuffer Framebuffer { get; set; }
        Framebuffer ScaledFramebuffer { get; }
        void Update();
    }
}