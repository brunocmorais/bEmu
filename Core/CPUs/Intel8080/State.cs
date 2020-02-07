using System;
using System.IO;
using bEmu.Core.Model;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Intel8080
{
    public class State : BaseState
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public Flags Flags;
        public Ports Ports;
        public bool EnableInterrupts { get; set; }

        public ushort BC
        {
            get { return GeneralUtils.Get16BitNumber(C, B); }
            set
            {
                GeneralUtils.WordTo2Bytes(value, out byte b, out byte c);
                B = b;
                C = c;
            }
        }

        public ushort DE
        {
            get { return GeneralUtils.Get16BitNumber(E, D); }
            set
            {
                GeneralUtils.WordTo2Bytes(value, out byte d, out byte e);
                D = d;
                E = e;
            }
        }

        public ushort HL
        {
            get { return GeneralUtils.Get16BitNumber(L, H); }
            set
            {
                GeneralUtils.WordTo2Bytes(value, out byte h, out byte l);
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
            get { return (ushort) GeneralUtils.Get16BitNumber(F, A); }
        }

        public void UpdatePorts(int number, byte value)
        {
            switch (number)
            {
                case 1:
                    Ports.Read1 = value;
                    break;
                case 2:
                    Ports.Read2 = value;
                    break;
                case 3:
                    break;
            }
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