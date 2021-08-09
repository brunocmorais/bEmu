using bEmu.Core;

namespace bEmu.Core.CPU.Intel8080
{
    public class Opcode : Core.CPU.Opcode<byte>
    {
        public Opcode(byte byte1) : base(byte1) { }
    }
}