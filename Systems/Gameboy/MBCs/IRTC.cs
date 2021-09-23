namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IRTC : IMBC
    {
        void Tick(int lastCycleCount);
        void Shutdown();
    }
}