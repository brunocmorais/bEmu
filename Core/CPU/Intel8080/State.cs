using System.Collections.Generic;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU.Intel8080
{
    public class State : Core.System.State
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
        
        public State(ISystem system) : base(system) { }
        
        public ushort BC
        {
            get { return ByteOperations.GetWordFrom2Bytes(C, B); }
            set
            {
                ByteOperations.Get2BytesFromWord(value, out byte b, out byte c);
                B = b;
                C = c;
            }
        }

        public ushort DE
        {
            get { return ByteOperations.GetWordFrom2Bytes(E, D); }
            set
            {
                ByteOperations.Get2BytesFromWord(value, out byte d, out byte e);
                D = d;
                E = e;
            }
        }

        public ushort HL
        {
            get { return ByteOperations.GetWordFrom2Bytes(L, H); }
            set
            {
                ByteOperations.Get2BytesFromWord(value, out byte h, out byte l);
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
            get { return ByteOperations.GetWordFrom2Bytes(F, A); }
        }

        public override byte[] SaveState()
        {
            throw new global::System.NotImplementedException();
        }

        public override void LoadState(byte[] value)
        {
            throw new global::System.NotImplementedException();
        }

        public override string ToString()
        {
            return "af = " + AF.ToString("x") + "\n" +
                   "bc = " + BC.ToString("x") + "\n" +
                   "de = " + DE.ToString("x") + "\n" +
                   "hl = " + HL.ToString("x") + "\n" +
                   "pc = " + PC.ToString("x") + "\n" +
                   "sp = " + SP.ToString("x") + "\n";
        }

        public override void Reset()
        {
            base.Reset();
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
            H = 0;
            L = 0;
            Flags = new Flags();
            EnableInterrupts = false;
        }
    }
}