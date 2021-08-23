using System;
using bEmu.Core.Util;
using bEmu.Core.System;

namespace bEmu.Core.CPU.LR35902
{
    public class State : Core.System.State<ushort, ushort>
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }
        public Flags Flags;
        public bool EnableInterrupts { get; set; }

        public State(IRunnableSystem system) : base(system) { }
        
        public ushort BC
        {
            get { return LittleEndian.GetWordFrom2Bytes(C, B); }
            set
            {
                LittleEndian.Get2BytesFromWord(value, out byte b, out byte c);
                B = b;
                C = c;
            }
        }

        public ushort DE
        {
            get { return LittleEndian.GetWordFrom2Bytes(E, D); }
            set
            {
                LittleEndian.Get2BytesFromWord(value, out byte d, out byte e);
                D = d;
                E = e;
            }
        }

        public ushort HL
        {
            get { return LittleEndian.GetWordFrom2Bytes(L, H); }
            set
            {
                LittleEndian.Get2BytesFromWord(value, out byte h, out byte l);
                H = h;
                L = l;
            }
        }

        public ushort AF
        {
            get { return LittleEndian.GetWordFrom2Bytes(F, A); }
            set
            {
                LittleEndian.Get2BytesFromWord(value, out byte a, out byte f);
                A = a;
                F = f;
            }
        }

        public byte F
        {
            get 
            {
                return (byte)((Flags.Zero ? 1 : 0) << 7 |
                        (Flags.Subtract ? 1 : 0) << 6 |
                        (Flags.HalfCarry ? 1 : 0) << 5 |
                        (Flags.Carry ? 1 : 0) << 4);
            }
            set
            {
                Flags.Zero = (value & 0x80) == 0x80;
                Flags.Subtract = (value & 0x40) == 0x40;
                Flags.HalfCarry = (value & 0x20) == 0x20;
                Flags.Carry = (value & 0x10) == 0x10;
            }
        }

        public override byte[] SaveState()
        {
            throw new NotImplementedException();
        }

        public override void LoadState(byte[] value)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "\n AF = " + AF.ToString("x").PadLeft(4, '0').ToUpper() +
                   "\n BC = " + BC.ToString("x").PadLeft(4, '0').ToUpper() +
                   "\n DE = " + DE.ToString("x").PadLeft(4, '0').ToUpper() +
                   "\n HL = " + HL.ToString("x").PadLeft(4, '0').ToUpper() +
                   "\n SP = " + SP.ToString("x").PadLeft(4, '0').ToUpper() +
                   "\n PC = " + PC.ToString("x").PadLeft(4, '0').ToUpper();
        }

        public override void Reset()
        {
            base.Reset();
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
            F = 0;
            H = 0;
            L = 0;
            Flags = new Flags();
            EnableInterrupts = false;
        }
    }
}