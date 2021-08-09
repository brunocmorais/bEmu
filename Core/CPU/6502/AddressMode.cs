namespace bEmu.Core.CPU.MOS6502
{
    public enum AddressMode
    {
        Accumulator = 0,
        Absolute = 1,
        AbsoluteX = 2,
        AbsoluteY = 3,
        Immediate = 4,
        Implied = 5,
        Indirect = 6,
        XIndirect = 7,
        IndirectY = 8,
        Relative = 9,
        ZeroPage = 10,
        ZeroPageX = 11,
        ZeroPageY = 12
    }
}