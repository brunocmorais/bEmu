using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public interface IBaseGame
    {
        void StopGame();
        void UpdateGame();
        void UpdateGamePad(KeyboardState keyboardState);
    }
}