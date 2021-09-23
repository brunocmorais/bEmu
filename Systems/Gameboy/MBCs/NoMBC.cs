using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{
    public class NoMBC : Mapper
    {
        protected override byte[] CartRam => RamBanks != null ? RamBanks[0] : null;

        protected override int ExternalRamSize => 8192;

        protected override int RamBankCount => 1;

        public NoMBC(IMMU mmu, bool ram, bool battery) : base(16384, mmu, battery, ram) { }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return Rom0[addr];
            else
                return RomBanks[1][addr - 0x4000];
        }
    }
}