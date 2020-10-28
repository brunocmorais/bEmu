using System.Collections.Generic;
using System.IO;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.MBCs
{
    public class MBC5 : DefaultMBC
    {
        private byte romb0;
        private byte romb1;
        private byte ramb;
        private byte ramg;
        protected override byte[] CartRam => RamBanks != null ? RamBanks[ramb] : null;
        protected override int ExternalRamSize => 8192;
        protected override int RamBankCount => 16;

        public MBC5(IMMU mmu, bool ram, bool battery) : base(mmu, battery, ram)
        {
            romb0 = 1;
            romb1 = 0;
            ramb = 0;
        }

        public override void SetMode(int addr, byte value)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF)
                ramg = value;
            else if (addr >= 0x2000 && addr <= 0x2FFF)
                romb0 = value;
            else if (addr >= 0x3000 && addr <= 0x3FFF)
                romb1 = (byte) (value & 0x1);
            else if (addr >= 0x4000 && addr <= 0x5FFF)
                ramb = (byte) (value & 0xF);
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return Rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return RomBanks[BankNumber][addr - 0x4000];

            return 0xFF;
        }

        public int BankNumber => (((romb1 << 8) | romb0)) % RomBanks.Length;

        public override void WriteCartRAM(int addr, byte value)
        {
            if (ramg == 0x0A && CartRam != null)
                CartRam[addr] = value;
        }

        public override byte ReadCartRAM(int addr)
        {
            if (ramg == 0x0A && CartRam != null)
                return CartRam[addr];

            return 0xFF;
        }
    }
}