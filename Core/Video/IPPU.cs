using bEmu.Core.System;

namespace bEmu.Core.Video
{
    public interface IPPU
    {
        ISystem System { get; }
        int Width { get; }
        int Height { get; }
        Framebuffer Framebuffer { get; }
        int Frameskip { get; set; }
        int Frame { get; set; }
        int Cycles { get; set; }
        void StepCycle();
        void DefineSize(int width, int height);
        void Reset();
    }
}