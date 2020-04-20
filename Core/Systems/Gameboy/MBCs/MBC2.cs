using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{

    public class MBC2 : DefaultMBC
    {
        int romb;
        byte ramg;
        protected override byte[] cartRAM => ramBanks[0];

        public MBC2(string fileName, bool battery) : base(fileName, battery)
        {
            romb = 1;
            InitializeRAMBanks(1, 512);
        }

        public override void SetMode(int addr, byte value)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
            {
                if ((addr & 0x100) == 0x100)
                {
                    romb = value & 0xF;

                    if (romb == 0)
                        romb++;
                }
                else
                    ramg = value;
            }
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return romBanks[romb % romBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public override void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
                cartRAM[addr] = (byte) (value & 0xF);
        }

        public override byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
                return (byte) (cartRAM[addr % cartRAM.Length] | 0xF0);

            return 0xFF;
        }
    }
}