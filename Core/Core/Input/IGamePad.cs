using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.Input
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