using bEmu.Core.Enums;

namespace bEmu.Core.GamePad
{
    public interface IGamePadBuilder<T> where T : struct
    {
        IGamePad Build(T[] pressedKeys);
        GamePadKey GetGamePadKey(T key);
    }
}