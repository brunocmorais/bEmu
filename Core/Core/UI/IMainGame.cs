using System;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.UI.Menus;

namespace bEmu.Core.UI
{
    public interface IMainGame : IDisposable
    {
        IOptions Options { get; }
        IGameMenu Menu { get; }
        IOSD Osd { get; }
        bool IsRunning { get; }
        ISystem System { get; }

        void LoadSystem(SystemType system, string file);
        void StopGame();
        void ResetGame();
        void LoadState();
        void SaveState();
        void SetScreenSize();
        void CloseGame();
        void Pause();
        void SetSound(bool enable);
    }
}