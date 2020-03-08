namespace bEmu.Core
{
    public interface ICPU<TState> : IExecutor
    {
        TState State { get; }
        MMU MMU { get; }
    }
}