using System;
using System.Linq;
using System.Text;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Intel8080
{
    public partial class Intel8080<TState> : CPU<TState> where TState : State
    {
        public Intel8080(ISystem system) : base(system) { }

        protected ushort GetNextWord()
        {
            byte b1 = MMU[State.PC++];
            byte b2 = MMU[State.PC++];
            return BitUtils.GetWordFrom2Bytes(b1, b2);
        }

        public byte GetNextByte()
        {
            return MMU[State.PC++];
        }

        protected byte ReadByteFromMemory(ushort addr)
        {
            return MMU[addr];
        }

        protected void WriteByteToMemory(ushort addr, byte value)
        {
            MMU[addr] = value;
        }

        protected ushort ReadWordFromMemory(ushort addr)
        {
            byte a = MMU[addr];
            byte b = MMU[addr + 1];
            return BitUtils.GetWordFrom2Bytes(a, b);
        }

        protected void WriteWordToMemory(ushort addr, ushort word)
        {
            BitUtils.Get2BytesFromWord(word, out byte a, out byte b);
            MMU[addr] = b;
            MMU[addr + 1] = a;
        }

        private void UpdateFlags(byte value)
        {
            State.Flags.Zero = CheckZero(value);
            State.Flags.Sign = CheckSign(value);
            State.Flags.Parity = CheckParity(value);
            State.Flags.AuxiliaryCarry = false;
        }

        protected ushort PopStack()
        {
            ushort word = ReadWordFromMemory(State.SP);
            State.SP += 2;
            return word;
        }

        protected void PushStack(ushort value)
        {
            State.SP -= 2;
            WriteWordToMemory(State.SP, value);
        }

        protected byte GetByteFromRegister(Register register)
        {
            switch (register)
            {
                case Register.A: return State.A;
                case Register.B: return State.B;
                case Register.C: return State.C;
                case Register.D: return State.D;
                case Register.E: return State.E;
                case Register.H: return State.H;
                case Register.L: return State.L;
                case Register.AF: return ReadByteFromMemory(State.AF);
                case Register.BC: return ReadByteFromMemory(State.BC);
                case Register.DE: return ReadByteFromMemory(State.DE);
                case Register.HL: return ReadByteFromMemory(State.HL);
                case Register.SP: return ReadByteFromMemory(State.SP);
                case Register.PC: return ReadByteFromMemory(State.PC);
                default:
                    throw new Exception("Registrador desconhecido.");
            }
        }

        protected ushort GetWordFromRegister(Register register)
        {
            switch (register)
            {
                case Register.AF: return State.AF;
                case Register.BC: return State.BC;
                case Register.DE: return State.DE;
                case Register.HL: return State.HL;
                case Register.SP: return State.SP;
                case Register.PC: return State.PC;
                default:
                    throw new Exception("Registrador não permitido.");
            }
        }

        public void GenerateInterrupt(int interruptNumber)
        {
            if (State.EnableInterrupts)
            {
                PushStack(State.PC);
                Di();
                State.PC = (ushort) (8 * interruptNumber);
            }
        }

        protected bool CheckZero(byte value)
        {
            return value == 0;
        }

        protected bool CheckSign(byte value)
        {
            return (value & 0x80) == 0x80;
        }

        protected bool CheckParity(byte value)
        {
            byte numberOfOneBits = 0;

            for (int i = 0; i < 8; i++) 
                numberOfOneBits += (byte)((value >> i) & 1);

            return (numberOfOneBits & 1) == 0;
        }

        protected bool CheckAuxiliaryCarryAdd(params byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] &= 0x0F;

            return bytes.Sum(x => x) >= 0x10;
        }

        protected bool CheckAuxiliaryCarryAdd(params ushort[] words)
        {
            for (int i = 0; i < words.Length; i++)
                words[i] &= 0x0FFF;

            return words.Sum(x => x) >= 0x1000;
        }

        public override IOpcode StepCycle()
        {
            var opcode = new Opcode(GetNextByte());
            base.StepCycle();

            switch (opcode.Byte)
            {
                case 0x00:
                case 0x10:
                case 0x20:
                case 0x30: Nop(); break;
                case 0x01: Lxi(Register.BC); break;
                case 0x11: Lxi(Register.DE); break;
                case 0x21: Lxi(Register.HL); break;
                case 0x31: Lxi(Register.SP); break;
                case 0x02: Stax(Register.BC); break;
                case 0x12: Stax(Register.DE); break;
                case 0x03: Inx(Register.BC); break;
                case 0x13: Inx(Register.DE); break;
                case 0x23: Inx(Register.HL);break;
                case 0x33: Inx(Register.SP); break;
                case 0x04: Inr(Register.B); break;
                case 0x14: Inr(Register.D); break;
                case 0x24: Inr(Register.H); break;
                case 0x34: Inr(Register.HL); break;
                case 0x05: Dcr(Register.B); break;
                case 0x15: Dcr(Register.D); break;
                case 0x25: Dcr(Register.H); break;
                case 0x35: Dcr(Register.HL); break;
                case 0x06: Mvi(Register.B); break;
                case 0x16: Mvi(Register.D); break;
                case 0x26: Mvi(Register.H); break;
                case 0x36: Mvi(Register.HL); break;
                case 0x07: Rlc(); break;
                case 0x08: Nop(); break;
                case 0x18: Nop(); break;
                case 0x28: Nop(); break;
                case 0x38: Nop(); break;
                case 0x09: Dad(Register.BC); break;
                case 0x19: Dad(Register.DE); break;
                case 0x29: Dad(Register.HL); break;
                case 0x39: Dad(Register.SP); break;
                case 0x0A: Ldax(Register.BC); break;
                case 0x1A: Ldax(Register.DE); break;
                case 0x0B: Dcx(Register.BC); break;
                case 0x1B: Dcx(Register.DE); break;
                case 0x2B: Dcx(Register.HL); break;
                case 0x3B: Dcx(Register.SP); break;
                case 0x0C: Inr(Register.C); break;
                case 0x1C: Inr(Register.E); break;
                case 0x2C: Inr(Register.L); break;
                case 0x3C: Inr(Register.A); break;
                case 0x0D: Dcr(Register.C); break;
                case 0x1D: Dcr(Register.E); break;
                case 0x2D: Dcr(Register.L); break;
                case 0x3D: Dcr(Register.A); break;
                case 0x0E: Mvi(Register.C); break;
                case 0x1E: Mvi(Register.E); break;
                case 0x2E: Mvi(Register.L); break;
                case 0x3E: Mvi(Register.A); break;
                case 0x0F: Rrc(); break;
                case 0x17: Ral(); break;
                case 0x1F: Rar(); break;
                case 0x22: Shld(); break;
                case 0x27: Daa(); break;
                case 0x2A: Lhld(); break;
                case 0x2F: Cma(); break;
                case 0x32: Sta(); break;
                case 0x37: Stc(); break;
                case 0x3A: Lda(); break;
                case 0x3F: Cmc(); break;
                case 0x40: Mov(Register.B, Register.B); break;
                case 0x41: Mov(Register.B, Register.C); break;
                case 0x42: Mov(Register.B, Register.D); break;
                case 0x43: Mov(Register.B, Register.E); break;
                case 0x44: Mov(Register.B, Register.H); break;
                case 0x45: Mov(Register.B, Register.L); break;
                case 0x46: Mov(Register.B, Register.HL); break;
                case 0x47: Mov(Register.B, Register.A); break;
                case 0x48: Mov(Register.C, Register.B); break;
                case 0x49: Mov(Register.C, Register.C); break;
                case 0x4A: Mov(Register.C, Register.D); break;
                case 0x4B: Mov(Register.C, Register.E); break;
                case 0x4C: Mov(Register.C, Register.H); break;
                case 0x4D: Mov(Register.C, Register.L); break;
                case 0x4E: Mov(Register.C, Register.HL); break;
                case 0x4F: Mov(Register.C, Register.A); break;
                case 0x50: Mov(Register.D, Register.B); break;
                case 0x51: Mov(Register.D, Register.C); break;
                case 0x52: Mov(Register.D, Register.D); break;
                case 0x53: Mov(Register.D, Register.E); break;
                case 0x54: Mov(Register.D, Register.H); break;
                case 0x55: Mov(Register.D, Register.L); break;
                case 0x56: Mov(Register.D, Register.HL); break;
                case 0x57: Mov(Register.D, Register.A); break;
                case 0x58: Mov(Register.E, Register.B); break;
                case 0x59: Mov(Register.E, Register.C); break;
                case 0x5A: Mov(Register.E, Register.D); break;
                case 0x5B: Mov(Register.E, Register.E); break;
                case 0x5C: Mov(Register.E, Register.H); break;
                case 0x5D: Mov(Register.E, Register.L); break;
                case 0x5E: Mov(Register.E, Register.HL); break;
                case 0x5F: Mov(Register.E, Register.A); break;
                case 0x60: Mov(Register.H, Register.B); break;
                case 0x61: Mov(Register.H, Register.C); break;
                case 0x62: Mov(Register.H, Register.D); break;
                case 0x63: Mov(Register.H, Register.E); break;
                case 0x64: Mov(Register.H, Register.H); break;
                case 0x65: Mov(Register.H, Register.L); break;
                case 0x66: Mov(Register.H, Register.HL); break;
                case 0x67: Mov(Register.H, Register.A); break;
                case 0x68: Mov(Register.L, Register.B); break;
                case 0x69: Mov(Register.L, Register.C); break;
                case 0x6A: Mov(Register.L, Register.D); break;
                case 0x6B: Mov(Register.L, Register.E); break;
                case 0x6C: Mov(Register.L, Register.H); break;
                case 0x6D: Mov(Register.L, Register.L); break;
                case 0x6E: Mov(Register.L, Register.HL); break;
                case 0x6F: Mov(Register.L, Register.A); break;
                case 0x70: Mov(Register.HL, Register.B); break;
                case 0x71: Mov(Register.HL, Register.C); break;
                case 0x72: Mov(Register.HL, Register.D); break;
                case 0x73: Mov(Register.HL, Register.E); break;
                case 0x74: Mov(Register.HL, Register.H); break;
                case 0x75: Mov(Register.HL, Register.L); break;
                case 0x76: Hlt(); break;
                case 0x77: Mov(Register.HL, Register.A); break;
                case 0x78: Mov(Register.A, Register.B); break;
                case 0x79: Mov(Register.A, Register.C); break;
                case 0x7A: Mov(Register.A, Register.D); break;
                case 0x7B: Mov(Register.A, Register.E); break;
                case 0x7C: Mov(Register.A, Register.H); break;
                case 0x7D: Mov(Register.A, Register.L); break;
                case 0x7E: Mov(Register.A, Register.HL); break;
                case 0x7F: Mov(Register.A, Register.A); break;
                case 0x80: Add(Register.B);  break;
                case 0x81: Add(Register.C);  break;
                case 0x82: Add(Register.D);  break;
                case 0x83: Add(Register.E);  break;
                case 0x84: Add(Register.H);  break;
                case 0x85: Add(Register.L);  break;
                case 0x86: Add(Register.HL);  break;
                case 0x87: Add(Register.A);  break;
                case 0x88: Adc(Register.B); break;
                case 0x89: Adc(Register.C); break;
                case 0x8A: Adc(Register.D); break;
                case 0x8B: Adc(Register.E); break;
                case 0x8C: Adc(Register.H); break;
                case 0x8D: Adc(Register.L); break;
                case 0x8E: Adc(Register.HL);break;
                case 0x8F: Adc(Register.A); break;
                case 0x90: Sub(Register.B); break;
                case 0x91: Sub(Register.C); break;
                case 0x92: Sub(Register.D); break;
                case 0x93: Sub(Register.E); break;
                case 0x94: Sub(Register.H); break;
                case 0x95: Sub(Register.L); break;
                case 0x96: Sub(Register.HL);break;
                case 0x97: Sub(Register.A); break;
                case 0x98: Sbb(Register.B); break;
                case 0x99: Sbb(Register.C); break;
                case 0x9A: Sbb(Register.D); break;
                case 0x9B: Sbb(Register.E); break;
                case 0x9C: Sbb(Register.H); break;
                case 0x9D: Sbb(Register.L); break;
                case 0x9E: Sbb(Register.HL);break;
                case 0x9F: Sbb(Register.A); break;
                case 0xA0: Ana(Register.B); break;
                case 0xA1: Ana(Register.C); break;
                case 0xA2: Ana(Register.D); break;
                case 0xA3: Ana(Register.E); break;
                case 0xA4: Ana(Register.H); break;
                case 0xA5: Ana(Register.L); break;
                case 0xA6: Ana(Register.HL);break;
                case 0xA7: Ana(Register.A); break;
                case 0xA8: Xra(Register.B); break;
                case 0xA9: Xra(Register.C); break;
                case 0xAA: Xra(Register.D); break;
                case 0xAB: Xra(Register.E); break;
                case 0xAC: Xra(Register.H); break;
                case 0xAD: Xra(Register.L); break;
                case 0xAE: Xra(Register.HL);break;
                case 0xAF: Xra(Register.A); break;
                case 0xB0: Ora(Register.B); break;
                case 0xB1: Ora(Register.C); break;
                case 0xB2: Ora(Register.D); break;
                case 0xB3: Ora(Register.E); break;
                case 0xB4: Ora(Register.H); break;
                case 0xB5: Ora(Register.L); break;
                case 0xB6: Ora(Register.HL);break;
                case 0xB7: Ora(Register.A); break;
                case 0xB8: Cmp(Register.B); break;
                case 0xB9: Cmp(Register.C); break;
                case 0xBA: Cmp(Register.D); break;
                case 0xBB: Cmp(Register.E); break;
                case 0xBC: Cmp(Register.H); break;
                case 0xBD: Cmp(Register.L); break;
                case 0xBE: Cmp(Register.HL);break;
                case 0xBF: Cmp(Register.A); break;
                case 0xC0: Rnz(); break;
                case 0xD0: Rnc(); break;
                case 0xE0: Rpo(); break;
                case 0xF0: Rp(); break;
                case 0xC1: Pop(Register.BC); break;
                case 0xD1: Pop(Register.DE); break;
                case 0xE1: Pop(Register.HL); break;
                case 0xF1: PopPsw(); break;
                case 0xC2: Jnz(); break;
                case 0xD2: Jnc(); break;
                case 0xE2: Jpo(); break;
                case 0xF2: Jp(); break;
                case 0xC3: Jmp(); break;
                case 0xC4: Cnz(); break;
                case 0xD4: Cnc(); break;
                case 0xE4: Cpo(); break;
                case 0xF4: Cp(); break;
                case 0xC5: Push(Register.BC); break;
                case 0xD5: Push(Register.DE); break;
                case 0xE5: Push(Register.HL); break;
                case 0xF5: Push(Register.AF); break;
                case 0xC6: Adi(); break;
                case 0xD6: Sui(); break;
                case 0xE6: Ani(); break;
                case 0xF6: Ori(); break;
                case 0xC7: Rst(0); break;
                case 0xD7: Rst(2); break;
                case 0xE7: Rst(4); break;
                case 0xF7: Rst(6); break;
                case 0xC8: Rz(); break;
                case 0xD8: Rc(); break;
                case 0xE8: Rpe(); break;
                case 0xF8: Rm(); break;
                case 0xC9: Ret(); break;
                case 0xD9: Ret(); break;
                case 0xCA: Jz(); break;
                case 0xDA: Jc(); break;
                case 0xEA: Jpe(); break;
                case 0xFA: Jm(); break;
                case 0xCB: Jmp(); break;
                case 0xCC: Cz(); break;
                case 0xDC: Cc(); break;
                case 0xEC: Cpe(); break;
                case 0xFC: Cm(); break;
                case 0xCD: Call(); break;
                case 0xDD: Call(); break;
                case 0xED: Call(); break;
                case 0xFD: Call(); break;
                case 0xCE: Aci(); break;
                case 0xDE: Sbi(); break;
                case 0xEE: Xri(); break;
                case 0xFE: Cpi(); break;
                case 0xCF: Rst(1); break;
                case 0xDF: Rst(3); break;
                case 0xEF: Rst(5); break;
                case 0xFF: Rst(7); break;
                case 0xD3: Out(); break;
                case 0xDB: In(); break;
                case 0xE3: Xthl(); break;
                case 0xE9: Pchl(); break;
                case 0xEB: Xchg(); break;
                case 0xF3: Di(); break;
                case 0xF9: Sphl(); break;
                case 0xFB: Ei(); break;
            }

            return opcode;
        }

        public string CallDiagnosticsRoutine()
        {
            if (State.PC == 5)
            {
                if (State.C == 9)
                {
                    var sb = new StringBuilder();
                    ushort offset = BitUtils.GetWordFrom2Bytes(State.E, State.D);
                    offset += 3;
                    char c = Convert.ToChar(MMU[offset]);

                    while (c != '$')
                    {
                        sb.Append(c);
                        c = Convert.ToChar(MMU[++offset]);
                    }

                    Hlt();

                    return sb.ToString();
                }
            }

            return null;
        }
    }
}