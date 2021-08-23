using bEmu.Core.Audio;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Video;

namespace bEmu.Core.CPU
{
    public abstract class VM<TState, TMMU, TPPU, TAPU> : IVM<TState, TMMU, TPPU, TAPU>
        where TState : class, IState
        where TMMU : class, IMMU
        where TPPU : class, IPPU
        where TAPU : class, IAPU
    {
        public IRunnableSystem System { get; }
        public TState State { get; }
        public TMMU MMU { get; }
        public TPPU PPU { get; }
        public TAPU APU { get; }
        public int Clock { get; }

        public VM(IAudioVideoSystem system, int clock)
        {
            System = system;
            State = System.State as TState;
            MMU = System.MMU as TMMU;
            PPU = system.PPU as TPPU;
            APU = system.APU as TAPU;
            Clock = clock;
        }

        public virtual IOpcode StepCycle()
        {
            System.State.Instructions++;
            return default(IOpcode);
        }
    }
}