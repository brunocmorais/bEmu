using System.Collections.Generic;
using bEmu.Core.Enums;

namespace bEmu.Core.GamePad
{
    public interface IGamePadState
    {
        IEnumerable<GamePadKey> PressedKeys { get; }
        bool HasBeenPressed(GamePadKey gamePadKey);
        bool HasBeenReleased(GamePadKey gamePadKey);
        void UpdateState(IGamePad gamePad);
    }
}