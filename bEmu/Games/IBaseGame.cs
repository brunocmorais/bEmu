using System;
using System.Collections.Generic;
using bEmu.Components;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public interface IBaseGame : IDisposable
    {
        ISystem System { get; }
        IPPU Gpu { get; }
        IState State { get; }
        IMMU Mmu { get; }
        IAPU Apu { get; }
        bool IsRunning { get; set; }
        DateTime LastStartDate { get; }
        Options Options { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Stack<IMenu> Menus { get; }

        void LoadGame(string file);
        void StopGame();
        void UpdateGame();
        void ResetGame();
        void LoadState();
        void SaveState();
        void UpdateGamePad(KeyboardState keyboardState);
    }
}