namespace bEmu.Systems.NES
{
    public class NROM : IMapper
    {
        private byte[] prg0 = new byte[0x4000];
        private byte[] prg1 = new byte[0x4000];


        public byte Read(ushort address)
        {
            if (address >= 0x8000 && address <= 0xBFFF)
                return prg0[address - 0x8000];
            else if (address >= 0xC000 && address <= 0xFFFF)
                return prg1[address - 0xC000];
            else
                return 0;
        }

        public void LoadProgram(byte[] bytes)
        {
            
        }
    }
}