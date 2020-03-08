namespace bEmu.Core
{
    public interface IVM<TState, TPPU> : IExecutor
    { 
        TState State { get; }
        MMU MMU { get; }
        TPPU PPU { get; }
    }
}