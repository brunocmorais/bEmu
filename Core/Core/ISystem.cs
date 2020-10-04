namespace bEmu.Core
{
    public interface ISystem
    {
        IRunner Runner { get; }
        IState State { get; }
        IMMU MMU { get; }
        IPPU PPU { get; }
        IAPU APU { get; }
        string FileName { get; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        void Initialize();
        IState GetInitialState();
        void Reset();
        bool LoadState();
        void SaveState();
    }
}