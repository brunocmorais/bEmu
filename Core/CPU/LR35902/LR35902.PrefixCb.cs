using bEmu.Core.Memory;
using bEmu.Core.CPU;

namespace bEmu.Core.CPU.LR35902
{
    public abstract partial class LR35902<TState, TMMU>
    {
        public void Cb()
        {
            switch (GetNextByte())
            {
                case 0x00: Rlc(Register.B); break;
                case 0x01: Rlc(Register.C); break;
                case 0x02: Rlc(Register.D); break;
                case 0x03: Rlc(Register.E); break;
                case 0x04: Rlc(Register.H); break;
                case 0x05: Rlc(Register.L); break;
                case 0x06: Rlc(Register.HL); break;
                case 0x07: Rlc(Register.A); break;
                case 0x08: Rrc(Register.B); break;
                case 0x09: Rrc(Register.C); break;
                case 0x0A: Rrc(Register.D); break;
                case 0x0B: Rrc(Register.E); break;
                case 0x0C: Rrc(Register.H); break;
                case 0x0D: Rrc(Register.L); break;
                case 0x0E: Rrc(Register.HL); break;
                case 0x0F: Rrc(Register.A); break;
                case 0x10: Rl(Register.B); break;
                case 0x11: Rl(Register.C); break;
                case 0x12: Rl(Register.D); break;
                case 0x13: Rl(Register.E); break;
                case 0x14: Rl(Register.H); break;
                case 0x15: Rl(Register.L); break;
                case 0x16: Rl(Register.HL); break;
                case 0x17: Rl(Register.A); break;
                case 0x18: Rr(Register.B); break;
                case 0x19: Rr(Register.C); break;
                case 0x1A: Rr(Register.D); break;
                case 0x1B: Rr(Register.E); break;
                case 0x1C: Rr(Register.H); break;
                case 0x1D: Rr(Register.L); break;
                case 0x1E: Rr(Register.HL); break;
                case 0x1F: Rr(Register.A); break;
                case 0x20: Sla(Register.B); break;
                case 0x21: Sla(Register.C); break;
                case 0x22: Sla(Register.D); break;
                case 0x23: Sla(Register.E); break;
                case 0x24: Sla(Register.H); break;
                case 0x25: Sla(Register.L); break;
                case 0x26: Sla(Register.HL); break;
                case 0x27: Sla(Register.A); break;
                case 0x28: Sra(Register.B); break;
                case 0x29: Sra(Register.C); break;
                case 0x2A: Sra(Register.D); break;
                case 0x2B: Sra(Register.E); break;
                case 0x2C: Sra(Register.H); break;
                case 0x2D: Sra(Register.L); break;
                case 0x2E: Sra(Register.HL); break;
                case 0x2F: Sra(Register.A); break;
                case 0x30: Swap(Register.B); break;
                case 0x31: Swap(Register.C); break;
                case 0x32: Swap(Register.D); break;
                case 0x33: Swap(Register.E); break;
                case 0x34: Swap(Register.H); break;
                case 0x35: Swap(Register.L); break;
                case 0x36: Swap(Register.HL); break;
                case 0x37: Swap(Register.A); break;
                case 0x38: Srl(Register.B); break;
                case 0x39: Srl(Register.C); break;
                case 0x3A: Srl(Register.D); break;
                case 0x3B: Srl(Register.E); break;
                case 0x3C: Srl(Register.H); break;
                case 0x3D: Srl(Register.L); break;
                case 0x3E: Srl(Register.HL); break;
                case 0x3F: Srl(Register.A); break;
                case 0x40: Bit(0, Register.B);  break;
                case 0x41: Bit(0, Register.C);  break;
                case 0x42: Bit(0, Register.D);  break;
                case 0x43: Bit(0, Register.E);  break;
                case 0x44: Bit(0, Register.H);  break;
                case 0x45: Bit(0, Register.L);  break;
                case 0x46: Bit(0, Register.HL); break;
                case 0x47: Bit(0, Register.A);  break;
                case 0x48: Bit(1, Register.B);  break;
                case 0x49: Bit(1, Register.C);  break;
                case 0x4A: Bit(1, Register.D);  break;
                case 0x4B: Bit(1, Register.E);  break;
                case 0x4C: Bit(1, Register.H);  break;
                case 0x4D: Bit(1, Register.L);  break;
                case 0x4E: Bit(1, Register.HL); break;
                case 0x4F: Bit(1, Register.A);  break;
                case 0x50: Bit(2, Register.B);  break;
                case 0x51: Bit(2, Register.C);  break;
                case 0x52: Bit(2, Register.D);  break;
                case 0x53: Bit(2, Register.E);  break;
                case 0x54: Bit(2, Register.H);  break;
                case 0x55: Bit(2, Register.L);  break;
                case 0x56: Bit(2, Register.HL); break;
                case 0x57: Bit(2, Register.A);  break;
                case 0x58: Bit(3, Register.B);  break;
                case 0x59: Bit(3, Register.C);  break;
                case 0x5A: Bit(3, Register.D);  break;
                case 0x5B: Bit(3, Register.E);  break;
                case 0x5C: Bit(3, Register.H);  break;
                case 0x5D: Bit(3, Register.L);  break;
                case 0x5E: Bit(3, Register.HL); break;
                case 0x5F: Bit(3, Register.A);  break;
                case 0x60: Bit(4, Register.B);  break;
                case 0x61: Bit(4, Register.C);  break;
                case 0x62: Bit(4, Register.D);  break;
                case 0x63: Bit(4, Register.E);  break;
                case 0x64: Bit(4, Register.H);  break;
                case 0x65: Bit(4, Register.L);  break;
                case 0x66: Bit(4, Register.HL); break;
                case 0x67: Bit(4, Register.A);  break;
                case 0x68: Bit(5, Register.B);  break;
                case 0x69: Bit(5, Register.C);  break;
                case 0x6A: Bit(5, Register.D);  break;
                case 0x6B: Bit(5, Register.E);  break;
                case 0x6C: Bit(5, Register.H);  break;
                case 0x6D: Bit(5, Register.L);  break;
                case 0x6E: Bit(5, Register.HL); break;
                case 0x6F: Bit(5, Register.A);  break;
                case 0x70: Bit(6, Register.B);  break;
                case 0x71: Bit(6, Register.C);  break;
                case 0x72: Bit(6, Register.D);  break;
                case 0x73: Bit(6, Register.E);  break;
                case 0x74: Bit(6, Register.H);  break;
                case 0x75: Bit(6, Register.L);  break;
                case 0x76: Bit(6, Register.HL); break;
                case 0x77: Bit(6, Register.A);  break;
                case 0x78: Bit(7, Register.B);  break;
                case 0x79: Bit(7, Register.C);  break;
                case 0x7A: Bit(7, Register.D);  break;
                case 0x7B: Bit(7, Register.E);  break;
                case 0x7C: Bit(7, Register.H);  break;
                case 0x7D: Bit(7, Register.L);  break;
                case 0x7E: Bit(7, Register.HL); break;
                case 0x7F: Bit(7, Register.A);  break;
                case 0x80: Res(0, Register.B);  break;
                case 0x81: Res(0, Register.C);  break;
                case 0x82: Res(0, Register.D);  break;
                case 0x83: Res(0, Register.E);  break;
                case 0x84: Res(0, Register.H);  break;
                case 0x85: Res(0, Register.L);  break;
                case 0x86: Res(0, Register.HL); break;
                case 0x87: Res(0, Register.A);  break;
                case 0x88: Res(1, Register.B);  break;
                case 0x89: Res(1, Register.C);  break;
                case 0x8A: Res(1, Register.D);  break;
                case 0x8B: Res(1, Register.E);  break;
                case 0x8C: Res(1, Register.H);  break;
                case 0x8D: Res(1, Register.L);  break;
                case 0x8E: Res(1, Register.HL); break;
                case 0x8F: Res(1, Register.A);  break;
                case 0x90: Res(2, Register.B);  break;
                case 0x91: Res(2, Register.C);  break;
                case 0x92: Res(2, Register.D);  break;
                case 0x93: Res(2, Register.E);  break;
                case 0x94: Res(2, Register.H);  break;
                case 0x95: Res(2, Register.L);  break;
                case 0x96: Res(2, Register.HL); break;
                case 0x97: Res(2, Register.A);  break;
                case 0x98: Res(3, Register.B);  break;
                case 0x99: Res(3, Register.C);  break;
                case 0x9A: Res(3, Register.D);  break;
                case 0x9B: Res(3, Register.E);  break;
                case 0x9C: Res(3, Register.H);  break;
                case 0x9D: Res(3, Register.L);  break;
                case 0x9E: Res(3, Register.HL); break;
                case 0x9F: Res(3, Register.A);  break;
                case 0xA0: Res(4, Register.B);  break;
                case 0xA1: Res(4, Register.C);  break;
                case 0xA2: Res(4, Register.D);  break;
                case 0xA3: Res(4, Register.E);  break;
                case 0xA4: Res(4, Register.H);  break;
                case 0xA5: Res(4, Register.L);  break;
                case 0xA6: Res(4, Register.HL); break;
                case 0xA7: Res(4, Register.A);  break;
                case 0xA8: Res(5, Register.B);  break;
                case 0xA9: Res(5, Register.C);  break;
                case 0xAA: Res(5, Register.D);  break;
                case 0xAB: Res(5, Register.E);  break;
                case 0xAC: Res(5, Register.H);  break;
                case 0xAD: Res(5, Register.L);  break;
                case 0xAE: Res(5, Register.HL); break;
                case 0xAF: Res(5, Register.A);  break;
                case 0xB0: Res(6, Register.B);  break;
                case 0xB1: Res(6, Register.C);  break;
                case 0xB2: Res(6, Register.D);  break;
                case 0xB3: Res(6, Register.E);  break;
                case 0xB4: Res(6, Register.H);  break;
                case 0xB5: Res(6, Register.L);  break;
                case 0xB6: Res(6, Register.HL); break;
                case 0xB7: Res(6, Register.A);  break;
                case 0xB8: Res(7, Register.B);  break;
                case 0xB9: Res(7, Register.C);  break;
                case 0xBA: Res(7, Register.D);  break;
                case 0xBB: Res(7, Register.E);  break;
                case 0xBC: Res(7, Register.H);  break;
                case 0xBD: Res(7, Register.L);  break;
                case 0xBE: Res(7, Register.HL); break;
                case 0xBF: Res(7, Register.A);  break;
                case 0xC0: Set(0, Register.B);  break;
                case 0xC1: Set(0, Register.C);  break;
                case 0xC2: Set(0, Register.D);  break;
                case 0xC3: Set(0, Register.E);  break;
                case 0xC4: Set(0, Register.H);  break;
                case 0xC5: Set(0, Register.L);  break;
                case 0xC6: Set(0, Register.HL); break;
                case 0xC7: Set(0, Register.A);  break;
                case 0xC8: Set(1, Register.B);  break;
                case 0xC9: Set(1, Register.C);  break;
                case 0xCA: Set(1, Register.D);  break;
                case 0xCB: Set(1, Register.E);  break;
                case 0xCC: Set(1, Register.H);  break;
                case 0xCD: Set(1, Register.L);  break;
                case 0xCE: Set(1, Register.HL); break;
                case 0xCF: Set(1, Register.A);  break;
                case 0xD0: Set(2, Register.B);  break;
                case 0xD1: Set(2, Register.C);  break;
                case 0xD2: Set(2, Register.D);  break;
                case 0xD3: Set(2, Register.E);  break;
                case 0xD4: Set(2, Register.H);  break;
                case 0xD5: Set(2, Register.L);  break;
                case 0xD6: Set(2, Register.HL); break;
                case 0xD7: Set(2, Register.A);  break;
                case 0xD8: Set(3, Register.B);  break;
                case 0xD9: Set(3, Register.C);  break;
                case 0xDA: Set(3, Register.D);  break;
                case 0xDB: Set(3, Register.E);  break;
                case 0xDC: Set(3, Register.H);  break;
                case 0xDD: Set(3, Register.L);  break;
                case 0xDE: Set(3, Register.HL); break;
                case 0xDF: Set(3, Register.A);  break;
                case 0xE0: Set(4, Register.B);  break;
                case 0xE1: Set(4, Register.C);  break;
                case 0xE2: Set(4, Register.D);  break;
                case 0xE3: Set(4, Register.E);  break;
                case 0xE4: Set(4, Register.H);  break;
                case 0xE5: Set(4, Register.L);  break;
                case 0xE6: Set(4, Register.HL); break;
                case 0xE7: Set(4, Register.A);  break;
                case 0xE8: Set(5, Register.B);  break;
                case 0xE9: Set(5, Register.C);  break;
                case 0xEA: Set(5, Register.D);  break;
                case 0xEB: Set(5, Register.E);  break;
                case 0xEC: Set(5, Register.H);  break;
                case 0xED: Set(5, Register.L);  break;
                case 0xEE: Set(5, Register.HL); break;
                case 0xEF: Set(5, Register.A);  break;
                case 0xF0: Set(6, Register.B);  break;
                case 0xF1: Set(6, Register.C);  break;
                case 0xF2: Set(6, Register.D);  break;
                case 0xF3: Set(6, Register.E);  break;
                case 0xF4: Set(6, Register.H);  break;
                case 0xF5: Set(6, Register.L);  break;
                case 0xF6: Set(6, Register.HL); break;
                case 0xF7: Set(6, Register.A);  break;
                case 0xF8: Set(7, Register.B);  break;
                case 0xF9: Set(7, Register.C);  break;
                case 0xFA: Set(7, Register.D);  break;
                case 0xFB: Set(7, Register.E);  break;
                case 0xFC: Set(7, Register.H);  break;
                case 0xFD: Set(7, Register.L);  break;
                case 0xFE: Set(7, Register.HL); break;
                case 0xFF: Set(7, Register.A);  break;
            }

            HandleInterrupts();
        }

