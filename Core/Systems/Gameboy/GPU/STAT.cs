namespace bEmu.Core.Systems.Gameboy.GPU
{
    public enum STAT
    {
        CoincidenceFlag = 0x4,
        Mode0HBlankInterrupt = 0x8,
        Mode1VBlankInterrupt = 0x10,
        Mode2OAMInterrupt = 0x20,
        LYCoincidenceInterrupt = 0x40
    }
}