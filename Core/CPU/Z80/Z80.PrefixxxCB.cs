using bEmu.Core.Memory;
using bEmu.Core.CPU;

namespace bEmu.Core.CPU.Z80
{
    public abstract partial class Z80<TState, TMMU>
    {
        public void XxCb(Indexer indexer)
        {
            ushort address = GetAddress(indexer, (sbyte) GetNextByte());

            switch (GetNextByte())
            {
                case 0x00: Rlc(address, Register.B); break;
                case 0x01: Rlc(address, Register.C); break;
                case 0x02: Rlc(address, Register.D); break;
                case 0x03: Rlc(address, Register.E); break;
                case 0x04: Rlc(address, Register.H); break;
                case 0x05: Rlc(address, Register.L); break;
                case 0x06: Rlc(address); break;
                case 0x07: Rlc(address, Register.A); break;
                case 0x08: Rrc(address, Register.B); break;
                case 0x09: Rrc(address, Register.C); break;
                case 0x0A: Rrc(address, Register.D); break;
                case 0x0B: Rrc(address, Register.E); break;
                case 0x0C: Rrc(address, Register.H); break;
                case 0x0D: Rrc(address, Register.L); break;
                case 0x0E: Rrc(address); break;
                case 0x0F: Rrc(address, Register.A); break;
                case 0x10: Rl(address, Register.B); break;
                case 0x11: Rl(address, Register.C); break;
                case 0x12: Rl(address, Register.D); break;
                case 0x13: Rl(address, Register.E); break;
                case 0x14: Rl(address, Register.H); break;
                case 0x15: Rl(address, Register.L); break;
                case 0x16: Rl(address); break;
                case 0x17: Rl(address, Register.A); break;
                case 0x18: Rr(address, Register.B); break;
                case 0x19: Rr(address, Register.C); break;
                case 0x1A: Rr(address, Register.D); break;
                case 0x1B: Rr(address, Register.E); break;
                case 0x1C: Rr(address, Register.H); break;
                case 0x1D: Rr(address, Register.L); break;
                case 0x1E: Rr(address); break;
                case 0x1F: Rr(address, Register.A); break;
                case 0x20: Sla(address, Register.B); break;
                case 0x21: Sla(address, Register.C); break;
                case 0x22: Sla(address, Register.D); break;
                case 0x23: Sla(address, Register.E); break;
                case 0x24: Sla(address, Register.H); break;
                case 0x25: Sla(address, Register.L); break;
                case 0x26: Sla(address); break;
                case 0x27: Sla(address, Register.A); break;
                case 0x28: Sra(address, Register.B); break;
                case 0x29: Sra(address, Register.C); break;
                case 0x2A: Sra(address, Register.D); break;
                case 0x2B: Sra(address, Register.E); break;
                case 0x2C: Sra(address, Register.H); break;
                case 0x2D: Sra(address, Register.L); break;
                case 0x2E: Sra(address); break;
                case 0x2F: Sra(address, Register.A); break;
                case 0x30: Sll(address, Register.B); break;
                case 0x31: Sll(address, Register.C); break;
                case 0x32: Sll(address, Register.D); break;
                case 0x33: Sll(address, Register.E); break;
                case 0x34: Sll(address, Register.H); break;
                case 0x35: Sll(address, Register.L); break;
                case 0x36: Sll(address); break;
                case 0x37: Sll(address, Register.A); break;
                case 0x38: Srl(address, Register.B); break;
                case 0x39: Srl(address, Register.C); break;
                case 0x3A: Srl(address, Register.D); break;
                case 0x3B: Srl(address, Register.E); break;
                case 0x3C: Srl(address, Register.H); break;
                case 0x3D: Srl(address, Register.L); break;
                case 0x3E: Srl(address); break;
                case 0x3F: Srl(address, Register.A); break;
                case 0x40: Bit(0, address); break;
                case 0x41: Bit(0, address); break;
                case 0x42: Bit(0, address); break;
                case 0x43: Bit(0, address); break;
                case 0x44: Bit(0, address); break;
                case 0x45: Bit(0, address); break;
                case 0x46: Bit(0, address); break;
                case 0x47: Bit(0, address); break;
                case 0x48: Bit(1, address); break;
                case 0x49: Bit(1, address); break;
                case 0x4A: Bit(1, address); break;
                case 0x4B: Bit(1, address); break;
                case 0x4C: Bit(1, address); break;
                case 0x4D: Bit(1, address); break;
                case 0x4E: Bit(1, address); break;
                case 0x4F: Bit(1, address); break;
                case 0x50: Bit(2, address); break;
                case 0x51: Bit(2, address); break;
                case 0x52: Bit(2, address); break;
                case 0x53: Bit(2, address); break;
                case 0x54: Bit(2, address); break;
                case 0x55: Bit(2, address); break;
                case 0x56: Bit(2, address); break;
                case 0x57: Bit(2, address); break;
                case 0x58: Bit(3, address); break;
                case 0x59: Bit(3, address); break;
                case 0x5A: Bit(3, address); break;
                case 0x5B: Bit(3, address); break;
                case 0x5C: Bit(3, address); break;
                case 0x5D: Bit(3, address); break;
                case 0x5E: Bit(3, address); break;
                case 0x5F: Bit(3, address); break;
                case 0x60: Bit(4, address); break;
                case 0x61: Bit(4, address); break;
                case 0x62: Bit(4, address); break;
                case 0x63: Bit(4, address); break;
                case 0x64: Bit(4, address); break;
                case 0x65: Bit(4, address); break;
                case 0x66: Bit(4, address); break;
                case 0x67: Bit(4, address); break;
                case 0x68: Bit(5, address); break;
                case 0x69: Bit(5, address); break;
                case 0x6A: Bit(5, address); break;
                case 0x6B: Bit(5, address); break;
                case 0x6C: Bit(5, address); break;
                case 0x6D: Bit(5, address); break;
                case 0x6E: Bit(5, address); break;
                case 0x6F: Bit(5, address); break;
                case 0x70: Bit(6, address); break;
                case 0x71: Bit(6, address); break;
                case 0x72: Bit(6, address); break;
                case 0x73: Bit(6, address); break;
                case 0x74: Bit(6, address); break;
                case 0x75: Bit(6, address); break;
                case 0x76: Bit(6, address); break;
                case 0x77: Bit(6, address); break;
                case 0x78: Bit(7, address); break;
                case 0x79: Bit(7, address); break;
                case 0x7A: Bit(7, address); break;
                case 0x7B: Bit(7, address); break;
                case 0x7C: Bit(7, address); break;
                case 0x7D: Bit(7, address); break;
                case 0x7E: Bit(7, address); break;
                case 0x7F: Bit(7, address); break;
                case 0x80: Res(0, address); break;
                case 0x81: Res(0, address); break;
                case 0x82: Res(0, address); break;
                case 0x83: Res(0, address); break;
                case 0x84: Res(0, address); break;
                case 0x85: Res(0, address); break;
                case 0x86: Res(0, address); break;
                case 0x87: Res(0, address); break;
                case 0x88: Res(1, address); break;
                case 0x89: Res(1, address); break;
                case 0x8A: Res(1, address); break;
                case 0x8B: Res(1, address); break;
                case 0x8C: Res(1, address); break;
                case 0x8D: Res(1, address); break;
                case 0x8E: Res(1, address); break;
                case 0x8F: Res(1, address); break;
                case 0x90: Res(2, address); break;
                case 0x91: Res(2, address); break;
                case 0x92: Res(2, address); break;
                case 0x93: Res(2, address); break;
                case 0x94: Res(2, address); break;
                case 0x95: Res(2, address); break;
                case 0x96: Res(2, address); break;
                case 0x97: Res(2, address); break;
                case 0x98: Res(3, address); break;
                case 0x99: Res(3, address); break;
                case 0x9A: Res(3, address); break;
                case 0x9B: Res(3, address); break;
                case 0x9C: Res(3, address); break;
                case 0x9D: Res(3, address); break;
                case 0x9E: Res(3, address); break;
                case 0x9F: Res(3, address); break;
                case 0xA0: Res(4, address); break;
                case 0xA1: Res(4, address); break;
                case 0xA2: Res(4, address); break;
                case 0xA3: Res(4, address); break;
                case 0xA4: Res(4, address); break;
                case 0xA5: Res(4, address); break;
                case 0xA6: Res(4, address); break;
                case 0xA7: Res(4, address); break;
                case 0xA8: Res(5, address); break;
                case 0xA9: Res(5, address); break;
                case 0xAA: Res(5, address); break;
                case 0xAB: Res(5, address); break;
                case 0xAC: Res(5, address); break;
                case 0xAD: Res(5, address); break;
                case 0xAE: Res(5, address); break;
                case 0xAF: Res(5, address); break;
                case 0xB0: Res(6, address); break;
                case 0xB1: Res(6, address); break;
                case 0xB2: Res(6, address); break;
                case 0xB3: Res(6, address); break;
                case 0xB4: Res(6, address); break;
                case 0xB5: Res(6, address); break;
                case 0xB6: Res(6, address); break;
                case 0xB7: Res(6, address); break;
                case 0xB8: Res(7, address); break;
                case 0xB9: Res(7, address); break;
                case 0xBA: Res(7, address); break;
                case 0xBB: Res(7, address); break;
                case 0xBC: Res(7, address); break;
                case 0xBD: Res(7, address); break;
                case 0xBE: Res(7, address); break;
                case 0xBF: Res(7, address); break;
                case 0xC0: Set(0, address); break;
                case 0xC1: Set(0, address); break;
                case 0xC2: Set(0, address); break;
                case 0xC3: Set(0, address); break;
                case 0xC4: Set(0, address); break;
                case 0xC5: Set(0, address); break;
                case 0xC6: Set(0, address); break;
                case 0xC7: Set(0, address); break;
                case 0xC8: Set(1, address); break;
                case 0xC9: Set(1, address); break;
                case 0xCA: Set(1, address); break;
                case 0xCB: Set(1, address); break;
                case 0xCC: Set(1, address); break;
                case 0xCD: Set(1, address); break;
                case 0xCE: Set(1, address); break;
                case 0xCF: Set(1, address); break;
                case 0xD0: Set(2, address); break;
                case 0xD1: Set(2, address); break;
                case 0xD2: Set(2, address); break;
                case 0xD3: Set(2, address); break;
                case 0xD4: Set(2, address); break;
                case 0xD5: Set(2, address); break;
                case 0xD6: Set(2, address); break;
                case 0xD7: Set(2, address); break;
                case 0xD8: Set(3, address); break;
                case 0xD9: Set(3, address); break;
                case 0xDA: Set(3, address); break;
                case 0xDB: Set(3, address); break;
                case 0xDC: Set(3, address); break;
                case 0xDD: Set(3, address); break;
                case 0xDE: Set(3, address); break;
                case 0xDF: Set(3, address); break;
                case 0xE0: Set(4, address); break;
                case 0xE1: Set(4, address); break;
                case 0xE2: Set(4, address); break;
                case 0xE3: Set(4, address); break;
                case 0xE4: Set(4, address); break;
                case 0xE5: Set(4, address); break;
                case 0xE6: Set(4, address); break;
                case 0xE7: Set(4, address); break;
                case 0xE8: Set(5, address); break;
                case 0xE9: Set(5, address); break;
                case 0xEA: Set(5, address); break;
                case 0xEB: Set(5, address); break;
                case 0xEC: Set(5, address); break;
                case 0xED: Set(5, address); break;
                case 0xEE: Set(5, address); break;
                case 0xEF: Set(5, address); break;
                case 0xF0: Set(6, address); break;
                case 0xF1: Set(6, address); break;
                case 0xF2: Set(6, address); break;
                case 0xF3: Set(6, address); break;
                case 0xF4: Set(6, address); break;
                case 0xF5: Set(6, address); break;
                case 0xF6: Set(6, address); break;
                case 0xF7: Set(6, address); break;
                case 0xF8: Set(7, address); break;
                case 0xF9: Set(7, address); break;
                case 0xFA: Set(7, address); break;
                case 0xFB: Set(7, address); break;
                case 0xFC: Set(7, address); break;
                case 0xFD: Set(7, address); break;
                case 0xFE: Set(7, address); break;
                case 0xFF: Set(7, address); break;
            }

            HandleInterrupts();
        }

