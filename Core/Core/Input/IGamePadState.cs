using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.Input
{
    public interface IGamePadState
    {
        IEnumerable<GamePadKey> GetPressedKeys();
        bool IsKeyDown(GamePadKey key);
    }
}