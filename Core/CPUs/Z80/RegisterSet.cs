using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Z80
{
    public class RegisterSet
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public Flags Flags = new Flags();
        
        public ushort BC
        {
            get { return BitUtils.GetWordFrom2Bytes(C, B); }
            set
            {
                BitUtils.Get2BytesFromWord(value, out byte b, out byte c);
                B = b;
                C = c;
            }
        }

        public ushort DE
        {
            get { return BitUtils.GetWordFrom2Bytes(E, D); }
            set
            {
                BitUtils.Get2BytesFromWord(value, out byte d, out byte e);
                D = d;
                E = e;
            }
        }

        public ushort HL
        {
            get { return BitUtils.GetWordFrom2Bytes(L, H); }
            set
            {
                BitUtils.Get2BytesFromWord(value, out byte h, out byte l);
                H = h;
                L = l;
            }
        }

        public ushort AF
        {
            get { return BitUtils.GetWordFrom2Bytes(F, A); }
            set
            {
                BitUtils.Get2BytesFromWord(value, out byte a, out byte f);
                A = a;
                F = f;
            }
        }

        public byte F
        {
            get 
            {
                return (byte)((Flags.Sign             ? 1 : 0) << 7 |
                              (Flags.Zero             ? 1 : 0) << 6 |
                              (Flags.F5               ? 1 : 0) << 5 |
                              (Flags.HalfCarry        ? 1 : 0) << 4 |
                              (Flags.F3               ? 1 : 0) << 3 |
                              (Flags.ParityOrOverflow ? 1 : 0) << 2 |
                              (Flags.Subtract         ? 1 : 0) << 1 |
                              (Flags.Carry            ? 1 : 0) << 0);
            }
            set
            {
                Flags.Sign             = (value & 0x80) == 0x80;
                Flags.Zero             = (value & 0x40) == 0x40;
                Flags.F5               = (value & 0x20) == 0x20;
                Flags.HalfCarry        = (value & 0x10) == 0x10;
                Flags.F3               = (value & 0x8)  == 0x8;
                Flags.ParityOrOverflow = (value & 0x4)  == 0x4;
                Flags.Subtract         = (value & 0x2)  == 0x2;
                Flags.Carry            = (value & 0x1)  == 0x1;
            }
        }
    }
}