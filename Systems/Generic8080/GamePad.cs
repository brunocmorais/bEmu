using bEmu.Core;

namespace bEmu.Systems.Generic8080
{
    public struct GamePad : IGamePad
    {
        public byte Read1 { get; set; }

        public GamePad(byte read1)
        {
            Read1 = read1;
        }
    }
}