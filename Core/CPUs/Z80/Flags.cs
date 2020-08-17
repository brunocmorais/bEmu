namespace bEmu.Core.CPUs.Z80
{
    public class Flags
    {
        public bool Sign { get; set; }
        public bool Zero { get; set; }
        public bool F5 { get; set; }
        public bool HalfCarry { get; set; }
        public bool F3 { get; set; }
        public bool ParityOrOverflow { get; set; }
        public bool Subtract { get; set; }
        public bool Carry { get; set; }
    }
}