namespace bEmu.Core.CPU.MOS6502
{
    public struct Flags
    {
        public bool Negative { get; set; }
        public bool Overflow { get; set; }
        public bool Break { get; set; }
        public bool Decimal { get; set; }
        public bool Interrupt { get; set; }
        public bool Zero { get; set; }
        public bool Carry { get; set; }
    }
}