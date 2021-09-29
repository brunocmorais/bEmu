using bEmu.Core.System;

namespace bEmu.Core.CPU.MOS6502
{
    public class State : Core.System.State<ushort, byte>
    {
        public byte A { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public Flags Flags;
        public byte SR
        {
            get
            {
                return (byte)((Flags.Negative ? 1 : 0) << 7 |
                        (Flags.Overflow ? 1 : 0) << 6 |
                        (1 << 5) |
                        (Flags.Break ? 1 : 0) << 4 |
                        (Flags.Decimal ? 1 : 0) << 3 |
                        (Flags.DisableInterrupt ? 1 : 0) << 2 |
                        (Flags.Zero ? 1 : 0) << 1 |
                        (Flags.Carry ? 1 : 0));
            }
            set
            {
                Flags.Negative = (value & 0x80) == 0x80;
                Flags.Overflow = (value & 0x40) == 0x40;
                Flags.Break = (value & 0x10) == 0x10;
                Flags.Decimal = (value & 0x8) == 0x8;
                Flags.DisableInterrupt = (value & 0x4) == 0x4;
                Flags.Zero = (value & 0x2) == 0x2;
                Flags.Carry = (value & 0x1) == 0x1;
            }
        }

        public override IEndianness Endianness { get; }

        public State(IRunnableSystem system) : base(system)
        {
            Endianness = EndiannessFactory.Instance.Get(Enums.Endianness.LittleEndian);
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