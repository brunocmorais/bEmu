using System.IO;
using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{

    public class MBC3 : Mapper, IRAM, IRTC
    {
        private int bank1;
        private int bank2;
        private byte ramg;
        private RTC rtc;
        protected override byte[] CartRam => RamBanks != null ? RamBanks[bank2] : null;
        protected override int ExternalRamSize => 8192;
        protected override int RamBankCount => 4;
        protected string SaveRTCName
        {
            get
            {
                string directory = Path.GetDirectoryName(MMU.System.ROM.FileName);
                string name = Path.GetFileNameWithoutExtension(MMU.System.ROM.FileName) + ".rtc";
                return Path.Combine(directory, name);
            }
        }

        public MBC3(IMMU mmu, bool ram, bool timer, bool battery) : base(16384, mmu, battery, ram)
        {
            bank1 = 1;
            bank2 = 0;

            if (timer)
                rtc = new RTC();
        }

        public void SetMode(int addr, byte value)
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
                rtc.Latch((byte) (value & 0x1));
        }

        public override byte ReadROM(int addr)
        {
            if (addr >= 0x0000 && addr <= 0x3FFF)
                return Rom0[addr];
            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return RomBanks[bank1 % RomBanks.Length][addr - 0x4000];

            return 0xFF;
        }

        public void WriteCartRAM(int addr, byte value)
        {
            if ((ramg & 0x0F) == 0x0A)
            {
                if ((rtc == null || rtc.Mode == 0x0) && CartRam != null)
                    CartRam[addr] = value;
                else if (rtc != null)
                    rtc.Write(value);
            }
        }

        public byte ReadCartRAM(int addr)
        {
            if ((ramg & 0x0F) == 0x0A)
            {
                if ((rtc == null || rtc.Mode == 0x0) && CartRam != null)
                    return CartRam[addr];
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

        public override void Shutdown()
        {
            base.Shutdown();

            if (rtc != null)
                File.WriteAllBytes(SaveRTCName, rtc.Export());
        }
    }
}