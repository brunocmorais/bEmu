using System;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.LR35902
{
    public partial class LR35902<TState, TMMU> : CPU<TState, TMMU> 
        where TState : State
        where TMMU : Core.Systems.Gameboy.MMU
    {
        public LR35902(ISystem system) : base(system) { }

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

        void SetByteToRegister(Register register, byte value)
        {
            switch (register)
            {
                case Register.A: State.A = value; break;
                case Register.B: State.B = value; break;
                case Register.C: State.C = value; break;
                case Register.D: State.D = value; break;
                case Register.E: State.E = value; break;
                case Register.H: State.H = value; break;
                case Register.L: State.L = value; break;
                case Register.AF: WriteByteToMemory(State.AF, value); break;
                case Register.BC: WriteByteToMemory(State.BC, value); break;
                case Register.DE: WriteByteToMemory(State.DE, value); break;
                case Register.HL: WriteByteToMemory(State.HL, value); break;
                case Register.SP: WriteByteToMemory(State.SP, value); break;
                case Register.PC: WriteByteToMemory(State.PC, value); break;
                default:
                    throw new Exception("Registrador desconhecido.");
            }
        }

        protected bool CheckZero(byte value)
        {
            return value == 0;
        }

        protected bool CheckHalfCarry(ushort a, ushort b, ushort result)
        {
            return (((a ^ b ^ result) & 0x10) == 0x10);
        }

        protected bool CheckHalfCarry(byte a, byte b, byte result)
        {
            return (((a ^ b ^ result) & 0x10) == 0x10);
        }

        public void HandleInterrupts()
        {
            if (!(State is bEmu.Core.Systems.Gameboy.State))
                return;

            var state = State as bEmu.Core.Systems.Gameboy.State;

            if (state.IE == 0 || state.IF == 0)
                return;

            for (int i = 0; i < 5; i++)
            {
                int mask = (0x1 << i);

                if ((state.IE & state.IF & mask) == mask)
                {
                    state.Halted = false;

                    if (!state.EnableInterrupts)
                        return;

                    state.EnableInterrupts = false;
                    Rst((ushort) (0x40 + (0x8 * i)));
                    state.IF &= (byte) ~mask;
                    break;
                }
            }
        }

        public override IOpcode StepCycle()
        {
            if (State.Halted)
            {
                HandleInterrupts();
                IncreaseCycles(16);
                return default(Opcode);
            }

            var opcode = new Opcode(GetNextByte());
            State.Instructions++;

            switch (opcode.Byte)
            {
                case 0x00: Nop(); break;
                case 0x10: Stop(); break;
                case 0x20: JrNZ(); break;
                case 0x30: JrNC(); break;
                case 0x01: LD_d16(Register.BC); break;
                case 0x11: LD_d16(Register.DE); break;
                case 0x21: LD_d16(Register.HL); break;
                case 0x31: LD_d16(Register.SP); break;
                case 0x02: LD_A(Register.BC, Action.None); break;
                case 0x12: LD_A(Register.DE, Action.None); break;
                case 0x22: LD_A(Register.HL, Action.Increment); break;
                case 0x32: LD_A(Register.HL, Action.Decrement); break;
                case 0x03: IncRegPair(Register.BC); break;
                case 0x13: IncRegPair(Register.DE); break;
                case 0x23: IncRegPair(Register.HL); break;
                case 0x33: IncRegPair(Register.SP); break;
                case 0x04: Inc(Register.B); break;
                case 0x14: Inc(Register.D); break;
                case 0x24: Inc(Register.H); break;
                case 0x34: IncRef(); break;
                case 0x05: Dec(Register.B); break;
                case 0x15: Dec(Register.D); break;
                case 0x25: Dec(Register.H); break;
                case 0x35: DecRef(); break;
                case 0x06: Ld_d8(Register.B); break;
                case 0x16: Ld_d8(Register.D); break;
                case 0x26: Ld_d8(Register.H); break;
                case 0x36: Ld_d8(Register.HL); break;
                case 0x07: Rlca(); break;
                case 0x17: Rla(); break;
                case 0x27: Daa(); break;
                case 0x37: Scf(); break;
                case 0x08: Ld_SP(); break;
                case 0x18: Jr(); break;
                case 0x28: Jrz(); break;
                case 0x38: Jrc(); break;
                case 0x09: AddHL(Register.BC); break;
                case 0x19: AddHL(Register.DE); break;
                case 0x29: AddHL(Register.HL); break;
                case 0x39: AddHL(Register.SP); break;
                case 0x0A: LdA(Register.BC, Action.None); break;
                case 0x1A: LdA(Register.DE, Action.None); break;
                case 0x2A: LdA(Register.HL, Action.Increment); break;
                case 0x3A: LdA(Register.HL, Action.Decrement); break;
                case 0x0B: DecRegPair(Register.BC); break;
                case 0x1B: DecRegPair(Register.DE); break;
                case 0x2B: DecRegPair(Register.HL); break;
                case 0x3B: DecRegPair(Register.SP); break;
                case 0x0C: Inc(Register.C); break;
                case 0x1C: Inc(Register.E); break;
                case 0x2C: Inc(Register.L); break;
                case 0x3C: Inc(Register.A); break;
                case 0x0D: Dec(Register.C); break;
                case 0x1D: Dec(Register.E); break;
                case 0x2D: Dec(Register.L); break;
                case 0x3D: Dec(Register.A); break;
                case 0x0E: Ld_d8(Register.C); break;
                case 0x1E: Ld_d8(Register.E); break;
                case 0x2E: Ld_d8(Register.L); break;
                case 0x3E: Ld_d8(Register.A); break;
                case 0x0F: Rrca(); break;
                case 0x1F: Rra(); break;
                case 0x2F: Cpl(); break;
                case 0x3F: Ccf(); break;
                case 0x40: Ld(Register.B, Register.B); break;
                case 0x41: Ld(Register.B, Register.C); break;
                case 0x42: Ld(Register.B, Register.D); break;
                case 0x43: Ld(Register.B, Register.E); break;
                case 0x44: Ld(Register.B, Register.H); break;
                case 0x45: Ld(Register.B, Register.L); break;
                case 0x46: Ld(Register.B, Register.HL); break;
                case 0x47: Ld(Register.B, Register.A); break;
                case 0x48: Ld(Register.C, Register.B); break;
                case 0x49: Ld(Register.C, Register.C); break;
                case 0x4A: Ld(Register.C, Register.D); break;
                case 0x4B: Ld(Register.C, Register.E); break;
                case 0x4C: Ld(Register.C, Register.H); break;
                case 0x4D: Ld(Register.C, Register.L); break;
                case 0x4E: Ld(Register.C, Register.HL); break;
                case 0x4F: Ld(Register.C, Register.A); break;
                case 0x50: Ld(Register.D, Register.B); break;
                case 0x51: Ld(Register.D, Register.C); break;
                case 0x52: Ld(Register.D, Register.D); break;
                case 0x53: Ld(Register.D, Register.E); break;
                case 0x54: Ld(Register.D, Register.H); break;
                case 0x55: Ld(Register.D, Register.L); break;
                case 0x56: Ld(Register.D, Register.HL); break;
                case 0x57: Ld(Register.D, Register.A); break;
                case 0x58: Ld(Register.E, Register.B); break;
                case 0x59: Ld(Register.E, Register.C); break;
                case 0x5A: Ld(Register.E, Register.D); break;
                case 0x5B: Ld(Register.E, Register.E); break;
                case 0x5C: Ld(Register.E, Register.H); break;
                case 0x5D: Ld(Register.E, Register.L); break;
                case 0x5E: Ld(Register.E, Register.HL); break;
                case 0x5F: Ld(Register.E, Register.A); break;
                case 0x60: Ld(Register.H, Register.B); break;
                case 0x61: Ld(Register.H, Register.C); break;
                case 0x62: Ld(Register.H, Register.D); break;
                case 0x63: Ld(Register.H, Register.E); break;
                case 0x64: Ld(Register.H, Register.H); break;
                case 0x65: Ld(Register.H, Register.L); break;
                case 0x66: Ld(Register.H, Register.HL); break;
                case 0x67: Ld(Register.H, Register.A); break;
                case 0x68: Ld(Register.L, Register.B); break;
                case 0x69: Ld(Register.L, Register.C); break;
                case 0x6A: Ld(Register.L, Register.D); break;
                case 0x6B: Ld(Register.L, Register.E); break;
                case 0x6C: Ld(Register.L, Register.H); break;
                case 0x6D: Ld(Register.L, Register.L); break;
                case 0x6E: Ld(Register.L, Register.HL); break;
                case 0x6F: Ld(Register.L, Register.A); break;
                case 0x70: Ld(Register.HL, Register.B); break;
                case 0x71: Ld(Register.HL, Register.C); break;
                case 0x72: Ld(Register.HL, Register.D); break;
                case 0x73: Ld(Register.HL, Register.E); break;
                case 0x74: Ld(Register.HL, Register.H); break;
                case 0x75: Ld(Register.HL, Register.L); break;
                case 0x76: Halt(); break;
                case 0x77: Ld(Register.HL, Register.A); break;
                case 0x78: Ld(Register.A, Register.B); break;
                case 0x79: Ld(Register.A, Register.C); break;
                case 0x7A: Ld(Register.A, Register.D); break;
                case 0x7B: Ld(Register.A, Register.E); break;
                case 0x7C: Ld(Register.A, Register.H); break;
                case 0x7D: Ld(Register.A, Register.L); break;
                case 0x7E: Ld(Register.A, Register.HL); break;
                case 0x7F: Ld(Register.A, Register.A); break;
                case 0x80: Add(Register.B);  break;
                case 0x81: Add(Register.C);  break;
                case 0x82: Add(Register.D);  break;
                case 0x83: Add(Register.E);  break;
                case 0x84: Add(Register.H);  break;
                case 0x85: Add(Register.L);  break;
                case 0x86: Add(Register.HL); break;
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
                case 0x98: Sbc(Register.B); break;
                case 0x99: Sbc(Register.C); break;
                case 0x9A: Sbc(Register.D); break;
                case 0x9B: Sbc(Register.E); break;
                case 0x9C: Sbc(Register.H); break;
                case 0x9D: Sbc(Register.L); break;
                case 0x9E: Sbc(Register.HL);break;
                case 0x9F: Sbc(Register.A); break;
                case 0xA0: And(Register.B); break;
                case 0xA1: And(Register.C); break;
                case 0xA2: And(Register.D); break;
                case 0xA3: And(Register.E); break;
                case 0xA4: And(Register.H); break;
                case 0xA5: And(Register.L); break;
                case 0xA6: And(Register.HL);break;
                case 0xA7: And(Register.A); break;
                case 0xA8: Xor(Register.B); break;
                case 0xA9: Xor(Register.C); break;
                case 0xAA: Xor(Register.D); break;
                case 0xAB: Xor(Register.E); break;
                case 0xAC: Xor(Register.H); break;
                case 0xAD: Xor(Register.L); break;
                case 0xAE: Xor(Register.HL);break;
                case 0xAF: Xor(Register.A); break;
                case 0xB0: Or(Register.B); break;
                case 0xB1: Or(Register.C); break;
                case 0xB2: Or(Register.D); break;
                case 0xB3: Or(Register.E); break;
                case 0xB4: Or(Register.H); break;
                case 0xB5: Or(Register.L); break;
                case 0xB6: Or(Register.HL);break;
                case 0xB7: Or(Register.A); break;
                case 0xB8: Cp(Register.B); break;
                case 0xB9: Cp(Register.C); break;
                case 0xBA: Cp(Register.D); break;
                case 0xBB: Cp(Register.E); break;
                case 0xBC: Cp(Register.H); break;
                case 0xBD: Cp(Register.L); break;
                case 0xBE: Cp(Register.HL);break;
                case 0xBF: Cp(Register.A); break;
                case 0xC0: RetNZ(); break;
                case 0xD0: RetNC(); break;
                case 0xE0: Ldh_a8_A(); break;
                case 0xF0: Ldh_A_a8(); break;
                case 0xC1: Pop(Register.BC); break;
                case 0xD1: Pop(Register.DE); break;
                case 0xE1: Pop(Register.HL); break;
                case 0xF1: PopPsw(); break;
                case 0xC2: JpNZ(); break;
                case 0xD2: JpNC(); break;
                case 0xE2: Ld_C_A(); break;
                case 0xF2: Ld_A_C(); break;
                case 0xC3: Jp(); break;
                case 0xD3: break;
                case 0xE3: break;
                case 0xF3: Di(); break;
                case 0xC4: CallNZ(); break;
                case 0xD4: CallNC(); break;
                case 0xE4: break;
                case 0xF4: break;
                case 0xC5: Push(Register.BC); break;
                case 0xD5: Push(Register.DE); break;
                case 0xE5: Push(Register.HL); break;
                case 0xF5: Push(Register.AF); break;
                case 0xC6: Add_A_d8(); break;
                case 0xD6: Sub_d8(); break;
                case 0xE6: And_d8(); break;
                case 0xF6: Or_d8(); break;
                case 0xC7: Rst(0x0); break;
                case 0xD7: Rst(0x10); break;
                case 0xE7: Rst(0x20); break;
                case 0xF7: Rst(0x30); break;
                case 0xC8: RetZ(); break;
                case 0xD8: RetC(); break;
                case 0xE8: AddSP(); break;
                case 0xF8: Ld_HL_SPr8(); break;
                case 0xC9: Ret(); break;
                case 0xD9: Reti(); break;
                case 0xE9: Jp_HL(); break;
                case 0xF9: Ld_SPHL(); break;
                case 0xCA: JpZ(); break;
                case 0xDA: JpC(); break;
                case 0xEA: Ld_a16_A(); break;
                case 0xFA: Ld_A_a16(); break;
                case 0xCB: Cb(); break;
                case 0xDB: break;
                case 0xEB: break;
                case 0xFB: Ei(); break;
                case 0xCC: CallZ(); break;
                case 0xDC: CallC(); break;
                case 0xEC: break;
                case 0xFC: break;
                case 0xCD: Call(); break;
                case 0xDD: break;
                case 0xED: break;
                case 0xFD: break;
                case 0xCE: Adc(); break;
                case 0xDE: Sbc(); break;
                case 0xEE: Xor(); break;
                case 0xFE: Cp(); break;
                case 0xCF: Rst(0x8); break;
                case 0xDF: Rst(0x18); break;
                case 0xEF: Rst(0x28); break;
                case 0xFF: Rst(0x38); break;
            }

            HandleInterrupts();

            return opcode;
        }

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
    }
}