using System;
using System.IO;

namespace bEmu.Core
{
    public class MMU : IMMU
    {
        public ISystem System { get; }
        byte[] memory;

        public MMU(long size)
        {
            memory = new byte[size];
        }

        public byte this[long index]
        {
            get
            {
                if (index > memory.LongLength - 1)
                    throw new ArgumentException($"Tentativa de ler endereço 0x{index.ToString("x")} em uma memória de tamanho 0x{memory.LongLength.ToString("x")}.");

                return memory[index];
            }
            set
            {
                if (index > memory.LongLength - 1)
                    throw new ArgumentException($"Tentativa de gravar no endereço 0x{index.ToString("x")} em uma memória de tamanho {memory.LongLength.ToString("x")}.");

                memory[index] = value;
            }
        }

        public void LoadProgram(string fileName, int startAddress)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            LoadProgram(bytes, startAddress);
        }

        public void LoadProgram(byte[] bytes, int startAddress)
        {
            if ((bytes.Length + startAddress) > memory.Length)
                throw new Exception("Programa não cabe na memória!");

            for (int i = 0; i < bytes.Length; i++)
                memory[i + startAddress] = bytes[i];
        }
    }
}