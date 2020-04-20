using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public class MBC5 : DefaultMBC
    {
        protected override byte[] cartRAM => ramBanks != null ? ramBanks[ramb] : null;
        byte romb0;
        byte romb1;
        byte ramb;
        byte ramg;

        public MBC5(string fileName, bool ram, bool battery) : base(fileName, battery)
        {
            romb0 = 1;
            romb1 = 0;
            ramb = 0;

            if (ram)
                InitializeRAMBanks(16, 8192);
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
                return rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return romBanks[(((romb1 << 8) | romb0)) % romBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public override void WriteCartRAM(int addr, byte value)
        {
            if (ramg == 0x0A && cartRAM != null)
                cartRAM[addr] = value;
        }

        public override byte ReadCartRAM(int addr)
        {
            if (ramg == 0x0A && cartRAM != null)
                return cartRAM[addr];

            return 0xFF;
        }
    }
}