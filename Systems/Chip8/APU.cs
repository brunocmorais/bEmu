using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class APU : Core.APU
    {
        public APU(ISystem system) : base(system)
        {
        }

        public override int BufferSize => 512;

        public override int SampleRate => 22050;

        public override byte[] UpdateBuffer()
        {
            return new byte[0];
        }
    }
}