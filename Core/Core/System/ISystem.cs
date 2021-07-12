using bEmu.Core.Audio;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;

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
        int StartAddress { get; }
        string FileName { get; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        int Frame { get; }
        int Frameskip { get; set; }
        Framebuffer Framebuffer { get; }
        byte[] SoundBuffer { get; }
        IDebugger Debugger { get; }
        SystemType Type { get; }

        IState GetInitialState();
        void Reset();
        bool LoadState();
        void SaveState();
        bool Update();
        void Stop();
        void UpdateGamePad(IGamePad gamePad);
        void ResetCycles();
        void LoadProgram();
        void AttachDebugger();
    }
}