using System.Collections.Generic;
using System.IO;
using bEmu.Core;
using bEmu.Core.Mappers;
using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{

    public class MBC1 : Mapper, IRAM, IMBC
    {
        private int mode;
        private int bank1;
        private int bank2;
        private byte ramg;
        protected override byte[] CartRam 
        {
            get
            {
                if (RamBanks == null)
                    return null;

                if (mode == 0)
                    return RamBanks[0];
                else
                    return RamBanks[bank2];
            }   
        }

        protected override int ExternalRamSize => 8192;
        protected override int RamBankCount => 4;


        public MBC1(IMMU mmu, bool ram, bool battery) : base(mmu, battery, ram)
        {
            bank1 = 1;
            bank2 = 0;
            mode = 0;
        }

        public void SetMode(int addr, byte value)
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
                    return Rom0[addr];
                else
                    return RomBanks[(bank2 << 5) % RomBanks.Length][addr];
            }
            else if (addr >= 0x4000 && addr <= 0x7FFF)
            {
                if (RomBanks.Length >= 16)
                    return RomBanks[((bank2 << 5) | bank1) % RomBanks.Length][addr - 0x4000];

                return RomBanks[bank1 % RomBanks.Length][addr - 0x4000];
            }

            return 0xFF;
        }

        public void WriteCartRAM(int addr, byte value)
        {
            if (CartRam != null && (ramg & 0x0F) == 0x0A)
                CartRam[addr] = value;
        }

        public byte ReadCartRAM(int addr)
        {
            if (CartRam != null && (ramg & 0x0F) == 0x0A)
                return CartRam[addr];

            return 0xFF;
        }
    }
}