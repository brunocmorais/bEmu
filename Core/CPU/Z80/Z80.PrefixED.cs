using bEmu.Core.Memory;
using bEmu.Core.CPU;

namespace bEmu.Core.CPU.Z80
{
    public abstract partial class Z80<TState, TMMU>
    {
        public void Ed()
        {
            switch (GetNextByte())
            {
                case 0x40: In(Register.C, Register.B); break;
                case 0x41: Out(Register.B, Register.C); break;
                case 0x42: Sbc(Register.HL, Register.BC); break;
                case 0x43: Ld_d16(Register.BC); break;
                case 0x44: break;
                case 0x45: break;
                case 0x46: break;
                case 0x47: break;
                case 0x48: In(Register.C, Register.C); break;
                case 0x49: Out(Register.C, Register.C); break;
                case 0x4A: break;
                case 0x4B: Ld_Reg_d16(Register.BC); break;
                case 0x4C: break;
                case 0x4D: break;
                case 0x4E: break;
                case 0x4F: break;
                case 0x50: In(Register.C, Register.D); break;
                case 0x51: Out(Register.D, Register.C); break;
                case 0x52: Sbc(Register.HL, Register.DE); break;
                case 0x53: Ld_d16(Register.DE); break;
                case 0x54: break;
                case 0x55: break;
                case 0x56: break;
                case 0x57: break;
                case 0x58: In(Register.C, Register.E); break;
                case 0x59: Out(Register.E, Register.C); break;
                case 0x5A: break;
                case 0x5B: Ld_Reg_d16(Register.DE); break;
                case 0x5C: break;
                case 0x5D: break;
                case 0x5E: break;
                case 0x5F: break;
                case 0x60: In(Register.C, Register.H); break;
                case 0x61: Out(Register.H, Register.C); break;
                case 0x62: Sbc(Register.HL, Register.HL); break;
                case 0x63: Ld_d16(Register.HL); break;
                case 0x64: break;
                case 0x65: break;
                case 0x66: break;
                case 0x67: break;
                case 0x68: In(Register.C, Register.L); break;
                case 0x69: Out(Register.L, Register.C); break;
                case 0x6A: break;
                case 0x6B: Ld_Reg_d16(Register.HL); break;
                case 0x6C: break;
                case 0x6D: break;
                case 0x6E: break;
                case 0x6F: break;
                case 0x70: In(Register.C); break;
                case 0x71: Out(Register.C); break;
                case 0x72: Sbc(Register.HL, Register.SP); break;
                case 0x73: Ld_d16(Register.SP); break;
                case 0x74: break;
                case 0x75: break;
                case 0x76: break;
                case 0x77: break;
                case 0x78: In(Register.C, Register.A); break;
                case 0x79: Out(Register.A, Register.C); break;
                case 0x7A: break;
                case 0x7B: Ld_Reg_d16(Register.SP); break;
                case 0x7C: break;
                case 0x7D: break;
                case 0x7E: break;
                case 0x7F: break;
                case 0xA0: break;
                case 0xA1: break;
                case 0xA2: break;
                case 0xA3: break;
                case 0xA4: break;
                case 0xA5: break;
                case 0xA6: break;
                case 0xA7: break;
                case 0xA8: break;
                case 0xA9: break;
                case 0xAA: break;
                case 0xAB: break;
                case 0xAC: break;
                case 0xAD: break;
                case 0xAE: break;
                case 0xAF: break;
                case 0xB0: break;
                case 0xB1: break;
                case 0xB2: break;
                case 0xB3: break;
                case 0xB4: break;
                case 0xB5: break;
                case 0xB6: break;
                case 0xB7: break;
                case 0xB8: break;
                case 0xB9: break;
                case 0xBA: break;
                case 0xBB: break;
                case 0xBC: break;
                case 0xBD: break;
                case 0xBE: break;
                case 0xBF: break;                
            }
        }

        protected void In(Register registerPort, Register? register = null)
        {
            var port = GetByteFromRegister(registerPort);
            var value = State.Ports[port];

            if (register.HasValue)
                SetByteToRegister(register.Value, value);
            
            State.Flags.Zero = CheckZero(value);
            State.Flags.HalfCarry = false;
            State.Flags.Sign = CheckNegative(value); 
            State.Flags.Subtract = false;
            State.Flags.ParityOrOverflow = CheckParity(value);

            IncreaseCycles(12);
        }

        protected void Out(Register registerPort, Register? register = null)
        {
            var port = GetByteFromRegister(registerPort);
            State.Ports[port] = register.HasValue ? GetByteFromRegister(register.Value) : (byte) 0;
            IncreaseCycles(12);
        }

        protected void Sbc(Register a, Register b)
        {
            ushort value = GetWordFromRegister(a);
            ushort sub = GetWordFromRegister(b);
            int carry = State.Flags.Carry ? 1 : 0;
            ushort result = (ushort) (value - sub - carry); 

            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarry(sub, value, (byte) result);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;

            IncreaseCycles(15);
        }

        protected void Ld_d16(Register register)
        {
            var address = GetNextWord();
            ushort value = GetWordFromRegister(register);
            
            MMU[address] = (byte) (value & 0xFF);
            MMU[address + 1] = (byte) ((value >> 8) & 0xFF);

            IncreaseCycles(20);
        }

        protected void Ld_Reg_d16(Register register)
        {
            var value = GetNextWord();
            var address = GetWordFromRegister(register);
            
            MMU[address] = (byte) (value & 0xFF);
            MMU[address + 1] = (byte) ((value >> 8) & 0xFF);

            IncreaseCycles(20);
        }

        protected void Neg()
        {
            
        }
    }
}