namespace bEmu.Core.Audio
{
    public abstract class APU : IAPU
    {
        public const int SampleRate = 22050;
        public const int BufferSize = 2048;
        public const int MaxBufferPending = 4;
        public ISystem System { get; }
        public byte[] Buffer { get; }
        public double Time { get; protected set; }

        public abstract void UpdateBuffer();
        public abstract void Update(int cycles);

        public APU(ISystem system)
        {
            System = system;
            Buffer = new byte[BufferSize];
        }
    }
}