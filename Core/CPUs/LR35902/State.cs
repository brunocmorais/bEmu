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
            return " af = " + AF.ToString("x").PadLeft(4, '0') +
                   " bc = " + BC.ToString("x").PadLeft(4, '0') +
                   " de = " + DE.ToString("x").PadLeft(4, '0') +
                   " hl = " + HL.ToString("x").PadLeft(4, '0') +
                   " pc = " + PC.ToString("x").PadLeft(4, '0') +
                   " sp = " + SP.ToString("x").PadLeft(4, '0') +
                   " cycles = " + Cycles.ToString().PadLeft(6, '0') +
                   " inst = "   + Instructions.ToString().PadLeft(6, '0');
        }

        public byte IE
        {
            get { return System.MMU[0xFFFF]; }
            set { System.MMU[0xFFFF] = value; }
        }
        public byte IF
        {
            get { return System.MMU[0xFF0F]; }
            set { System.MMU[0xFF0F] = value; }
        }

        public byte SCY
        {
            get { return System.MMU[0xFF42]; }
            set { System.MMU[0xFF42] = value; }
        }

        public byte SCX
        {
            get { return System.MMU[0xFF43]; }
            set { System.MMU[0xFF43] = value; }
        }

        public byte Joypad
        {
            get { return System.MMU[0xFF00]; }
            set { System.MMU[0xFF00] = value; }
        }

        public void EnableInterrupt(InterruptType type)
        {
            IE |= (byte) (0x1 << (int) type);
        }

        public void RequestInterrupt(InterruptType type)
        {
            IF |= (byte) (0x1 << (int) type);
        }
    }

    public enum InterruptType
    {
        VBlank = 0,
        LcdStat = 1,
        Timer = 2,
        Serial = 3,
        Joypad = 4
    }
}