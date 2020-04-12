namespace bEmu.Core.Systems
{
    public interface IMBC
    {
        void LoadProgram(byte[] bytes);
        byte ReadCartRAM(ushort addr);
        byte ReadROM(ushort addr);
        void SetMode(ushort addr, byte value);
        void WriteCartRAM(ushort addr, byte value);
    }
}