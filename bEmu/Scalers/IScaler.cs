using bEmu.Core;

namespace bEmu.Scalers
{
    public interface IScaler
    {
        int ScaleFactor { get; set; }
        Framebuffer Original { get; }
        Framebuffer Scaled { get; }
        void Update();
    }
}