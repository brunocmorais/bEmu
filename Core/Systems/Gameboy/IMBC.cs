namespace bEmu.Core.Systems
{
    public interface IMBC
    {
        void LoadProgram(byte[] bytes);
        byte ReadCartRAM(int addr);
        byte ReadROM(int addr);
        void SetMode(int addr, byte value);
        void WriteCartRAM(int addr, byte value);
        void Tick(int lastCycleCount);
    }
}