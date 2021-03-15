using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class GamePad : IGamePad
    {
        public bool[] Keys { get; set; }

        public GamePad()
        {
            Keys = new bool[16];
        }
    }
}