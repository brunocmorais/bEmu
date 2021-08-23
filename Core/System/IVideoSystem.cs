using bEmu.Core.Video;

namespace bEmu.Core.System
{
    public interface IVideoSystem : IRunnableSystem
    {
        IPPU PPU { get; }
        int Width { get; }
        int Height { get; }
        int Frame { get; }
        int Frameskip { get; set; }
        IFrameBuffer Framebuffer { get; }
        bool SkipFrame { get; }
    }
}