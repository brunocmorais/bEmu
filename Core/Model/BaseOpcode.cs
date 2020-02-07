using bEmu.Core.Util;

namespace bEmu.Core.Model
{
    public abstract class BaseOpcode : IOpcode
    {
        private byte byte1;
        private byte byte2;

        public BaseOpcode(byte opcode)
        {
            byte1 = opcode;
        }

        public BaseOpcode(byte byte1, byte byte2)
        {
            this.byte1 = byte1;
            this.byte2 = byte2;
        }

        public BaseOpcode(ushort opcode)
        {
            byte1 = (byte) ((opcode >> 8) & 0xFF);
            byte2 = (byte) (opcode & 0xFF);
        }

        public byte Byte { get { return byte1; } }

        public ushort UShort { get { return GeneralUtils.Get16BitNumber(byte2, byte1); } }
    }
}