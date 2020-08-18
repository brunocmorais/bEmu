using System.Collections.Generic;
using System.IO;

namespace bEmu.Systems.Gameboy.MBCs
{

    public class MBC2 : DefaultMBC
    {
        private int romb;
        private byte ramg;
        protected override byte[] CartRam => RamBanks[0];

        protected override int ExternalRamSize => 512;

        protected override int RamBankCount => 1;

        public MBC2(string fileName, bool battery) : base(fileName, battery, true)
        {
            romb = 1;
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
                return Rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return RomBanks[romb % RomBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public override void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
                CartRam[addr] = (byte) (value & 0xF);
        }

        public override byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
                return (byte) (CartRam[addr % CartRam.Length] | 0xF0);

            return 0xFF;
        }
    }
}