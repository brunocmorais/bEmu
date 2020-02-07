using System;
using System.Diagnostics;
using bEmu.Core.Model;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Intel8080
{
     partial class Intel8080 : ICPU
    {
       void Nop()
        {
            state.Cycles += 4;
        }

       void Lxi(Register register)
        {
            switch (register)
            {
                case Register.BC:
                    state.C = state.Memory[state.PC++];
                    state.B = state.Memory[state.PC++];
                    break;
                case Register.DE:
                    state.E = state.Memory[state.PC++];
                    state.D = state.Memory[state.PC++];
                    break;
                case Register.HL:
                    state.L = state.Memory[state.PC++];
                    state.H = state.Memory[state.PC++];
                    break;
                case Register.SP:
                    state.SP = GeneralUtils.Get16BitNumber(state.Memory[state.PC++], state.Memory[state.PC++]);
                    break;
            }
            
            state.Cycles += 10;
        }

       void Stax(Register register)
        {
            if (register == Register.BC)
                state.Memory[state.BC] = state.A;
            else if (register == Register.DE)
                state.Memory[state.DE] = state.A;

            state.Cycles += 7;
        }

       void Shld()
        {
            ushort value = state.HL;
            ushort addr = GetNextWord();
            WriteWordToMemory(addr, value);
            state.Cycles += 16;
        }

       void Sta()
        {
            WriteByteToMemory(GetNextWord(), state.A);
            state.Cycles += 13;
        }

       void Inx(Register registers)
        {
            switch (registers)
            {
                case Register.BC: state.BC++; break;
                case Register.DE: state.DE++; break;
                case Register.HL: state.HL++; break;
                case Register.SP: state.SP++; break;
            }

            state.Cycles += 5;

        }

       void Inr(Register register)
        {
            switch (register)
            {
                case Register.A: state.A++; UpdateZSP(state.A); break;
                case Register.B: state.B++; UpdateZSP(state.B); break;
                case Register.C: state.C++; UpdateZSP(state.C); break;
                case Register.D: state.D++; UpdateZSP(state.D); break;
                case Register.E: state.E++; UpdateZSP(state.E); break;
                case Register.H: state.H++; UpdateZSP(state.H); break;
                case Register.L: state.L++; UpdateZSP(state.L); break;
                case Register.HL: 
                    byte value = ReadByteFromMemory(state.HL);
                    WriteByteToMemory(state.HL, ++value); 
                    UpdateZSP(value); 
                    state.Cycles += 5; 
                    break;
            }

            state.Cycles += 5;
        }

       void Dcr(Register register)
        {
            switch (register)
            {
                case Register.A: state.A--; UpdateZSP(state.A); break;
                case Register.B: state.B--; UpdateZSP(state.B); break;
                case Register.C: state.C--; UpdateZSP(state.C); break;
                case Register.D: state.D--; UpdateZSP(state.D); break;
                case Register.E: state.E--; UpdateZSP(state.E); break;
                case Register.H: state.H--; UpdateZSP(state.H); break;
                case Register.L: state.L--; UpdateZSP(state.L); break;
                case Register.HL: 
                    byte value = ReadByteFromMemory(state.HL);
                    WriteByteToMemory(state.HL, --value); 
                    UpdateZSP(value); 
                    state.Cycles += 5; 
                    break;
            }

            state.Cycles += 5;
        }

       void Mvi(Register register)
        {
            switch (register)
            {
                case Register.A: state.A = GetNextByte(); break;
                case Register.B: state.B = GetNextByte(); break;
                case Register.C: state.C = GetNextByte(); break;
                case Register.D: state.D = GetNextByte(); break;
                case Register.E: state.E = GetNextByte(); break;
                case Register.H: state.H = GetNextByte(); break;
                case Register.L: state.L = GetNextByte(); break;
                case Register.HL: 
                    WriteByteToMemory(state.HL, GetNextByte()); 
                    state.Cycles += 3; 
                    break;
            }

            state.Cycles += 7;   
        }

       void Rlc()
        {
            state.Flags.Carry = ((state.A & 0x80) >> 7) == 1;
            state.A <<= 1;

            if (state.Flags.Carry)
                state.A |= 1;

            state.Cycles += 4;
        }

       void Rrc()
        {
            state.Flags.Carry = (state.A & 0x1) == 1;
            state.A >>= 1;

            if (state.Flags.Carry)
                state.A |= 0x80;

            state.Cycles += 4;
        }

        void Ral()
        {
            bool previousCarry = state.Flags.Carry;
            state.Flags.Carry = ((state.A & 0x80) >> 7) == 1;
            state.A <<= 1;

            if (previousCarry)
                state.A |= 1;

            state.Cycles += 4;
        }

        void Rar()
        {
            bool previousCarry = state.Flags.Carry;
            state.Flags.Carry = (state.A & 0x1) == 1;
            state.A >>= 1;

            if (previousCarry)
                state.A |= 0x80;

            state.Cycles += 4;
        }

        void Daa()
        {
            bool carry = state.Flags.Carry;
            byte correction = 0;

            byte lsb = (byte) (state.A & 0x0F);
            byte msb = (byte) (state.A >> 4);

            if (state.Flags.AuxiliaryCarry || lsb > 9) {
                correction += 0x06;
            }
            if (state.Flags.Carry || msb > 9 || (msb >= 9 && lsb > 9)) {
                correction += 0x60;
                carry = true;
            }

            state.A += correction;
            UpdateZSP(state.A);
            state.Flags.Carry = carry;
        }

        void Stc()
        {
            state.Flags.Carry = true;
            state.Cycles += 4;
        }

        void Dad(Register register)
        {
            ushort word = GetWordFromRegister(register);
            state.Flags.Carry = ((state.HL + word) & 0x10000) == 0x10000;
            state.HL += word;
            state.Cycles += 4;
        }

        void Ldax(Register register)
        {
            byte value = GetByteFromRegister(register);
            state.A = value;
            state.Cycles += 7;
        }

        void Lhld()
        {
            ushort addr = GetNextWord();
            state.HL = ReadWordFromMemory(addr);
            state.Cycles += 16;
        }

        void Lda()
        {
            state.A = ReadByteFromMemory(GetNextWord());
            state.Cycles += 13;
        }

        void Dcx(Register register)
        {
            switch (register)
            {
                case Register.BC: state.BC--; break;
                case Register.DE: state.DE--; break;
                case Register.HL: state.HL--; break;
                case Register.SP: state.SP--; break;
            }

            state.Cycles += 5;
        }

        void Cma()
        {
            state.A = (byte)~state.A;
            state.Cycles += 4;
        }

        void Cmc()
        {
            state.Flags.Carry = !state.Flags.Carry;
            state.Cycles += 4;
        }

        void Mov(Register registerA, Register registerB)
        {
            byte value = GetByteFromRegister(registerB);

            if (registerB == Register.HL)
                state.Cycles += 2;

            switch (registerA)
            {
                case Register.A: state.A = value; break;
                case Register.B: state.B = value; break;
                case Register.C: state.C = value; break;
                case Register.D: state.D = value; break;
                case Register.E: state.E = value; break;
                case Register.H: state.H = value; break;
                case Register.L: state.L = value; break;
                case Register.HL: WriteByteToMemory(state.HL, value); state.Cycles += 2; break;
            }

            state.Cycles += 5;
        }

        void Add(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A + value;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(state.A, value);
            state.A = (byte) (state.A + value);
            state.Cycles += 4;
        }

        void Adi()
        {
            byte value = GetNextByte();
            int result = state.A + value;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value);
            state.Cycles += 7;
        }

        void Aci()
        {
            byte value = GetNextByte();
            int carryValue = state.Flags.Carry ? 1 : 0;
            int result = state.A + value + carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value + carryValue);
            state.Cycles += 7;
        }

        void Adc(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = state.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A + value + carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(state.A, value, (byte) carryValue);
            state.A = (byte) (state.A + value + carryValue);
            state.Cycles += 4;
        }

        void Sub(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A - value;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0;
            state.Flags.AuxiliaryCarry = CheckAuxiliaryCarrySub(state.A, value);
            state.A = (byte) (state.A - value);
            state.Cycles += 4;   
        }

        void Sui()
        {
            byte value = GetNextByte();
            int result = state.A - value;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value);
            state.Cycles += 7;   
        }

        void Sbi()
        {
            byte value = GetNextByte();
            int carryValue = state.Flags.Carry ? 1 : 0;
            int result = state.A - value - carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value - carryValue);
            state.Cycles += 7;   
        }

        void Sbb(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = state.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A - value - carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.Flags.AuxiliaryCarry = CheckAuxiliaryCarrySub(state.A, value, (byte) carryValue);
            state.A = (byte) (state.A - value - carryValue);
            state.Cycles += 4;   
        }

        void Ana(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A &= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        void Ani()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A &= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        void Xra(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A ^= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        void Xri()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A ^= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        void Ora(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A |= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        void Ori()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A |= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        void Cmp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            ushort result = (ushort)(state.A - value);
            state.Flags.Carry = (result & 0xF000) == 0xF000;
            UpdateZSP((byte) ((state.A - value)));
            state.Cycles += 4;
        }

        void Cpi()
        {
            byte value = GetNextByte();                
            ushort result = (ushort)(state.A - value);
            state.Flags.Carry = (result & 0xF000) == 0xF000;                
            UpdateZSP((byte) ((state.A - value)));
            state.Cycles += 7;
        }

        void Ret()
        {
            state.PC = PopStack();
            state.Cycles += 10;
        }

        void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                state.Cycles += 1;
            }
            else
                state.Cycles += 5;
        }

        void Rnz()
        {
            ConditionalRet(!state.Flags.Zero);
        }

        void Rnc()
        {
            ConditionalRet(!state.Flags.Carry);
        }

        void Rpo()
        {
            ConditionalRet(!state.Flags.Parity);
        }

        void Rp()
        {
            ConditionalRet(!state.Flags.Sign);
        }

        void Rz()
        {
            ConditionalRet(state.Flags.Zero);
        }

        void Rc()
        {
            ConditionalRet(state.Flags.Carry);
        }

        void Rpe()
        {
            ConditionalRet(state.Flags.Parity);
        }

        void Rm()
        {
            ConditionalRet(state.Flags.Sign);
        }

        void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: state.BC = PopStack(); break;
                case Register.DE: state.DE = PopStack(); break;
                case Register.HL: state.HL = PopStack(); break;
            }

            state.Cycles += 10;
        }

        void PopPsw()
        {
            ushort af = PopStack();
            state.A = (byte)(af >> 8);
            byte psw = (byte) (af & 0xFF);
            
            state.Flags.Sign = ((psw >> 7) & 1) == 1;
            state.Flags.Zero = ((psw >> 6) & 1) == 1;
            state.Flags.AuxiliaryCarry = ((psw >> 4) & 1) == 1;
            state.Flags.Parity = ((psw >> 2) & 1) == 1;
            state.Flags.Carry = (psw & 1) == 1;

            state.Cycles += 10;
        }

        void Push(Register register)
        {
            switch (register)
            {
                case Register.BC: PushStack(state.BC); break;
                case Register.DE: PushStack(state.DE); break;
                case Register.HL: PushStack(state.HL); break;
                case Register.AF: PushStack(state.AF); break;
            }

            state.Cycles += 11;
        }

        void Jmp()
        {
            ushort addr = GetNextWord();
            state.PC = addr;
            state.Cycles += 10;
        }

        void Jmp(ushort addr)
        {
            state.PC = addr;
            state.Cycles += 10;
        }

        void ConditionalJmp(bool condition)
        {
            if (condition)
                Jmp();
            else
                GetNextWord();

            state.Cycles += 3;
        }

        void Jnz()
        {
            ConditionalJmp(!state.Flags.Zero);
        }

        void Jnc()
        {
            ConditionalJmp(!state.Flags.Carry);
        }

        void Jpo()
        {
            ConditionalJmp(!state.Flags.Parity);
        }

        void Jp()
        {
            ConditionalJmp(!state.Flags.Sign);
        }

        void Jz()
        {
            ConditionalJmp(state.Flags.Zero);
        }

        void Jc()
        {
            ConditionalJmp(state.Flags.Carry);
        }

        void Jpe()
        {
            ConditionalJmp(state.Flags.Parity);
        }

        void Jm()
        {
            ConditionalJmp(state.Flags.Sign);
        }

        void In()
		{
            state.Cycles += 10;
		}

        void Out()
        {
            state.Cycles += 10;
        }

        void Xthl()
        {
            ushort value = ReadWordFromMemory(state.SP);
            WriteWordToMemory(state.SP, state.HL);
            state.HL = value;

            state.Cycles += 18;
        }

        void Di()
        {
            state.EnableInterrupts = false;
            state.Cycles += 4;
        }

        void Ei()
        {
            state.EnableInterrupts = true;
            state.Cycles += 4;
        }

        void Call()
        {
            ushort addr = GetNextWord();
            PushStack(state.PC);
            Jmp(addr);
            state.Cycles += 7;
        }

        void Call(ushort addr)
        {
            PushStack(state.PC);
            Jmp(addr);
            state.Cycles += 7;
        }

        void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                state.Cycles += 11;
            }
        }

        void Cnz()
        {
            ConditionalCall(!state.Flags.Zero);
        }

        void Cnc()
        {
            ConditionalCall(!state.Flags.Carry);
        }

        void Cpo()
        {
            ConditionalCall(!state.Flags.Parity);
        }

        void Cp()
        {
            ConditionalCall(!state.Flags.Sign);
        }

        void Cz()
        {
            ConditionalCall(state.Flags.Zero);
        }

        void Cc()
        {
            ConditionalCall(state.Flags.Carry);
        }

        void Cpe()
        {
            ConditionalCall(state.Flags.Parity);
        }

        void Cm()
        {
            ConditionalCall(state.Flags.Sign);
        }

        void Rst(int num)
        {
            switch (num)
            {
                case 0: Call(0x00); break;
                case 1: Call(0x08); break;
                case 2: Call(0x10); break;
                case 3: Call(0x18); break;
                case 4: Call(0x20); break;
                case 5: Call(0x28); break;
                case 6: Call(0x30); break;
                case 7: Call(0x38); break;
            }

            state.Cycles += 11;
        }

        void Pchl()
        {
            state.PC = state.HL;
            state.Cycles += 5;
        }

        void Sphl()
        {
            state.SP = state.HL;
            state.Cycles += 5;
        }

        void Xchg()
        {
            ushort de = state.DE;
            state.DE = state.HL;
            state.HL = de;
            state.Cycles += 5;
        }



        void Hlt()
        {
            state.Halted = true;
            state.Cycles += 7;
        }
    }
}