using System;
using System.Diagnostics;
using bEmu.Core.CPU.LR35902;
using bEmu.Core.System;

namespace bEmu.Systems.Gameboy
{
    public class Timer
    {
        private readonly IGBSystem system;
        private int cycles;
        private int cyclesDivider;
        private bEmu.Systems.Gameboy.MMU Mmu => (system.MMU as bEmu.Systems.Gameboy.MMU);
        private State State => (system.State as bEmu.Systems.Gameboy.State);
        public bool Enabled => (TAC & 0x4) == 0x4;

        public byte DIV 
        {
            get { return Mmu.IO[0x04];}
            set { Mmu.IO[0x04] = value; }
        }

        public byte TIMA
        {
            get { return Mmu.IO[0x05]; }
            set { Mmu.IO[0x05] = value; }
        }

        public byte TMA
        {
            get { return Mmu.IO[0x06]; }
            set { Mmu.IO[0x06] = value; }
        }

        public byte TAC
        {
            get { return Mmu.IO[0x07]; }
            set { Mmu.IO[0x07] = value; }
        }

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
                    default: throw new Exception();
                }
            }
        }

        public Timer(IGBSystem system)
        {
            this.system = system;
            cycles = 0;
            cyclesDivider = 0;
        }

        public void UpdateTimers(int lastCycleCount)
        {
            cyclesDivider += lastCycleCount;

            if (cyclesDivider >= 256)
            {
                DIV++;
                cyclesDivider -= 256;
            }

            if (Enabled)
            {
                cycles += lastCycleCount;

                while (cycles >= Step)
                {
                    if (TIMA == 0xFF)
                    {
                        State.RequestInterrupt(InterruptType.Timer);
                        TIMA = TMA;
                    }
                    else
                        TIMA++;

                    cycles -= Step;
                }
            }
        }
    }
}