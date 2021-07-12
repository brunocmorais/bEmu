namespace bEmu.Core.Audio
{
    public interface IAPU
    {
        ISystem System { get; }
        byte[] Buffer { get; }
        double Time { get; }

        void UpdateBuffer();
        void Update(int cycles);
    }
}