        protected void Rlc(ushort address, Register? register = null)
        {
            byte value = MMU[address];
            State.Flags.Carry = (value & 0x80) == 0x80;
            value <<= 1;

            if (State.Flags.Carry)
                value |= 1;

            State.Flags.Zero = CheckZero(value);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            if (register.HasValue)
                SetByteToRegister(register.Value, value);
            else
                MMU[address] = value;

            IncreaseCycles(8);
        }

        protected void Rrc(ushort address, Register? register = null)
        {
            byte value = MMU[address];
            State.Flags.Carry = (value & 1) == 1;
            value >>= 1;

            if (State.Flags.Carry)
                value |= 0x80;

            State.Flags.Zero = CheckZero(value);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            
            if (register.HasValue)
                SetByteToRegister(register.Value, value);
            else
                MMU[address] = value;

            IncreaseCycles(8);
        }

        protected void Rl(ushort address, Register? register = null)
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((MMU[address] & 0x80) >> 7) == 1;

            if (register.HasValue)
            {
                SetByteToRegister(register.Value, (byte) (MMU[address] << 1));

                if (previousCarry)
                    SetByteToRegister(register.Value, (byte) (MMU[address] | 1));
            }
            else
            {
                MMU[address] = (byte) (MMU[address] << 1);

                if (previousCarry)
                    MMU[address] = (byte) (MMU[address] | 1);
            }

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Rr(ushort address, Register? register = null)
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((MMU[address] & 0x1)) == 1;

