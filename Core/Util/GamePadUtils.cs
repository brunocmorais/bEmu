using System.Collections.Generic;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.Input;

namespace bEmu.Core.Util
{
    public static class GamePadUtils
    {
        static IGamePadState currentKeyState;
        static IGamePadState previousKeyState;

        public static void UpdateState(IGamePadState gamePadState)
        {
            previousKeyState = currentKeyState;
            currentKeyState = gamePadState;
        }

        public static bool HasBeenPressed(GamePadKey gamePadKey)
        {
            return currentKeyState.IsKeyDown(gamePadKey) && !previousKeyState.IsKeyDown(gamePadKey);
        }

        public static bool HasBeenReleased(GamePadKey gamePadKey)
        {
            return !currentKeyState.IsKeyDown(gamePadKey) && previousKeyState.IsKeyDown(gamePadKey);
        }

        public static IEnumerable<GamePadKey> GetPressedGamePadKeys()
        {
            return currentKeyState.GetPressedKeys();
        }
    }
}