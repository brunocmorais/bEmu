using bEmu.Core;

namespace bEmu.Core.CPU.Z80
{
    public class Opcode : Core.CPU.Opcode<byte>
    {
        public Opcode(byte byte1) : base(byte1) { }
    }
}