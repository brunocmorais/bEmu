using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems
{

    public class MBC1 : IMBC
    {
        int Mode { get; set; }
        int Bank1 { get; set; }
        int Bank2 { get; set; }
        IList<byte[]> ROMBanks { get; set; }
        IList<byte[]> RAMBanks { get; set; }
        byte[] ROM0 => ROMBanks[0];
        byte[] cartRAM => Mode == 0 ? RAMBanks[0] : RAMBanks[Bank2];
        byte RAMG { get; set; }

        public MBC1()
        {
            ROMBanks = new List<byte[]>();
            RAMBanks = new List<byte[]>();
            Bank1 = 1;
            Bank2 = 0;
            Mode = 0;

            for (int i = 0; i < 4; i++)
                RAMBanks.Add(new byte[8192]);
        }

        public void SetMode(ushort addr, byte value)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF)
                RAMG = value;
            if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                if ((value & 0x1F) == 0x00)
                    Bank1 = 0x01;
                else
                    Bank1 = value & 0x1F;
            }
            if (addr >= 0x4000 && addr <= 0x5FFF)
                Bank2 = value & 0x3;
            if (addr >= 0x6000 && addr <= 0x7FFF)
                Mode = value & 0x1;
        }

        public byte ReadROM(ushort addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
            {
                if (Mode == 0)
                    return ROM0[addr];
                else
                    return ROMBanks[(Bank2 << 5) % ROMBanks.Count][addr];
            }
            else if (addr >= 0x4000 && addr <= 0x7FFF)
            {
                if (ROMBanks.Count >= 16)
                    return ROMBanks[((Bank2 << 5) | Bank1) % ROMBanks.Count][addr - 0x4000];

                return ROMBanks[Bank1 % ROMBanks.Count][addr - 0x4000];
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
                    ROMBanks.Add(new byte[16384]);
                    bank++;
                }

                ROMBanks[bank][counter % 16384] = bytes[counter];
                counter++;
            }
        }

        public void WriteCartRAM(ushort addr, byte value)
        {
            if ((RAMG & 0x0F) == 0x0A)
                cartRAM[addr] = value;
        }

        public byte ReadCartRAM(ushort addr)
        {
            if ((RAMG & 0x0F) == 0x0A)
                return cartRAM[addr];

            return 0xFF;
        }
    }
}