namespace bEmu.Intel8080
{
    public struct Flags
    {
        public bool Zero { get; set; }
        public bool Sign { get; set; }
        public bool Parity { get; set; }
        public bool Carry { get; set; }
        public bool AuxiliaryCarry { get; set; }
    }
}