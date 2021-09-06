using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Video
{
    public interface IPPU
    {
        IVideoSystem System { get; }
        int Width { get; }
        int Height { get; }
        IFrameBuffer Framebuffer { get; }
        int Frameskip { get; set; }
        int Frame { get; }
        int Cycles { get; set; }

        void StepCycle();
        void DefineSize(int width, int height);
        void Reset();
        void IncreaseFrame();
    }
}