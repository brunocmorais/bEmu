using System;
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

        void StopGame();
        void UpdateGame();
        void UpdateGamePad(KeyboardState keyboardState);
    }
}