using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.Input
{

    public struct GamePad : IGamePad
    {
        public ISet<GamePadKey> PressedKeys { get; }

        public GamePad(IEnumerable<GamePadKey> keys)
        {
            PressedKeys = new HashSet<GamePadKey>();
            
            foreach (var key in keys)
                PressedKeys.Add(key);
        }

        public bool IsKeyDown(GamePadKey key) => PressedKeys.Contains(key);
        public bool IsKeyUp(GamePadKey key) => !IsKeyDown(key);
    }
}