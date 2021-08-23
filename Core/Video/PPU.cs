using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Video
{

    public abstract class PPU : IPPU
    {
        public IVideoSystem System { get; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IFrameBuffer Framebuffer { get; protected set; }
        public int Frameskip { get; set; }
        public int Frame { get; set; }
        public int Cycles { get; set; }

        public PPU(IVideoSystem system, int width, int height)
        {
            System = system;
            DefineSize(width, height);
        }

        public void DefineSize(int width, int height)
        {
            Width = width;
            Height = height;

            if (Framebuffer == null)
                Framebuffer = new FrameBuffer(width, height);
            else
                Framebuffer.SetSize(width, height);
        }
        
        public abstract void StepCycle();

        public virtual void Reset()
        {
            Frame = 0;
            Cycles = 0;
            Framebuffer.Reset();
        }
    }
}