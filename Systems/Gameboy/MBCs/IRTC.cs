namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IRTC
    {
        void Tick(int lastCycleCount);
        void Shutdown();
    }
}