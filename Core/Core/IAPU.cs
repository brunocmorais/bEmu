namespace bEmu.Core
{
    public interface IAPU
    {
        int BufferSize { get; }
        int SampleRate { get; }
        ISystem System { get; }
        byte[] Buffer { get; }
        double Time { get; }

        void UpdateBuffer();
        void Update(int cycles);
    }
}