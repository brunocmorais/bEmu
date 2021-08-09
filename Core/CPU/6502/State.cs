using bEmu.Core.System;

namespace bEmu.Core.CPU.MOS6502
{
    public class State : Core.System.State<ushort, byte>
    {
        public byte AC { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public Flags Flags { get; set; }
        public byte SR
        {
            get
            {
                return (byte)((Flags.Negative ? 1 : 0) << 7 |
                        (Flags.Overflow ? 1 : 0) << 6 |
                        (Flags.Break ? 1 : 0) << 4 |
                        (Flags.Decimal ? 1 : 0) << 3 |
                        (Flags.Interrupt ? 1 : 0) << 2 |
                        (Flags.Zero ? 1 : 0) << 1 |
                        (Flags.Carry ? 1 : 0));
            }
        }

        public State(ISystem system) : base(system)
        {
        }

        public override void LoadState(byte[] value)
        {
            throw new global::System.NotImplementedException();
        }

        public override byte[] SaveState()
        {
            throw new global::System.NotImplementedException();
        }
    }
}