        protected void Rlc(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);
            State.Flags.Carry = (value & 0x80) == 0x80;
            value <<= 1;

            if (State.Flags.Carry)
                value |= 1;

            State.Flags.Zero = CheckZero(value);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            SetByteToRegister(register, value);

            IncreaseCycles(8);
        }

        protected void Rrc(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);
            State.Flags.Carry = (value & 1) == 1;
            value >>= 1;

            if (State.Flags.Carry)
                value |= 0x80;

            State.Flags.Zero = CheckZero(value);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            SetByteToRegister(register, value);

            IncreaseCycles(8);
        }

        protected void Rl(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x80) >> 7) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) << 1));

            if (previousCarry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Rr(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x1)) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) >> 1));

            if (previousCarry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 0x80));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Sla(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            State.Flags.Carry = ((GetByteFromRegister(register) & 0x80) >> 7) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) << 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Sra(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);
            byte bit7 = (byte) (value >> 7); 
            State.Flags.Carry = (value & 0x1) == 1;
            SetByteToRegister(register, (byte) ((value >> 1) | (bit7 << 7)));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Swap(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);

            byte msb = (byte) ((value & 0xF0) >> 4);
            byte lsb = (byte) ((value & 0x0F));
            byte result = (byte) ((lsb << 4) | msb);

            SetByteToRegister(register, result);

            State.Flags.Zero = CheckZero(result);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            State.Flags.Carry = false;

            IncreaseCycles(8);
        }

        protected void Srl(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);
            State.Flags.Carry = (value & 0x1) == 1;
            SetByteToRegister(register, (byte) (value >> 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Bit(int bitNumber, Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(8);

            byte value = GetByteFromRegister(register);
            byte mask = (byte) (0x1 << bitNumber);
            bool bit = (value & mask) == mask;
            State.Flags.Zero = !bit;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = true;

            IncreaseCycles(8);
        }

        protected void Set(int bitNumber, Register register)
        {
            byte value = GetByteFromRegister(register);
            value |= (byte) (0x1 << bitNumber);

            SetByteToRegister(register, value);

            if (register == Register.HL)
                IncreaseCycles(8);

            IncreaseCycles(8);
        }

        protected void Res(int bitNumber, Register register)
        {
            byte value = GetByteFromRegister(register);
            value &= (byte) ~(0x1 << bitNumber);
            SetByteToRegister(register, value);

            if (register == Register.HL)
                IncreaseCycles(8);

            IncreaseCycles(8);
        }
    }
}