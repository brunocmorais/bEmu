namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IRAM : IMBC
    {
        byte ReadCartRAM(int addr);
        void WriteCartRAM(int addr, byte value);
    }
}