using bEmu.Core.CPU.MOS6502;
using bEmu.Core.System;

namespace bEmu.Systems.NES
{
    public class CPU : MOS6502<State, MMU>
    {
        public CPU(ISystem system, int clock) : base(system, clock)
        {
        }
    }
}