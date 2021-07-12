using bEmu.Core.Enums;

namespace bEmu.Core.Input
{
    public interface IGamePadBuilder<T> where T : struct
    {
        GamePad Build(T[] pressedKeys);
    }
}