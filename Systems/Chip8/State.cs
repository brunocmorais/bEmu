using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class State : Core.State
    {
        public State(ISystem system) : base(system) { }
        public byte[] V { get; set; }
        public bool[] Keys { get; set; }
        public ushort I { get; set; }
        public ushort[] Stack { get; set; }
        public byte Delay { get; set; }
        public byte Sound { get; set; }
        public bool Draw { get; set; }
        public bool SuperChipMode { get; set; }
        public byte[] R { get; set; }
        public override void Reset()
        {
            base.Reset();

            PC = 0x200;
            V = new byte[16];
            Keys = new bool[16];
            I = 0;
            Stack = new ushort[16];
            Delay = 0;
            Sound = 0;
            Draw = false;
            SuperChipMode = false;
            R = new byte[16];
        }
    }
}