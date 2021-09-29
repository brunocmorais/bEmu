using System.Collections.Generic;
using System.Linq;
using bEmu.Core;
using bEmu.Core.CPU;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Systems.Chip8
{
    public class State : Core.System.State<ushort, ushort>
    {
        public State(IRunnableSystem system) : base(system) { }
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
            SP = 0;
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
            var pc = Endianness.ToBytes(PC).ToList();
            var sp = Endianness.ToBytes(SP).ToList();
            var cycles = Endianness.ToBytes(Cycles).ToList();
            var halted = Endianness.ToBytes(Halted).ToList();
            var instructions = Endianness.ToBytes(Instructions).ToList();
            var v = Endianness.ToBytes(V).ToList();
            var keys = Endianness.ToBytes(Keys).ToList();
            var i = Endianness.ToBytes(I).ToList();
            var stack = Endianness.ToBytes(Stack).ToList();
            var delay = Endianness.ToBytes(Delay).ToList();
            var sound = Endianness.ToBytes(Sound).ToList();
            var draw = Endianness.ToBytes(Draw).ToList();
            var superChipMode = Endianness.ToBytes(SuperChipMode).ToList();
            var r = Endianness.ToBytes(R).ToList();
            
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
            PC = Endianness.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            SP = Endianness.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Cycles = Endianness.FromBytes<int>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            Halted = Endianness.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            Instructions = Endianness.FromBytes<ulong>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            V = Endianness.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * V.Length)) - size, size), V.Length);
            Keys = Endianness.FromBytes<bool>(bytes.GetRange((counter += size = (sizeof(bool) * Keys.Length)) - size, size), Keys.Length);
            I = Endianness.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Stack = Endianness.FromBytes<ushort>(bytes.GetRange((counter += size = (sizeof(ushort) * Stack.Length)) - size, size), Stack.Length);
            Delay = Endianness.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Sound = Endianness.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Draw = Endianness.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            SuperChipMode = Endianness.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            R = Endianness.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * R.Length)) - size, size), R.Length);
        }
    }
}