namespace bEmu.Core.System
{
    public interface ISaveable
    {
        byte[] SaveState();
        void LoadState(byte[] bytes);
    }
}