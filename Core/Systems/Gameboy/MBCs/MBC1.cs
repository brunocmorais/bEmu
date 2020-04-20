using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{

    public class MBC1 : DefaultMBC
    {
        int mode;
        int bank1;
        int bank2;
        protected override byte[] cartRAM 
        {
            get
            {
                if (ramBanks == null)
                    return null;

                if (mode == 0)
                    return ramBanks[0];
                else
                    return ramBanks[bank2];
            }   
        }

        protected override int externalRAMSize => 8192;
        protected override int ramBankCount => 4;

        byte ramg;

        public MBC1(string fileName, bool ram, bool battery) : base(fileName, battery, ram)
        {
            bank1 = 1;
            bank2 = 0;
            mode = 0;
        }

        public override void SetMode(int addr, byte value)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF)
                ramg = value;
            if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                if ((value & 0x1F) == 0x00)
                    bank1 = 0x01;
                else
                    bank1 = value & 0x1F;
            }
            if (addr >= 0x4000 && addr <= 0x5FFF)
                bank2 = value & 0x3;
            if (addr >= 0x6000 && addr <= 0x7FFF)
                mode = value & 0x1;
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
            {
                if (mode == 0)
                    return rom0[addr];
                else
                    return romBanks[(bank2 << 5) % romBanks.Length][addr];
            }
            else if (addr >= 0x4000 && addr <= 0x7FFF)
            {
                if (romBanks.Length >= 16)
                    return romBanks[((bank2 << 5) | bank1) % romBanks.Length][addr - 0x4000];

                return romBanks[bank1 % romBanks.Length][addr - 0x4000];
            }

            return 0xFF;
        }

        public override void WriteCartRAM(int addr, byte value)
        {
            if (cartRAM != null && (ramg & 0x0F) == 0x0A)
                cartRAM[addr] = value;
        }

        public override byte ReadCartRAM(int addr)
        {
            if (cartRAM != null && (ramg & 0x0F) == 0x0A)
                return cartRAM[addr];

            return 0xFF;
        }
    }
}