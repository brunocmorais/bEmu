namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public class MBC0 : DefaultMBC
    {
        protected override byte[] cartRAM => ramBanks != null ? ramBanks[0] : null;

        protected override int externalRAMSize => 8192;

        protected override int ramBankCount => 1;

        public MBC0(string fileName, bool ram, bool battery) : base(fileName, battery, ram) { }

        public override byte ReadCartRAM(int addr)
        {
            if (cartRAM != null)
                return cartRAM[addr];
            
            return 0xFF;
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x4000)
                return rom0[addr];
            else
                return romBanks[1][addr - 0x4000];
        }

        public override void SetMode(int addr, byte value) { }
        public override void WriteCartRAM(int addr, byte value) { }
    }
}