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
        int StartAddress { get; }
        string FileName { get; set; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        int Frame { get; set; }
        int Frameskip { get; set; }
        Framebuffer Framebuffer { get; }
        byte[] SoundBuffer { get; }

        IState GetInitialState();
        void Reset();
        bool LoadState();
        void SaveState();
        bool Update();
        void Stop();
        void UpdateGamePad(IGamePad gamePad);
        void ResetCycles();
        void LoadProgram();
    }
}