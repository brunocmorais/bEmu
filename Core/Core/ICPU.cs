namespace bEmu.Core
{
    public interface ICPU<TState, TMMU> : IRunner
    {
        TState State { get; }
        TMMU MMU { get; }
    }
}