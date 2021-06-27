namespace bEmu.Core
{
    public abstract class APU : IAPU
    {
        public int BufferSize => 2048;
        public int SampleRate => 22050;
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