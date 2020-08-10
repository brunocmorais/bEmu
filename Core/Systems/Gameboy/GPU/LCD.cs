using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class LCD
    {
        private readonly MMU mmu;
        public byte BGP => mmu.IO[0x47];
        public byte OBP0 => mmu.IO[0x48];
        public byte OBP1 => mmu.IO[0x49];

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
            set 
            { 
                mmu.IO[0x44] = value; 
                SetLCYRegisterCoincidence();
            }
        }

        public byte LYC
        {
            get { return mmu.IO[0x45]; }
            set { mmu.IO[0x45] = value; }
        }

        public byte WY
        {
            get { return mmu.IO[0x4A]; }
            set { mmu.IO[0x4A] = value; }
        }

        public byte WX
        {
            get { return mmu.IO[0x4B]; }
            set { mmu.IO[0x4B] = value; }
        }

        public bool GetLCDCFlag(LCDC option)
        {
            return (LCDC & (int) option) == (int) option;
        }

        public bool GetSTATFlag(STAT option)
        {
            return (STAT & (int) option) == (int) option;
        }

        public GPUMode Mode
        {
            get => (GPUMode) (STAT & 0x3);
            set => STAT = (byte) ((STAT & 0xFC) | (int) value);
        }

        public void SetLCYRegisterCoincidence()
        {
            if (LY == LYC)
            {
                STAT |= 0x4;

                if (GetSTATFlag(Core.Systems.Gameboy.GPU.STAT.LYCoincidenceInterrupt))
                    mmu.State.RequestInterrupt(InterruptType.LcdStat);
            }
            else
                STAT &= 0xFB;
        }
    }
}