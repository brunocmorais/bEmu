using System;
using System.IO;
using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.LR35902
{
    public class State : CPUs.Intel8080.State
    {
        public new Flags Flags;

        public new byte F
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

        public new ushort AF
        {
            get { return BitUtils.GetWordFrom2Bytes(F, A); }
            set
            {
                BitUtils.Get2BytesFromWord(value, out byte a, out byte f);
                A = a;
                F = f;
            }
        }

        public override string ToString()
        {
            return " AF = " + AF.ToString("x").PadLeft(4, '0').ToUpper() +
                   " BC = " + BC.ToString("x").PadLeft(4, '0').ToUpper() +
                   " DE = " + DE.ToString("x").PadLeft(4, '0').ToUpper() +
                   " HL = " + HL.ToString("x").PadLeft(4, '0').ToUpper() +
                   " SP = " + SP.ToString("x").PadLeft(4, '0').ToUpper() +
                   " PC = " + PC.ToString("x").PadLeft(4, '0').ToUpper(); //+
                //    " cycles = " + Cycles.ToString().PadLeft(6, '0') +
                //    " inst = "   + Instructions.ToString().PadLeft(6, '0');
        }
    }
}