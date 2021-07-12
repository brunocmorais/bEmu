using bEmu.Core.Memory;

namespace bEmu.Core.CPU
{
    public interface IVM<TState, TPPU> : IRunner
    { 
        TState State { get; }
        MMU MMU { get; }
        TPPU PPU { get; }
    }
}