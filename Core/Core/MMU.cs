using System;
using System.IO;

namespace bEmu.Core
{
    public abstract class MMU : IMMU
    {
        private byte[] ram;
        public int Length => ram.Length;
        public ISystem System { get; }

        public MMU(ISystem system, int size)
        {
            ram = new byte[size];
            System = system;
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

        public virtual void LoadProgram(int startAddress = 0)
        {
            byte[] bytes = File.ReadAllBytes(System.FileName);
            LoadProgram(bytes, startAddress);
        }

        public virtual void LoadProgram(byte[] bytes, int startAddress = 0)
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