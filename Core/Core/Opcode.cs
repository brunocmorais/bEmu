using bEmu.Core.Util;

namespace bEmu.Core
{
    public abstract class Opcode : IOpcode
    {
        private byte byte1;
        private byte byte2;

        public Opcode(byte opcode)
        {
            byte1 = opcode;
        }

        public Opcode(byte byte1, byte byte2)
        {
            this.byte1 = byte1;
            this.byte2 = byte2;
        }

        public Opcode(ushort opcode)
        {
            byte1 = (byte) ((opcode >> 8) & 0xFF);
            byte2 = (byte) (opcode & 0xFF);
        }

        public byte Byte { get { return byte1; } }

        public ushort UShort { get { return BitUtils.GetWordFrom2Bytes(byte2, byte1); } }
    }
}