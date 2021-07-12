namespace bEmu.Core.CPU.Z80
{
    public class Ports
    {
        private byte[] ports;

        public Ports(int size)
        {
            ports = new byte[size];
        }

        public byte this[byte index]
        {
            get => ports[index];
            set => ports[index] = value;
        }
    }
}