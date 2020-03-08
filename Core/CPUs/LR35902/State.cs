using System;
using System.IO;
using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.LR35902
{
    public class State : CPUs.Intel8080.State
    {
        public new Flags Flags;

        public new byte F
        {
            get 
            {
                return (byte)((Flags.Zero ? 1 : 0) << 7 |
                        (Flags.Subtract ? 1 : 0) << 6 |
                        (Flags.HalfCarry ? 1 : 0) << 5 |
                        (Flags.Carry ? 1 : 0) << 4);
            }
        }
    }
}