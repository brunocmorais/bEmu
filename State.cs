using System;
using System.IO;

namespace Intel8080
{
    public struct State
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public ushort SP { get; set; }
        public ushort PC { get; set; }
        public byte[] Memory { get; set; }
        public Flags Flags;
        public Ports Ports;
        public bool EnableInterrupts { get; set; }
        public int Cycles { get; set; }
        public bool Halted { get; set; }
        public int Instructions { get; set; }

        public ushort BC
        {
            get { return Util.Get16BitNumber(C, B); }
            set
            {
                Util.WordTo2Bytes(value, out byte b, out byte c);
                B = b;
                C = c;
            }
        }

        public ushort DE
        {
            get { return Util.Get16BitNumber(E, D); }
            set
            {
                Util.WordTo2Bytes(value, out byte d, out byte e);
                D = d;
                E = e;
            }
        }

        public ushort HL
        {
            get { return Util.Get16BitNumber(L, H); }
            set
            {
                Util.WordTo2Bytes(value, out byte h, out byte l);
                H = h;
                L = l;
            }
        }

        public byte F
        {
            get 
            {
                return (byte)((Flags.Sign ? 1 : 0) << 7 |
                        (Flags.Zero ? 1 : 0) << 6 |
                        (Flags.AuxiliaryCarry ? 1 : 0) << 4 |
                        (Flags.Parity ? 1 : 0) << 2 |
                        (1 << 1) |
                        (Flags.Carry ? 1 : 0));
            }
        }

        public ushort AF
        {
            get { return (ushort) Util.Get16BitNumber(F, A); }
        }

        public void LoadProgram(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            
            if (bytes.Length > Memory.Length)
                throw new Exception("Programa muito grande!");

            for (int i = 0; i < bytes.Length; i++)
                Memory[i] = bytes[i];
        }

        public override string ToString()
        {
            return "af = " + AF.ToString("x") + "\n" +
                   "bc = " + BC.ToString("x") + "\n" +
                   "de = " + DE.ToString("x") + "\n" +
                   "hl = " + HL.ToString("x") + "\n" +
                   "pc = " + PC.ToString("x") + "\n" +
                   "sp = " + SP.ToString("x") + "\n" +
                   "cycles = " + Cycles       + "\n" +
                   "inst = "   + Instructions + "\n" +
                   "flags = " + 
                    (Flags.Zero ? "Z" : ".") +
                    (Flags.Sign ? "S" : ".") +
                    (Flags.Parity ? "P" : ".") +
                    (Flags.AuxiliaryCarry ? "A" : ".") +
                    (Flags.Carry ? "C" : ".");
        }
    }
}