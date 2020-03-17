using System;
using System.IO;

namespace bEmu.Core
{
    public class MMU : IMMU
    {
        byte[] ram;

        public int Length => ram.Length;

        public MMU(int size)
        {
            ram = new byte[size];
        }

        public byte this[int addr]
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

        public void LoadProgram(string fileName, int startAddress = 0)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            LoadProgram(bytes, startAddress);
        }

        public void LoadProgram(byte[] bytes, int startAddress = 0)
        {
            if ((bytes.Length + startAddress) > ram.Length)
                throw new Exception("Programa não cabe na memória!");

            for (int i = 0; i < bytes.Length; i++)
                ram[i + startAddress] = bytes[i];
        }
    }
}