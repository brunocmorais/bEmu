namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IHasRTC
    {
        void Tick(int lastCycleCount);
    }
}