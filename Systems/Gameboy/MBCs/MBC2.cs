using System.Collections.Generic;
using System.IO;
using bEmu.Core;
using bEmu.Core.Mappers;
using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{

    public class MBC2 : Mapper, IRAM, IMBC
    {
        private int romb;
        private byte ramg;
        protected override byte[] CartRam => RamBanks[0];

        protected override int ExternalRamSize => 512;

        protected override int RamBankCount => 1;

        public MBC2(IMMU mmu, bool battery) : base(mmu, battery, true)
        {
            romb = 1;
        }

        public void SetMode(int addr, byte value)
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
                return Rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return RomBanks[romb % RomBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
                CartRam[addr] = (byte) (value & 0xF);
        }

        public byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
                return (byte) (CartRam[addr % CartRam.Length] | 0xF0);

            return 0xFF;
        }
    }
}