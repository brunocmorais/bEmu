using System;
using System.IO;

namespace bEmu.Core.Model
{
    public abstract class BaseState : IState
    {
        public byte[] Memory { get; set; }
        public ushort PC { get; set; }
        public ushort SP { get; set; }
        public int Cycles { get; set; }
        public bool Halted { get; set; }
        public int Instructions { get; set; }

        public void LoadProgram(string fileName, int position)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            LoadProgram(bytes, position);
        }

        public void LoadProgram(byte[] bytes, int position)
        {
            if ((bytes.Length + position) > Memory.Length)
                throw new Exception("Programa não cabe na memória!");

            for (int i = 0; i < bytes.Length; i++)
                Memory[i + position] = bytes[i];
        }
    }
}