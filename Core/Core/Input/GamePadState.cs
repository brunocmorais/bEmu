using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Core;
using bEmu.Core.Components;
using bEmu.Core.Enums;

namespace bEmu.Core.Input
{
    public struct GamePadState : IGamePadState
    {
        private readonly Dictionary<GamePadKey, bool> keys;

        public GamePadState(GamePadKey[] pressedKeys)
        {
            keys = new Dictionary<GamePadKey, bool>();

            foreach (var key in Enum.GetValues(typeof(GamePadKey)).Cast<GamePadKey>())
                keys.Add(key, pressedKeys.Contains(key));
        }

        public bool IsKeyDown(GamePadKey key) => keys[key];

        public IEnumerable<GamePadKey> GetPressedKeys()
        {
            foreach (var key in keys)
                if (key.Value == true)
                    yield return key.Key;
        }
    }
}