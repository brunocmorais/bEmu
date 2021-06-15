namespace bEmu.Core
{
    public abstract class APU : IAPU
    {
        public abstract int BufferSize { get; }
        public abstract int SampleRate { get; }
        public ISystem System { get; }

        public abstract byte[] UpdateBuffer();

        public APU(ISystem system)
        {
            System = system;
        }
    }
}