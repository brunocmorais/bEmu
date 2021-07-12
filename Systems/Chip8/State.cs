using System.Collections.Generic;
using System.Linq;
using bEmu.Core;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Systems.Chip8
{
    public class State : Core.System.State
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
            var pc = ByteOperations.ToBytes(PC).ToList();
            var sp = ByteOperations.ToBytes(SP).ToList();
            var cycles = ByteOperations.ToBytes(Cycles).ToList();
            var halted = ByteOperations.ToBytes(Halted).ToList();
            var instructions = ByteOperations.ToBytes(Instructions).ToList();
            var v = ByteOperations.ToBytes(V).ToList();
            var keys = ByteOperations.ToBytes(Keys).ToList();
            var i = ByteOperations.ToBytes(I).ToList();
            var stack = ByteOperations.ToBytes(Stack).ToList();
            var delay = ByteOperations.ToBytes(Delay).ToList();
            var sound = ByteOperations.ToBytes(Sound).ToList();
            var draw = ByteOperations.ToBytes(Draw).ToList();
            var superChipMode = ByteOperations.ToBytes(SuperChipMode).ToList();
            var r = ByteOperations.ToBytes(R).ToList();
            
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
            PC = ByteOperations.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            SP = ByteOperations.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Cycles = ByteOperations.FromBytes<int>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            Halted = ByteOperations.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            Instructions = ByteOperations.FromBytes<ulong>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            V = ByteOperations.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * V.Length)) - size, size), V.Length);
            Keys = ByteOperations.FromBytes<bool>(bytes.GetRange((counter += size = (sizeof(bool) * Keys.Length)) - size, size), Keys.Length);
            I = ByteOperations.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Stack = ByteOperations.FromBytes<ushort>(bytes.GetRange((counter += size = (sizeof(ushort) * Stack.Length)) - size, size), Stack.Length);
            Delay = ByteOperations.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Sound = ByteOperations.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Draw = ByteOperations.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            SuperChipMode = ByteOperations.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            R = ByteOperations.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * R.Length)) - size, size), R.Length);
        }
    }
}