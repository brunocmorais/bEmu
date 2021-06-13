namespace bEmu.Core
{
    public interface ISystem
    {
        IRunner Runner { get; }
        IState State { get; }
        IMMU MMU { get; }
        IPPU PPU { get; }
        IAPU APU { get; }
        IDebugger Debugger { get; set; }
        int Width { get; }
        int Height { get; }
        int RefreshRate { get; }
        int CycleCount { get; }
        int Cycles { get; }
        string FileName { get; set; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        IState GetInitialState();
        void Reset();
        bool LoadState();
        void SaveState();
        bool Update();
        void Stop();
        void UpdateGamePad(IGamePad gamePad);
        void ResetCycles();
    }
}