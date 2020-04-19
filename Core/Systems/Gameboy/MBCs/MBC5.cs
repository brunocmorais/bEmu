using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.MBCs
{

    public class MBC5 : IMBC
    {
        byte romb0;
        byte romb1;
        byte ramb;
        IList<byte[]> romBanks;
        IList<byte[]> ramBanks;
        byte[] rom0 => romBanks[0];
        byte[] cartRAM => ramBanks[ramb];
        byte ramg;

        public MBC5()
        {
            romBanks = new List<byte[]>();
            ramBanks = new List<byte[]>();
            romb0 = 1;
            romb1 = 0;
            ramb = 0;

            for (int i = 0; i < 16; i++)
                ramBanks.Add(new byte[8192]);
        }

        public void SetMode(int addr, byte value)
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

        public byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return romBanks[(((romb1 << 8) | romb0)) % romBanks.Count][addr - 0x4000];

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
            if (ramg == 0x0A)
                cartRAM[addr] = value;
        }

        public byte ReadCartRAM(int addr)
        {
            if (ramg == 0x0A)
                return cartRAM[addr];

            return 0xFF;
        }

        public void Tick(int lastCycleCount) { }
    }
}