using bEmu.Core.CPU.LR35902;

namespace bEmu.Systems.Gameboy.GPU
{
    public class LCD
    {
        private readonly MMU mmu;
        private readonly State state;

        public LCD(MMU mmu, State state)
        {
            this.mmu = mmu;
            this.state = state;
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

                if (GetSTATFlag(bEmu.Systems.Gameboy.GPU.STAT.LYCoincidenceInterrupt))
                    state.RequestInterrupt(InterruptType.LcdStat);
            }
            else
                STAT &= 0xFB;
        }
    }
}