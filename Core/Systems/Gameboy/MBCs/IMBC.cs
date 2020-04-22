namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public interface IMBC
    {
        void LoadProgram(byte[] bytes);
        byte ReadCartRAM(int addr);
        byte ReadROM(int addr);
        void SetMode(int addr, byte value);
        void WriteCartRAM(int addr, byte value);
        void Shutdown();
    }
}