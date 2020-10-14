using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework;

namespace bEmu.Extensions
{
    public static class KeyboardStateExtensions
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;
        static Dictionary<Keys, double> pressedKeysDic = new Dictionary<Keys, double>(); 

        public static void UpdateState(GameTime gameTime)
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            var pressedKeys = currentKeyState.GetPressedKeys();
            
            foreach (var key in pressedKeysDic)
                if (!pressedKeys.Contains(key.Key))
                    pressedKeysDic.Remove(key.Key);

            foreach (var key in pressedKeys)
            {
                if (!pressedKeysDic.ContainsKey(key))
                    pressedKeysDic.Add(key, gameTime.TotalGameTime.TotalMilliseconds);
            }
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        public static double KeyPressedSince(Keys key, GameTime gameTime)
        {
            if (!pressedKeysDic.ContainsKey(key))
                return 0;

            return gameTime.TotalGameTime.TotalMilliseconds - pressedKeysDic[key];
        }

        public static Keys[] GetPressedKeys()
        {
            return currentKeyState.GetPressedKeys();
        }
    }
}