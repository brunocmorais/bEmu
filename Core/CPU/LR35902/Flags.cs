namespace bEmu.Core.CPU.LR35902
{
    public struct Flags
    {
        public bool Zero { get; set; }
        public bool Subtract { get; set; }
        public bool HalfCarry { get; set; }
        public bool Carry { get; set; }
    }
}