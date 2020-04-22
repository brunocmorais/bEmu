namespace bEmu.Core
{
    public interface IVM<TState, TPPU> : IRunner
    { 
        TState State { get; }
        MMU MMU { get; }
        TPPU PPU { get; }
    }
}