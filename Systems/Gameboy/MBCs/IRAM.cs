namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IRAM
    {
        byte ReadCartRAM(int addr);
        void WriteCartRAM(int addr, byte value);
    }
}