            if (register.HasValue)
            {
                SetByteToRegister(register.Value, (byte) (MMU[address] >> 1));

                if (previousCarry)
                    SetByteToRegister(register.Value, (byte) (MMU[address] | 0x80));
            }
            else
            {
                MMU[address] = (byte) (MMU[address] >> 1);

                if (previousCarry)
                    MMU[address] = (byte) (MMU[address] | 0x80);
            }

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Sla(ushort address, Register? register = null)
        {
            State.Flags.Carry = ((MMU[address] & 0x80) >> 7) == 1;

            if (register.HasValue)
                SetByteToRegister(register.Value, (byte) (MMU[address] << 1));
            else
                MMU[address] = (byte) (MMU[address] << 1);

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Sra(ushort address, Register? register = null)
        {
            byte value = MMU[address];
            byte bit7 = (byte) (value >> 7); 
            State.Flags.Carry = (value & 0x1) == 1;

            if (register.HasValue)
                SetByteToRegister(register.Value, (byte) ((value >> 1) | (bit7 << 7)));
            else
                MMU[address] = (byte) ((value >> 1) | (bit7 << 7));

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Sll(ushort address, Register? register = null)
        {
            byte value = MMU[address];
            State.Flags.Carry = (value & 0x80) == 0x80;

            if (register.HasValue)
                SetByteToRegister(register.Value, (byte) ((value << 1) | 1));
            else
                MMU[address] = (byte) ((value << 1) | 1);

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Srl(ushort address, Register? register = null)
        {
            byte value = MMU[address];
            State.Flags.Carry = (value & 0x1) == 1;

            if (register.HasValue)
                SetByteToRegister(register.Value, (byte) (value >> 1));
            else
                MMU[address] = (byte) (value >> 1);

            State.Flags.Zero = CheckZero(MMU[address]);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(8);
        }

        protected void Bit(int bitNumber, ushort address)
        {
            byte value = MMU[address];
            byte mask = (byte) (0x1 << bitNumber);
            bool bit = (value & mask) == mask;
            State.Flags.Zero = !bit;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = true;

            IncreaseCycles(8);
        }

        protected void Set(int bitNumber, ushort address)
        {
            byte value = MMU[address];
            value |= (byte) (0x1 << bitNumber);
            MMU[address] = value;

            IncreaseCycles(8);
        }

        protected void Res(int bitNumber, ushort address)
        {
            byte value = MMU[address];
            value &= (byte) ~(0x1 << bitNumber);
            MMU[address] = value;

            IncreaseCycles(8);
        }
    }
}