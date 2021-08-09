using System;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Util;
using bEmu.Core.System;

namespace bEmu.Core.CPU.Z80
{
    public abstract partial class Z80<TState, TMMU> : CPU<TState, TMMU> 
        where TState : State
        where TMMU : MMU
    {
        const int CyclesInHalt = 12;
        
        public Z80(ISystem system, int clock) : base(system, clock) { }

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

        protected byte GetByteFromAlternateRegister(Register register)
        {
            switch (register)
            {
                case Register.A: return State.Alt.A;
                case Register.B: return State.Alt.B;
                case Register.C: return State.Alt.C;
                case Register.D: return State.Alt.D;
                case Register.E: return State.Alt.E;
                case Register.F: return State.Alt.F;
                case Register.H: return State.Alt.H;
                case Register.L: return State.Alt.L;
                case Register.AF: return ReadByteFromMemory(State.Alt.AF);
                case Register.BC: return ReadByteFromMemory(State.Alt.BC);
                case Register.DE: return ReadByteFromMemory(State.Alt.DE);
                case Register.HL: return ReadByteFromMemory(State.Alt.HL);
                default:
                    throw new Exception("Registrador desconhecido.");
            }
        }

        protected ushort GetWordFromAlternateRegister(Register register)
        {
            switch (register)
            {
                case Register.AF: return State.Alt.AF;
                case Register.BC: return State.Alt.BC;
                case Register.DE: return State.Alt.DE;
                case Register.HL: return State.Alt.HL;
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

        protected ushort GetWordFromMainRegister(Register register)
        {
            switch (register)
            {
                case Register.AF: return State.Main.AF;
                case Register.BC: return State.Main.BC;
                case Register.DE: return State.Main.DE;
                case Register.HL: return State.Main.HL;
                default:
                    throw new Exception("Registrador não permitido.");
            }
        }

        protected void SetByteToRegister(Register register, byte value)
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

        protected void SetWordToAlternateRegister(Register register, ushort value)
        {
            switch (register)
            {
                case Register.AF: State.Alt.AF = value; break;
                case Register.BC: State.Alt.BC = value; break;
                case Register.DE: State.Alt.DE = value; break;
                case Register.HL: State.Alt.HL = value; break;
                default:
                    throw new Exception("Registrador desconhecido.");
            }
        }

        protected void SetWordToMainRegister(Register register, ushort value)
        {
            switch (register)
            {
                case Register.AF: State.Main.AF = value; break;
                case Register.BC: State.Main.BC = value; break;
                case Register.DE: State.Main.DE = value; break;
                case Register.HL: State.Main.HL = value; break;
                default:
                    throw new Exception("Registrador desconhecido.");
            }
        }

        protected void SetWordToRegister(Register register, ushort value)
        {
            switch (register)
            {
                case Register.AF: State.AF = value; break;
                case Register.BC: State.BC = value; break;
                case Register.DE: State.DE = value; break;
                case Register.HL: State.HL = value; break;
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

        public abstract void HandleInterrupts();

        public override IOpcode StepCycle()
        {
            State.Instructions++;
            Opcode opcode;

            if (State.Halted)
            {
                HandleInterrupts();
                IncreaseCycles(CyclesInHalt);
                
                opcode = new Opcode(0x00);
                opcode.CyclesTaken = CyclesInHalt;

                return opcode;
            }

            opcode = new Opcode(GetNextByte());
            int cycles = State.Cycles;

            switch (opcode)
            {
                case 0x00: Nop(); break;
                case 0x10: Djnz(); break;
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
                case 0x08: ExAlt(Register.AF, Register.AF); break;
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
                case 0xE0: RetPO(); break;
                case 0xF0: RetP(); break;
                case 0xC1: Pop(Register.BC); break;
                case 0xD1: Pop(Register.DE); break;
                case 0xE1: Pop(Register.HL); break;
                case 0xF1: PopPsw(); break;
                case 0xC2: JpNZ(); break;
                case 0xD2: JpNC(); break;
                case 0xE2: Ld_C_A(); break;
                case 0xF2: Ld_A_C(); break;
                case 0xC3: Jp(); break;
                case 0xD3: Out(Register.A); break;
                case 0xE3: Ex_SP_HL(); break;
                case 0xF3: Di(); break;
                case 0xC4: CallNZ(); break;
                case 0xD4: CallNC(); break;
                case 0xE4: CallPO(); break;
                case 0xF4: CallP(); break;
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
                case 0xE8: RetPE(); break;
                case 0xF8: RetM(); break;
                case 0xC9: Ret(); break;
                case 0xD9: Exx(); break;
                case 0xE9: Jp_HL(); break;
                case 0xF9: Ld_SPHL(); break;
                case 0xCA: JpZ(); break;
                case 0xDA: JpC(); break;
                case 0xEA: JpPE(); break;
                case 0xFA: JpM(); break;
                case 0xCB: Cb(); break;
                case 0xDB: In(Register.A); break;
                case 0xEB: Ex(Register.DE, Register.HL); break;
                case 0xFB: Ei(); break;
                case 0xCC: CallZ(); break;
                case 0xDC: CallC(); break;
                case 0xEC: CallPE(); break;
                case 0xFC: CallM(); break;
                case 0xCD: Call(); break;
                case 0xDD: Dd(); break;
                case 0xED: Ed(); break;
                case 0xFD: Fd(); break;
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
        
            opcode.CyclesTaken = State.Cycles - cycles;

            return opcode;
        }
    }
}
