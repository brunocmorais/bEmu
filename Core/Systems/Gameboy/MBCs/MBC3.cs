using System.Collections.Generic;
using System.IO;

namespace bEmu.Core.Systems.Gameboy.MBCs
{

    public class MBC3 : DefaultMBC, IHasRTC
    {
        protected override byte[] cartRAM => ramBanks != null ? ramBanks[bank2] : null;
        int bank1;
        int bank2;
        byte ramg;
        RTC rtc;

        public MBC3(string fileName, bool ram, bool timer, bool battery) : base(fileName, battery)
        {
            bank1 = 1;
            bank2 = 0;

            if (timer)
                rtc = new RTC();

            if (ram)
                InitializeRAMBanks(4, 8192);
        }

        public override void SetMode(int addr, byte value)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF)
                ramg = value;
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                if (value == 0x00)
                    bank1 = 0x01;
                else
                    bank1 = value & 0x7F;
            }
            else if (addr >= 0x4000 && addr <= 0x5FFF)
            {
                if (value >= 0x0 && value <= 0x3)
                    bank2 = value & 0x3;
                else if (value >= 0x8 && value <= 0xC && rtc != null)
                    rtc.SetMode(value);
            }
            else if (addr >= 0x6000 && addr <= 0x7FFF && rtc != null)
                rtc.Latched = (value & 0x1) == 0x1 && !rtc.Latched;
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return romBanks[bank1 % romBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public override void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
            {
                if ((rtc == null || rtc.Mode == 0x0) && cartRAM != null)
                    cartRAM[addr] = value;
                else if (rtc != null)
                    rtc.Write(value);
            }
        }

        public override byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
            {
                if ((rtc == null || rtc.Mode == 0x0) && cartRAM != null)
                    return cartRAM[addr];
                else if (rtc != null)
                    return rtc.Read();
            }

            return 0xFF;
        }

        public void Tick(int lastCycleCount)
        {
            if (rtc != null)
                rtc.Tick(lastCycleCount);
        }
    }
}