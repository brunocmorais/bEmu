using bEmu.Core;

namespace bEmu.Systems.Empty
{
    public class PPU : Core.PPU
    {
        public PPU(ISystem system, int width, int height) : base(system, width, height)
        {
        }

        public override void StepCycle()
        {
            
        }
    }
}