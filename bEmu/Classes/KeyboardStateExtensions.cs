using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public static class KeyboardStateExtensions
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        public static void UpdateState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }
    }
}