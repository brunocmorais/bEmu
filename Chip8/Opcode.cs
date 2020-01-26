namespace bEmu.Chip8
{
    public struct Opcode
    {
        private ushort opcode;

        public Opcode(ushort opcode)
        {
            this.opcode = opcode;
        }

        public ushort Nnn
        {
            get { return (ushort) (opcode & 0x0FFF); }
        }

        public byte Kk
        {
            get { return (byte) (opcode & 0x00FF); }
        }

        public byte X
        {
            get { return (byte) ((opcode & 0x0F00) >> 8); }
        }

        public byte Y
        {
            get { return (byte) ((opcode & 0x00F0) >> 4); }
        }

        public byte Nibble
        {
            get { return (byte) ((opcode & 0x000F)); }
        }

        public ushort Value
        {
            get {return opcode; }
        }
    }
}