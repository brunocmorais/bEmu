using bEmu.Core.Memory;
using bEmu.Core.System;

namespace bEmu.Core.CPU
{
    public abstract class CPU<TState, TMMU> : ICPU<TState, TMMU> 
        where TState : class, IState
        where TMMU : class, IMMU
    {
        public ISystem System { get; }
        public TState State { get; }
        public TMMU MMU { get; }

        public CPU(ISystem system)
        {
            System = system;
            State = system.State as TState;
            MMU = system.MMU as TMMU;
        }

        public virtual IOpcode StepCycle()
        {
            State.Instructions++;
            return default(IOpcode);
        }

        public void IncreaseCycles(sbyte cycles)
        {
            State.Cycles += cycles;
        }
    }
}