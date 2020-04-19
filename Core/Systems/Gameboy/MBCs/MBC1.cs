using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.MBCs
{

    public class MBC1 : IMBC
    {
        int mode;
        int bank1;
        int bank2;
        IList<byte[]> romBanks;
        IList<byte[]> ramBanks;
        byte[] rom0 => romBanks[0];
        byte[] cartRAM => mode == 0 ? ramBanks[0] : ramBanks[bank2];
        byte ramg;

        public MBC1()
        {
            romBanks = new List<byte[]>();
            ramBanks = new List<byte[]>();
            bank1 = 1;
            bank2 = 0;
            mode = 0;

            for (int i = 0; i < 4; i++)
                ramBanks.Add(new byte[8192]);
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

        public byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
            {
                if (mode == 0)
                    return rom0[addr];
                else
                    return romBanks[(bank2 << 5) % romBanks.Count][addr];
            }
            else if (addr >= 0x4000 && addr <= 0x7FFF)
            {
                if (romBanks.Count >= 16)
                    return romBanks[((bank2 << 5) | bank1) % romBanks.Count][addr - 0x4000];

                return romBanks[bank1 % romBanks.Count][addr - 0x4000];
            }

            return 0xFF;
        }

        public void LoadProgram(byte[] bytes)
        {
            int bank = -1;
            int counter = 0;

            while (counter < bytes.Length)
            {
                if (counter % 16384 == 0)
                {
                    romBanks.Add(new byte[16384]);
                    bank++;
                }

                romBanks[bank][counter % 16384] = bytes[counter];
                counter++;
            }
        }

        public void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
                cartRAM[addr] = value;
        }

        public byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
                return cartRAM[addr];

            return 0xFF;
        }

        public void Tick(int lastCycleCount) { }
    }
}