using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.System
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
        int CycleCount { get; }
        int Cycles { get; }
        int StartAddress { get; }
        string FileName { get; }
        string SaveFileName { get; }
        string SaveStateName { get; }
        int Frame { get; }
        int Frameskip { get; set; }
        IFrameBuffer Framebuffer { get; }
        byte[] SoundBuffer { get; }
        IDebugger Debugger { get; }
        SystemType Type { get; }
        bool SkipFrame { get; }

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