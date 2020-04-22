using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Intel8080
{
    public class State : Core.State
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
            get { return BitUtils.GetWordFrom2Bytes(F, A); }
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
    }
}