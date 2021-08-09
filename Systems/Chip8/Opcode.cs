using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class Opcode : Core.CPU.Opcode<ushort>
    {
        public Opcode(ushort value) : base(value) { }

        public ushort Nnn
        {
            get { return (ushort) (Value & 0x0FFF); }
        }

        public byte Kk
        {
            get { return (byte) (Value & 0x00FF); }
        }

        public byte X
        {
            get { return (byte) ((Value & 0x0F00) >> 8); }
        }

        public byte Y
        {
            get { return (byte) ((Value & 0x00F0) >> 4); }
        }

        public byte Nibble
        {
            get { return (byte) ((Value & 0x000F)); }
        }
    }
}