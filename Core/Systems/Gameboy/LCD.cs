using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class LCD
    {
        private readonly MMU mmu;

        public LCD(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte LCDC
        {
            get { return mmu.IO[0x40]; }
            set { mmu.IO[0x40] = value; }
        }

        public byte STAT
        {
            get { return mmu.IO[0x41]; }
            set { mmu.IO[0x41] = value; }
        }

        public byte SCY
        {
            get { return mmu.IO[0x42]; }
            set { mmu.IO[0x42] = value; }
        }

        public byte SCX
        {
            get { return mmu.IO[0x43]; }
            set { mmu.IO[0x43] = value; }
        }

        public byte LY
        {
            get { return mmu.IO[0x44]; }
            set { mmu.IO[0x44] = value; }
        }

        public byte LYC
        {
            get { return mmu.IO[0x45]; }
            set { mmu.IO[0x45] = value; }
        }

        public byte BGP => mmu.IO[0x47];
        public byte OBP0 => mmu.IO[0x48];
        public byte OBP1 => mmu.IO[0x49];

        public bool GetLCDCFlag(LCDC option)
        {
            int op = (0x1 << (int) option);
            return (LCDC & op) == op;
        }

        public int GetSTATFlag(STAT option)
        {
            int op = (int) option;

            if (op < 1)
                return STAT & 0x3;

            return (STAT & (0x1 << op)) >> op;
        }

        public void SetSTATFlag(STAT option, bool value)
        {
            if ((int) option < 3)
                return; 

            byte mask = (byte) (0x1 << (int) option);

            if (value)
            {
                mask = (byte) ~mask;
                STAT &= mask;
            }
            else
                STAT |= mask;
        }

        public void SetSTATMode(int number)
        {
            byte value = (byte) (number | 0xFC);
            STAT &= value;
        }

        public void SetLCDCFlag(LCDC option, bool value)
        {
            byte mask = (byte) (0x1 << (int) option);

            if (value)
            {
                mask = (byte) ~mask;
                LCDC &= mask;
            }
            else
                LCDC |= mask;
        }
    }
}