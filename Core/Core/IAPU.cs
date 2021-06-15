namespace bEmu.Core
{
    public interface IAPU
    {
        int BufferSize { get; }
        int SampleRate { get; }
        ISystem System { get; }
        byte[] UpdateBuffer();
    }
}