namespace bEmu.Core.System
{
    public interface ISaveStateSystem
    {
        string SaveStateName { get; }
        bool LoadState();
        void SaveState();
    }
}