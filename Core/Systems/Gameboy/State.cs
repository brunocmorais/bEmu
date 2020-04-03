using System;
using System.IO;
using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.Systems.Gameboy
{
    public class State : CPUs.LR35902.State
    {
        public LCD LCD { get; }

        public State(ISystem system)
        {
            System = system;
            LCD = new LCD(System.MMU as MMU);
        }

        public Joypad Joypad { get; set; }

        public byte IE
        {
            get { return System.MMU[0xFFFF]; }
            set { System.MMU[0xFFFF] = value; }
        }

        public byte IF
        {
            get { return System.MMU[0xFF0F]; }
            set { System.MMU[0xFF0F] = value; }
        }

        public void EnableInterrupt(InterruptType type)
        {
            IE |= (byte) (0x1 << (int) type);
        }

        public void RequestInterrupt(InterruptType type)
        {
            IF |= (byte) (0x1 << (int) type);
        }
    }
}