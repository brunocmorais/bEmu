namespace bEmu.Systems.NES
{
    public abstract class DefaultMapper : IMapper
    {
        protected byte[][] RomBanks;
        protected byte[][] RamBanks;
        protected byte[] Rom0 => RomBanks[0];
        protected abstract int RomBankSize { get; }

        public void LoadProgram(byte[] bytes)
        {
            RomBanks = new byte[bytes.Length / RomBankSize][];

            for (int i = 0; i < RomBanks.Length; i++)
                RomBanks[i] = new byte[RomBankSize];

            for (int i = 0; i < bytes.Length; i++)
                RomBanks[i / RomBankSize][i % RomBankSize] = bytes[i];
        }

        public abstract byte Read(ushort address);
    }
}