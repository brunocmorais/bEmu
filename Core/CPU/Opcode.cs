using bEmu.Core.Util;

namespace bEmu.Core.CPU
{
    public abstract class Opcode<T> : Number<T>, IOpcode where T : struct
    {
        public Opcode(T value) : base(value) { }

        public int CyclesTaken { get; set; }
    }
}