using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class Timer
    {
        private readonly ISystem system;
        int cycles = 0;
        int cyclesDivider = 0;

        public Timer(ISystem system)
        {
            this.system = system;
            (system.MMU as bEmu.Core.Systems.Gameboy.MMU).InitTimer();
        }

        public byte DIV 
        {
            get { return system.MMU[0xFF04];}
            set { system.MMU[0xFF04] = value; }
        }

        public byte TIMA
        {
            get { return system.MMU[0xFF05]; }
            set { system.MMU[0xFF05] = value; }
        }

        public byte TMA
        {
            get { return system.MMU[0xFF06]; }
            set { system.MMU[0xFF06] = value; }
        }

        public byte TAC
        {
            get { return system.MMU[0xFF07]; }
            set { system.MMU[0xFF07] = value; }
        }

        public bool Enabled => (TAC & 0x4) == 0x4;

        public int Step
        {
            get 
            {
                switch (TAC & 0x3)
                {
                    case 0:  return 1024;
                    case 1:  return 16;
                    case 2:  return 64;
                    case 3:  return 256;
                    default: return 0x00;
                }
            }
        }

        public void UpdateTimers(int lastCycleCount)
        {
            cyclesDivider += lastCycleCount;

            if (cyclesDivider >= 256)
            {
                DIV++;
                cyclesDivider = 0;
            }

            if (Enabled)
            {
                cycles += lastCycleCount;

                if (cycles >= Step)
                {
                    if (TIMA == 0xFF)
                    {
                        (system.State as bEmu.Core.Systems.Gameboy.State).RequestInterrupt(InterruptType.Timer);
                        TIMA = TMA;
                    }
                    else
                    {
                        TIMA++;
                        cycles = 0;
                    }
                }
            }
        }
    }
}