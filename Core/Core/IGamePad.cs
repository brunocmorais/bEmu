using System.Collections.Generic;

namespace bEmu.Core
{
    public interface IGamePad
    {
        ISet<GamePadKey> PressedKeys { get; set; }
        void AddPressedKeys(IEnumerable<GamePadKey> keys);
        bool IsKeyDown(GamePadKey key);
        bool IsKeyUp(GamePadKey key);
        void Reset();
    }
}