using bEmu.Core;
using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{
    public class MBC0 : DefaultMBC
    {
        protected override byte[] CartRam => RamBanks != null ? RamBanks[0] : null;

        protected override int ExternalRamSize => 8192;

        protected override int RamBankCount => 1;

        public MBC0(IMMU mmu, bool ram, bool battery) : base(mmu, battery, ram) { }

        public override byte ReadCartRAM(int addr)
        {
            if (CartRam != null)
                return CartRam[addr];
            
            return 0xFF;
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return Rom0[addr];
            else
                return RomBanks[1][addr - 0x4000];
        }

        public override void SetMode(int addr, byte value) { }
        public override void WriteCartRAM(int addr, byte value) { }
    }
}