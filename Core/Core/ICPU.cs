namespace bEmu.Core
{
    public interface ICPU<TState> : IExecutor
    {
        TState State { get; }
        IMMU MMU { get; }
    }
}