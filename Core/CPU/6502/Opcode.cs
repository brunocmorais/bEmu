using bEmu.Core;

namespace bEmu.Core.CPU.MOS6502
{
    public class Opcode : Core.CPU.Opcode<byte>
    {
        public Opcode(byte byte1) : base(byte1) { }

        public byte Lo => (byte)(Value & 0xF);
        public byte Hi => (byte)((Value & 0xF0) >> 4);
    }
}