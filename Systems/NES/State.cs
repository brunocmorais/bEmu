using bEmu.Core.System;

namespace bEmu.Systems.NES
{
    public class State : Core.CPU.MOS6502.State
    {
        public State(IRunnableSystem system) : base(system)
        {
        }
    }
}