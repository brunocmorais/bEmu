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
            var pc = LittleEndian.ToBytes(PC).ToList();
            var sp = LittleEndian.ToBytes(SP).ToList();
            var cycles = LittleEndian.ToBytes(Cycles).ToList();
            var halted = LittleEndian.ToBytes(Halted).ToList();
            var instructions = LittleEndian.ToBytes(Instructions).ToList();
            var v = LittleEndian.ToBytes(V).ToList();
            var keys = LittleEndian.ToBytes(Keys).ToList();
            var i = LittleEndian.ToBytes(I).ToList();
            var stack = LittleEndian.ToBytes(Stack).ToList();
            var delay = LittleEndian.ToBytes(Delay).ToList();
            var sound = LittleEndian.ToBytes(Sound).ToList();
            var draw = LittleEndian.ToBytes(Draw).ToList();
            var superChipMode = LittleEndian.ToBytes(SuperChipMode).ToList();
            var r = LittleEndian.ToBytes(R).ToList();
            
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
            PC = LittleEndian.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            SP = LittleEndian.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Cycles = LittleEndian.FromBytes<int>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            Halted = LittleEndian.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            Instructions = LittleEndian.FromBytes<ulong>(bytes.GetRange((counter += size = sizeof(int)) - size, size));
            V = LittleEndian.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * V.Length)) - size, size), V.Length);
            Keys = LittleEndian.FromBytes<bool>(bytes.GetRange((counter += size = (sizeof(bool) * Keys.Length)) - size, size), Keys.Length);
            I = LittleEndian.FromBytes<ushort>(bytes.GetRange((counter += size = sizeof(ushort)) - size, size));
            Stack = LittleEndian.FromBytes<ushort>(bytes.GetRange((counter += size = (sizeof(ushort) * Stack.Length)) - size, size), Stack.Length);
            Delay = LittleEndian.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Sound = LittleEndian.FromBytes<byte>(bytes.GetRange((counter += size = sizeof(byte)) - size, size));
            Draw = LittleEndian.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            SuperChipMode = LittleEndian.FromBytes<bool>(bytes.GetRange((counter += size = sizeof(bool)) - size, size));
            R = LittleEndian.FromBytes<byte>(bytes.GetRange((counter += size = (sizeof(byte) * R.Length)) - size, size), R.Length);
        }
    }
}