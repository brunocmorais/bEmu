using bEmu.Core.Audio;
using bEmu.Core.Enums;
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
        public TState State => System.State as TState;
        public TMMU MMU => System.MMU as TMMU;
        public TPPU PPU => (System as IAudioVideoSystem).PPU as TPPU;
        public TAPU APU => (System as IAudioVideoSystem).APU as TAPU;
        public int Clock { get; }
        public IEndianness Endianness { get; }

        public VM(Endianness endianness, IAudioVideoSystem system, int clock)
        {
            Endianness = EndiannessFactory.Instance.Get(endianness);
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