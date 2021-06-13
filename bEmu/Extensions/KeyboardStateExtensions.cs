using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework;
using bEmu.Core;

namespace bEmu.Extensions
{
    public static class KeyboardStateExtensions
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        public static void UpdateState(GameTime gameTime)
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static bool HasBeenReleased(Keys key)
        {
            return !currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyDown(key);
        }

        public static Keys[] GetPressedKeys()
        {
            return currentKeyState.GetPressedKeys();
        }

        public static IEnumerable<GamePadKey> GetPressedGamePadKeys()
        {
            var pressedKeys = GetPressedKeys();

            foreach (var key in pressedKeys)
                yield return GetGamePadKey(key);
        }

        static GamePadKey GetGamePadKey(Keys key)
        {
            switch (key)
            {
                case Keys.D0: return GamePadKey.D0; 
                case Keys.D1: return GamePadKey.D1; 
                case Keys.D2: return GamePadKey.D2; 
                case Keys.D3: return GamePadKey.D3; 
                case Keys.D4: return GamePadKey.D4; 
                case Keys.D5: return GamePadKey.D5; 
                case Keys.D6: return GamePadKey.D6; 
                case Keys.D7: return GamePadKey.D7; 
                case Keys.D8: return GamePadKey.D8; 
                case Keys.D9: return GamePadKey.D9;
                case Keys.A: return GamePadKey.A; 
                case Keys.B: return GamePadKey.B; 
                case Keys.C: return GamePadKey.C; 
                case Keys.D: return GamePadKey.D; 
                case Keys.E: return GamePadKey.E; 
                case Keys.F: return GamePadKey.F; 
                case Keys.G: return GamePadKey.G; 
                case Keys.H: return GamePadKey.H; 
                case Keys.I: return GamePadKey.I; 
                case Keys.J: return GamePadKey.J; 
                case Keys.K: return GamePadKey.K; 
                case Keys.L: return GamePadKey.L; 
                case Keys.M: return GamePadKey.M; 
                case Keys.N: return GamePadKey.N;
                case Keys.O: return GamePadKey.O; 
                case Keys.P: return GamePadKey.P; 
                case Keys.Q: return GamePadKey.Q; 
                case Keys.R: return GamePadKey.R; 
                case Keys.S: return GamePadKey.S; 
                case Keys.T: return GamePadKey.T; 
                case Keys.U: return GamePadKey.U; 
                case Keys.V: return GamePadKey.V; 
                case Keys.W: return GamePadKey.W; 
                case Keys.X: return GamePadKey.X; 
                case Keys.Y: return GamePadKey.Y; 
                case Keys.Z: return GamePadKey.Z;
                case Keys.LeftControl: return GamePadKey.LeftControl; 
                case Keys.RightControl: return GamePadKey.RightControl; 
                case Keys.LeftAlt: return GamePadKey.LeftAlt; 
                case Keys.RightAlt: return GamePadKey.RightAlt;
                case Keys.LeftShift: return GamePadKey.LeftShift; 
                case Keys.RightShift: return GamePadKey.RightShift; 
                case Keys.Space: return GamePadKey.Space; 
                case Keys.Enter: return GamePadKey.Enter;   
                case Keys.Up: return GamePadKey.Up;
                case Keys.Down: return GamePadKey.Down;
                case Keys.Left: return GamePadKey.Left;
                case Keys.Right: return GamePadKey.Right;

                default: return 0;             
            }
        }
    }
}