using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.GamePad
{
    public class GamePadState : IGamePadState
    {
        IGamePad currentKeyState;
        IGamePad previousKeyState;

        public void UpdateState(IGamePad gamePad)
        {
            previousKeyState = currentKeyState;
            currentKeyState = gamePad;
        }

        public bool HasBeenPressed(GamePadKey gamePadKey)
        {
            return currentKeyState.IsKeyDown(gamePadKey) && !previousKeyState.IsKeyDown(gamePadKey);
        }

        public bool HasBeenReleased(GamePadKey gamePadKey)
        {
            return !currentKeyState.IsKeyDown(gamePadKey) && previousKeyState.IsKeyDown(gamePadKey);
        }

        public IEnumerable<GamePadKey> PressedKeys => currentKeyState.PressedKeys;
    }
}