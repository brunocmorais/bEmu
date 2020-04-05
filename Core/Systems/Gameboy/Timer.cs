namespace bEmu.Core.Systems.Gameboy
{
    public class Timer
    {
        private readonly MMU mmu;

        public Timer(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte DIV 
        {
            get { return mmu[0xFF04];}
            set { mmu[0xFF04] = value; }
        }

        public byte TIMA
        {
            get { return mmu[0xFF05]; }
            set { mmu[0xFF05] = value; }
        }

        public byte TMA
        {
            get { return mmu[0xFF06]; }
            set { mmu[0xFF06] = value; }
        }

        public byte TAC
        {
            get { return mmu[0xFF07]; }
            set { mmu[0xFF07] = value; }
        }

        public bool Enabled => (TAC & 0x4) == 0x4;

        public byte Step
        {
            get 
            {
                switch (TAC & 0x3)
                {
                    case 0:  return 0x03;
                    case 1:  return 0xFF;
                    case 2:  return 0x30;
                    case 3:  return 0x10;
                    default: return 0x00;
                }
            }
        }
    }
}