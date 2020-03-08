using bEmu.Core;

namespace bEmu.Core.Systems.Chip8
{
    public class State : Core.State
    {
        public byte[] V { get; set; }
        public bool[] Keys { get; set; }
        public ushort I { get; set; }
        public ushort[] Stack { get; set; }
        public byte Delay { get; set; }
        public byte Sound { get; set; }
        public bool Draw { get; set; }
        public bool SuperChipMode { get; set; }
        public byte[] R { get; set; }
    }
}