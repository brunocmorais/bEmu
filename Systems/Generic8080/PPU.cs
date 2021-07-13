using bEmu.Core;

namespace bEmu.Systems.Generic8080
{
    public class PPU : Core.Video.PPU
    {
        public const int VRAMAddress = 0x2400;

        public PPU(System system, int width, int height) : base(system, width, height) 
        { 
            Framebuffer = new DirectFrameBuffer(width, height, this.System.MMU as MMU);
        }

        public override void StepCycle() { }
    }
}