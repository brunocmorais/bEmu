using System;
using System.IO;
using bEmu.Core.System;

namespace bEmu.Core.Memory
{
    public abstract class MMU : IMMU
    {
        private byte[] ram;
        public int Length => ram.Length;
        public IRunnableSystem System { get; }

        public MMU(IRunnableSystem system, int size)
        {
            ram = new byte[size];
            System = system;
            LoadProgram();
        }

        public virtual byte this[int addr]
        {
            get
            {
                ValidateRange(addr);
                return ram[addr];
            }
            set
            {
                ValidateRange(addr);
                ram[addr] = value;
            }
        }

        private void ValidateRange(int index)
        {
            if (index > ram.Length - 1)
                throw new ArgumentException($"Tentativa de acessar endereço 0x{index.ToString("x")} em uma memória de tamanho 0x{ram.LongLength.ToString("x")}.");
        }

        public virtual void LoadProgram()
        {
            if (System.ROM != null)
                LoadProgram(System.ROM.Bytes, System.StartAddress);
        }

        private void LoadProgram(byte[] bytes, int startAddress)
        {
            if ((bytes.Length + startAddress) > ram.Length)
                throw new Exception("Programa não cabe na memória!");

            for (int i = 0; i < bytes.Length; i++)
                ram[i + startAddress] = bytes[i];
        }

        public virtual byte[] SaveState()
        {
            return ram;
        }

        public virtual void LoadState(byte[] bytes)
        {
            int start = bytes.Length - this.Length;

            for (int i = 0; i < Length; i++)
                ram[i] = bytes[i + start];
        }
    }
}