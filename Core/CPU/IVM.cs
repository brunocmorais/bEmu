using bEmu.Core.Memory;

namespace bEmu.Core.CPU
{
    public interface IVM<TState, TMMU, TPPU, TAPU> : IRunner
    { 
        TState State { get; }
        TMMU MMU { get; }
        TPPU PPU { get; }
        TAPU APU { get; }
    }
}