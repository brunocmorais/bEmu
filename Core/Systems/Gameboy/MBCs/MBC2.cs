using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.MBCs
{

    public class MBC2 : IMBC
    {
        int romb;
        IList<byte[]> romBanks;
        byte[] rom0 => romBanks[0];
        byte[] cartRAM = new byte[512];
        byte ramg;

        public MBC2()
        {
            romBanks = new List<byte[]>();
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
                {
                    ramg = value;
                }
            }
        }

        public byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return romBanks[romb % romBanks.Count][addr - 0x4000];

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
                cartRAM[addr] = (byte) (value & 0xF);
        }

        public byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
                return (byte) (cartRAM[addr % cartRAM.Length] | 0xF0);

            return 0xFF;
        }

        public void Tick(int lastCycleCount) { }
    }
}