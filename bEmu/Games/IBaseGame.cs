using System;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public interface IBaseGame : IDisposable
    {
        void StopGame();
        void UpdateGame();
        void UpdateGamePad(KeyboardState keyboardState);
    }
}