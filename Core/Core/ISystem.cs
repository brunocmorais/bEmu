namespace bEmu.Core
{
    public interface ISystem
    {
        IExecutor Runner { get; }
        IState State { get; }
        IMMU MMU { get; }
        IPPU PPU { get; }
        void Initialize();
        IState GetInitialState();
    }
}