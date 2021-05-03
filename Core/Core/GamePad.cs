using System.Collections.Generic;

namespace bEmu.Core
{

    public class GamePad : IGamePad
    {
        public ISet<GamePadKey> PressedKeys { get; set; }

        public GamePad()
        {
            PressedKeys = new HashSet<GamePadKey>();
        }

        public void AddPressedKeys(IEnumerable<GamePadKey> keys)
        {
            foreach (var key in keys)
                PressedKeys.Add(key);
        }

        public bool IsKeyDown(GamePadKey key)
        {
            return PressedKeys.Contains(key);
        }

        public bool IsKeyUp(GamePadKey key)
        {
            return !IsKeyDown(key);
        }

        public void Reset()
        {
            PressedKeys.Clear();
        }
    }
}