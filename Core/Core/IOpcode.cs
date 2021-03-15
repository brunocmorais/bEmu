namespace bEmu.Core
{
    public interface IOpcode
    {
        byte Byte { get; }
        ushort UShort { get; }
        int CyclesTaken { get; set; }
    }
}