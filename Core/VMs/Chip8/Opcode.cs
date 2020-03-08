using bEmu.Core;

namespace bEmu.Core.VMs.Chip8
{
    public class Opcode : Core.Opcode
    {
        public Opcode(byte byte1, byte byte2) : base(byte1, byte2) { }

        public ushort Nnn
        {
            get { return (ushort) (UShort & 0x0FFF); }
        }

        public byte Kk
        {
            get { return (byte) (UShort & 0x00FF); }
        }

        public byte X
        {
            get { return (byte) ((UShort & 0x0F00) >> 8); }
        }

        public byte Y
        {
            get { return (byte) ((UShort & 0x00F0) >> 4); }
        }

        public byte Nibble
        {
            get { return (byte) ((UShort & 0x000F)); }
        }
    }
}