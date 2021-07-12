namespace bEmu.Core
{
    public interface ISaveable
    {
        byte[] SaveState();
        void LoadState(byte[] bytes);
    }
}