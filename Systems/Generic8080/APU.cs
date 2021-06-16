using bEmu.Core;

namespace bEmu.Systems.Generic8080
{
    public class APU : Core.APU
    {
        private readonly byte[] buffer;
        public override int BufferSize => 512;
        public override int SampleRate => 22050;

        public APU(ISystem system) : base(system)
        {
            buffer = new byte[BufferSize];
        }

        public override byte[] UpdateBuffer()
        {
            return buffer;
        }

        public override void Update(int cycles)
        {
            
        }
    }
}