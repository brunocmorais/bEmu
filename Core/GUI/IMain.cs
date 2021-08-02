using System;
using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.GUI.Menus;
using bEmu.Core.GUI.Popups;
using bEmu.Core.Audio;

namespace bEmu.Core.GUI
{
    public interface IMain : IDisposable
    {
        IOptions Options { get; set; }
        IOSD Osd { get; }
        bool IsRunning { get; }
        ISystem System { get; set; }
        MenuManager MenuManager { get; }
        IPopupManager PopupManager { get; }
        Wave Recording { get; }

        void LoadSystem(SystemType system, string file);
        void StopGame();
        void ResetGame();
        void LoadState();
        void SaveState();
        void SetScreenSize();
        void CloseGame();
        void Pause();
        void SetSound(bool enable);
        void SetScaler();
        void Snapshot();
        void StartRecording();
        void StopRecording();
        void Start();
    }
}