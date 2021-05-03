using System.Collections.Generic;
using System.Linq;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Systems.Chip8
{
    public class State : Core.State
    {
        public State(ISystem system) : base(system) { }
        public byte[] V { get; set; }
        public bool[] Keys {get; set; }
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
            R = new byte[8];
        }

        public override byte[] SaveState()
        {
            var pc = BitUtils.ToBytes(PC).ToList();
            var sp = BitUtils.ToBytes(SP).ToList();
            var cycles = BitUtils.ToBytes(Cycles).ToList();
            var halted = BitUtils.ToBytes(Halted).ToList();
            var instructions = BitUtils.ToBytes(Instructions).ToList();
            var v = BitUtils.ToBytes(V).ToList();
            var keys = BitUtils.ToBytes(Keys).ToList();
            var i = BitUtils.ToBytes(I).ToList();
            var stack = BitUtils.ToBytes(Stack).ToList();
            var delay = BitUtils.ToBytes(Delay).ToList();
            var sound = BitUtils.ToBytes(Sound).ToList();
            var draw = BitUtils.ToBytes(Draw).ToList();
            var superChipMode = BitUtils.ToBytes(SuperChipMode).ToList();
            var r = BitUtils.ToBytes(R).ToList();
            
            return Enumerable.Empty<byte>()
                .Concat(pc)
                .Concat(sp)
                .Concat(cycles)
                .Concat(halted)
                .Concat(instructions)
                .Concat(v)
                .Concat(keys)
                .Concat(i)
                .Concat(stack)
                .Concat(delay)
                .Concat(sound)
                .Concat(draw)
                .Concat(superChipMode)
                .Concat(r)
                .ToArray();
        }

        public override void LoadState(byte[] value)
        {
            var bytes = value.ToList();
            int counter = 0;
            int size = 0;
            PC = BitUtils.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            SP = BitUtils.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Cycles = BitUtils.FromBytes<int>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            Halted = BitUtils.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            Instructions = BitUtils.FromBytes<int>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            V = BitUtils.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * V.Length)) - size, size), V.Length);
            Keys = BitUtils.FromBytes<bool>(bytes.GetRange((counter += size = (sizeof(bool) * Keys.Length)) - size, size), Keys.Length);
            I = BitUtils.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Stack = BitUtils.FromBytes<ushort>(bytes.GetRange((counter += size = (sizeof(ushort) * Stack.Length)) - size, size), Stack.Length);
            Delay = BitUtils.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Sound = BitUtils.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Draw = BitUtils.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            SuperChipMode = BitUtils.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            R = BitUtils.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * R.Length)) - size, size), R.Length);
        }
    }
}