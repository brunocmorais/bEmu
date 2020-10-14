using System;
using System.Collections.Generic;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.GameSystems;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public interface IMainGame : IDisposable
    {
        Options Options { get; set; }
        GameMenu Menu { get; }
        GraphicsDevice GraphicsDevice { get; }
        ContentManager Content { get; }
        int DrawCounter { get; }
        IGameSystem GameSystem { get; }
        Texture2D BackBuffer { get; set; }
        OSD Osd { get; }
        SpriteBatch SpriteBatch { get; }
        Fonts Fonts { get; }
        bool IsRunning { get; set; }

        void LoadGame(SupportedSystems system, string file);
        void StopGame();
        void ResetGame();
        void LoadState();
        void SaveState();
        void SetScaler();
        void SetScreenSize();
        void CloseGame();
        void Pause();
    }
}