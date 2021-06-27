using System;
using bEmu.Components;
using bEmu.Systems;
using Microsoft.Xna.Framework.Graphics;
using bEmu.Core;
using bEmu.Menus;

namespace bEmu
{
    public interface IMainGame : IDisposable
    {
        Options Options { get; set; }
        GameMenu Menu { get; }
        GraphicsDevice GraphicsDevice { get; }
        OSD Osd { get; }
        SpriteBatch SpriteBatch { get; }
        Fonts Fonts { get; }
        bool IsRunning { get; set; }
        ISystem System { get; }

        void LoadSystem(SupportedSystems system, string file);
        void StopGame();
        void ResetGame();
        void LoadState();
        void SaveState();
        void SetScaler();
        void SetScreenSize();
        void CloseGame();
        void Pause();
        void SetSound(bool enable);
    }
}