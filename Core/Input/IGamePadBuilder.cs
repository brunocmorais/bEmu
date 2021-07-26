using bEmu.Core.Enums;

namespace bEmu.Core.Input
{
    public interface IGamePadBuilder<T> where T : struct
    {
        IGamePad Build(T[] pressedKeys);
        GamePadKey GetGamePadKey(T key);
    }
}