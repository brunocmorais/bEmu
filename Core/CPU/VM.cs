using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Video;

namespace bEmu.Core.CPU
{
    public abstract class VM<TState, TPPU> : IVM<TState, TPPU>
        where TState : class, IState
        where TPPU : class, IPPU
    {
        public ISystem System { get; set; }
        public TState State => (System.State as TState);
        public MMU MMU => (System.MMU as MMU);
        public TPPU PPU => (System.PPU as TPPU);
        public int Clock { get; }

        public VM(ISystem system, int clock)
        {
            System = system;
            Clock = clock;
        }

        public virtual IOpcode StepCycle()
        {
            System.State.Instructions++;
            return default(IOpcode);
        }
    }
}