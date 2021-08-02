using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.GamePad
{
    public interface IGamePad
    {
        ISet<GamePadKey> PressedKeys { get; }
        bool IsKeyDown(GamePadKey key);
        bool IsKeyUp(GamePadKey key);
    }
}