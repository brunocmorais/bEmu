namespace bEmu.Core
{
    public interface ISystem
    {
        IRunner Runner { get; }
        IState State { get; }
        IMMU MMU { get; }
        IPPU PPU { get; }
        IAPU APU { get; }
        int Width { get; }
        int Height { get; }
        int RefreshRate { get; }
        int CycleCount { get; }
        int Cycles { get; }
        string FileName { get; set; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        void Initialize();
        IState GetInitialState();
        void Reset();
        bool LoadState();
        void SaveState();
        void Update();
        void Stop();
        void UpdateGamePad(IGamePad gamePad);
        void ResetCycles();
    }
}