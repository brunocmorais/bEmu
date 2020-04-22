using System;
using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.Intel8080
{
    public partial class Intel8080<TState, TMMU> : CPU<TState, TMMU> 
        where TState : State
        where TMMU : MMU
    {
        private void Nop()
        {
            IncreaseCycles(4);
        }

        private void Lxi(Register register)
        {
            switch (register)
            {
                case Register.BC:
                    State.C = MMU[State.PC++];
                    State.B = MMU[State.PC++];
                    break;
                case Register.DE:
                    State.E = MMU[State.PC++];
                    State.D = MMU[State.PC++];
                    break;
                case Register.HL:
                    State.L = MMU[State.PC++];
                    State.H = MMU[State.PC++];
                    break;
                case Register.SP:
                    State.SP = BitUtils.GetWordFrom2Bytes(MMU[State.PC++], MMU[State.PC++]);
                    break;
            }
            
            IncreaseCycles(10);
        }

        private void Stax(Register register)
        {
            if (register == Register.BC)
                MMU[State.BC] = State.A;
            else if (register == Register.DE)
                MMU[State.DE] = State.A;

            IncreaseCycles(7);
        }

        private void Shld()
        {
            ushort value = State.HL;
            ushort addr = GetNextWord();
            WriteWordToMemory(addr, value);
            IncreaseCycles(16);
        }

        private void Sta()
        {
            WriteByteToMemory(GetNextWord(), State.A);
            IncreaseCycles(13);
        }

        private void Inx(Register registers)
        {
            switch (registers)
            {
                case Register.BC: State.BC++; break;
                case Register.DE: State.DE++; break;
                case Register.HL: State.HL++; break;
                case Register.SP: State.SP++; break;
            }

            IncreaseCycles(5);

        }

        private void Inr(Register register)
        {
            switch (register)
            {
                case Register.A: State.A++; UpdateFlags(State.A); break;
                case Register.B: State.B++; UpdateFlags(State.B); break;
                case Register.C: State.C++; UpdateFlags(State.C); break;
                case Register.D: State.D++; UpdateFlags(State.D); break;
                case Register.E: State.E++; UpdateFlags(State.E); break;
                case Register.H: State.H++; UpdateFlags(State.H); break;
                case Register.L: State.L++; UpdateFlags(State.L); break;
                case Register.HL: 
                    byte value = ReadByteFromMemory(State.HL);
                    WriteByteToMemory(State.HL, ++value); 
                    UpdateFlags(value); 
                    IncreaseCycles(5); 
                    break;
            }

            IncreaseCycles(5);
        }

        private void Dcr(Register register)
        {
            switch (register)
            {
                case Register.A: State.A--; UpdateFlags(State.A); break;
                case Register.B: State.B--; UpdateFlags(State.B); break;
                case Register.C: State.C--; UpdateFlags(State.C); break;
                case Register.D: State.D--; UpdateFlags(State.D); break;
                case Register.E: State.E--; UpdateFlags(State.E); break;
                case Register.H: State.H--; UpdateFlags(State.H); break;
                case Register.L: State.L--; UpdateFlags(State.L); break;
                case Register.HL: 
                    byte value = ReadByteFromMemory(State.HL);
                    WriteByteToMemory(State.HL, --value); 
                    UpdateFlags(value); 
                    IncreaseCycles(5); 
                    break;
            }

            IncreaseCycles(5);
        }

        private void Mvi(Register register)
        {
            switch (register)
            {
                case Register.A: State.A = GetNextByte(); break;
                case Register.B: State.B = GetNextByte(); break;
                case Register.C: State.C = GetNextByte(); break;
                case Register.D: State.D = GetNextByte(); break;
                case Register.E: State.E = GetNextByte(); break;
                case Register.H: State.H = GetNextByte(); break;
                case Register.L: State.L = GetNextByte(); break;
                case Register.HL: 
                    WriteByteToMemory(State.HL, GetNextByte()); 
                    IncreaseCycles(3); 
                    break;
            }

            IncreaseCycles(7);   
        }

        private void Rlc()
        {
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (State.Flags.Carry)
                State.A |= 1;

            IncreaseCycles(4);
        }

        private void Rrc()
        {
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (State.Flags.Carry)
                State.A |= 0x80;

            IncreaseCycles(4);
        }

        private void Ral()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (previousCarry)
                State.A |= 1;

            IncreaseCycles(4);
        }

        private void Rar()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (previousCarry)
                State.A |= 0x80;

            IncreaseCycles(4);
        }

        private void Daa()
        {
            bool carry = State.Flags.Carry;
            byte correction = 0;

            byte lsb = (byte) (State.A & 0x0F);
            byte msb = (byte) (State.A >> 4);

            if (State.Flags.AuxiliaryCarry || lsb > 9) {
                correction += 0x06;
            }
            if (State.Flags.Carry || msb > 9 || (msb >= 9 && lsb > 9)) {
                correction += 0x60;
                carry = true;
            }

            State.A += correction;
            UpdateFlags(State.A);
            State.Flags.Carry = carry;

            IncreaseCycles(4);
        }

        private void Stc()
        {
            State.Flags.Carry = true;
            IncreaseCycles(4);
        }

        private void Dad(Register register)
        {
            ushort word = GetWordFromRegister(register);
            State.Flags.Carry = ((State.HL + word) & 0x10000) == 0x10000;
            State.HL += word;
            IncreaseCycles(4);
        }

        private void Ldax(Register register)
        {
            byte value = GetByteFromRegister(register);
            State.A = value;
            IncreaseCycles(7);
        }

        private void Lhld()
        {
            ushort addr = GetNextWord();
            State.HL = ReadWordFromMemory(addr);
            IncreaseCycles(16);
        }

        private void Lda()
        {
            State.A = ReadByteFromMemory(GetNextWord());
            IncreaseCycles(13);
        }

        private void Dcx(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC--; break;
                case Register.DE: State.DE--; break;
                case Register.HL: State.HL--; break;
                case Register.SP: State.SP--; break;
            }

            IncreaseCycles(5);
        }

        private void Cma()
        {
            State.A = (byte)~State.A;
            IncreaseCycles(4);
        }

        private void Cmc()
        {
            State.Flags.Carry = !State.Flags.Carry;
            IncreaseCycles(4);
        }

        private void Mov(Register registerA, Register registerB)
        {
            byte value = GetByteFromRegister(registerB);

            if (registerB == Register.HL)
                IncreaseCycles(2);

            switch (registerA)
            {
                case Register.A: State.A = value; break;
                case Register.B: State.B = value; break;
                case Register.C: State.C = value; break;
                case Register.D: State.D = value; break;
                case Register.E: State.E = value; break;
                case Register.H: State.H = value; break;
                case Register.L: State.L = value; break;
                case Register.HL: WriteByteToMemory(State.HL, value); IncreaseCycles(2); break;
            }

            IncreaseCycles(5);
        }

        private void Add(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            int result = State.A + value;
            UpdateFlags((byte) result);
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(State.A, value);
            State.A = (byte) (State.A + value);
            IncreaseCycles(4);
        }

        private void Adi()
        {
            byte value = GetNextByte();
            int result = State.A + value;
            UpdateFlags((byte) result);
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(State.A, value);
            State.A = (byte) (State.A + value);
            IncreaseCycles(7);
        }

        private void Aci()
        {
            byte value = GetNextByte();
            int carryValue = State.Flags.Carry ? 1 : 0;
            int result = State.A + value + carryValue;
            UpdateFlags((byte) result);
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(State.A, value);
            State.A = (byte) (State.A + value + carryValue);
            IncreaseCycles(7);
        }

        private void Adc(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = State.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                IncreaseCycles(3);

            int result = State.A + value + carryValue;
            UpdateFlags((byte) result);
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(State.A, value, (byte) carryValue);
            State.A = (byte) (State.A + value + carryValue);
            IncreaseCycles(4);
        }

        private void Sub(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            int result = State.A - value;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0;
            State.A = (byte) (State.A - value);
            IncreaseCycles(4);   
        }

        private void Sui()
        {
            byte value = GetNextByte();
            int result = State.A - value;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value);
            IncreaseCycles(7);   
        }

        private void Sbi()
        {
            byte value = GetNextByte();
            int carryValue = State.Flags.Carry ? 1 : 0;
            int result = State.A - value - carryValue;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value - carryValue);
            IncreaseCycles(7);   
        }

        private void Sbb(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = State.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                IncreaseCycles(3);

            int result = State.A - value - carryValue;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value - carryValue);
            IncreaseCycles(4);   
        }

        private void Ana(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A &= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        private void Ani()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A &= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        private void Xra(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A ^= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        private void Xri()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A ^= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        private void Ora(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A |= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        private void Ori()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A |= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        private void Cmp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;
            UpdateFlags((byte) ((State.A - value)));
            IncreaseCycles(4);
        }

        private void Cpi()
        {
            byte value = GetNextByte();                
            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;                
            UpdateFlags((byte) ((State.A - value)));
            IncreaseCycles(7);
        }

        private void Ret()
        {
            State.PC = PopStack();
            IncreaseCycles(10);
        }

        private void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                IncreaseCycles(1);
            }
            else
                IncreaseCycles(5);
        }

        private void Rnz()
        {
            ConditionalRet(!State.Flags.Zero);
        }

        private void Rnc()
        {
            ConditionalRet(!State.Flags.Carry);
        }

        private void Rpo()
        {
            ConditionalRet(!State.Flags.Parity);
        }

        private void Rp()
        {
            ConditionalRet(!State.Flags.Sign);
        }

        private void Rz()
        {
            ConditionalRet(State.Flags.Zero);
        }

        private void Rc()
        {
            ConditionalRet(State.Flags.Carry);
        }

        private void Rpe()
        {
            ConditionalRet(State.Flags.Parity);
        }

        private void Rm()
        {
            ConditionalRet(State.Flags.Sign);
        }

        private void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC = PopStack(); break;
                case Register.DE: State.DE = PopStack(); break;
                case Register.HL: State.HL = PopStack(); break;
            }

            IncreaseCycles(10);
        }

        private void PopPsw()
        {
            ushort af = PopStack();
            State.A = (byte)(af >> 8);
            byte psw = (byte) (af & 0xFF);
            
            State.Flags.Sign = ((psw >> 7) & 1) == 1;
            State.Flags.Zero = ((psw >> 6) & 1) == 1;
            State.Flags.AuxiliaryCarry = ((psw >> 4) & 1) == 1;
            State.Flags.Parity = ((psw >> 2) & 1) == 1;
            State.Flags.Carry = (psw & 1) == 1;

            IncreaseCycles(10);
        }

        private void Push(Register register)
        {
            switch (register)
            {
                case Register.BC: PushStack(State.BC); break;
                case Register.DE: PushStack(State.DE); break;
                case Register.HL: PushStack(State.HL); break;
                case Register.AF: PushStack(State.AF); break;
            }

            IncreaseCycles(11);
        }

        private void Jmp()
        {
            ushort addr = GetNextWord();
            State.PC = addr;
            IncreaseCycles(10);
        }

        private void Jmp(ushort addr)
        {
            State.PC = addr;
            IncreaseCycles(10);
        }

        private void ConditionalJmp(bool condition)
        {
            if (condition)
                Jmp();
            else
                GetNextWord();

            IncreaseCycles(3);
        }

        private void Jnz()
        {
            ConditionalJmp(!State.Flags.Zero);
        }

        private void Jnc()
        {
            ConditionalJmp(!State.Flags.Carry);
        }

        private void Jpo()
        {
            ConditionalJmp(!State.Flags.Parity);
        }

        private void Jp()
        {
            ConditionalJmp(!State.Flags.Sign);
        }

        private void Jz()
        {
            ConditionalJmp(State.Flags.Zero);
        }

        private void Jc()
        {
            ConditionalJmp(State.Flags.Carry);
        }

        private void Jpe()
        {
            ConditionalJmp(State.Flags.Parity);
        }

        private void Jm()
        {
            ConditionalJmp(State.Flags.Sign);
        }

        private void In()
		{
            IncreaseCycles(10);
		}

        private void Out()
        {
            IncreaseCycles(10);
        }

        private void Xthl()
        {
            ushort value = ReadWordFromMemory(State.SP);
            WriteWordToMemory(State.SP, State.HL);
            State.HL = value;

            IncreaseCycles(18);
        }

        private void Di()
        {
            State.EnableInterrupts = false;
            IncreaseCycles(4);
        }

        private void Ei()
        {
            State.EnableInterrupts = true;
            IncreaseCycles(4);
        }

        private void Call()
        {
            ushort addr = GetNextWord();
            PushStack(State.PC);
            Jmp(addr);
            IncreaseCycles(7);
        }

        private void Call(ushort addr)
        {
            PushStack(State.PC);
            Jmp(addr);
            IncreaseCycles(7);
        }

        private void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                IncreaseCycles(11);
            }
        }

        private void Cnz()
        {
            ConditionalCall(!State.Flags.Zero);
        }

        private void Cnc()
        {
            ConditionalCall(!State.Flags.Carry);
        }

        private void Cpo()
        {
            ConditionalCall(!State.Flags.Parity);
        }

        private void Cp()
        {
            ConditionalCall(!State.Flags.Sign);
        }

        private void Cz()
        {
            ConditionalCall(State.Flags.Zero);
        }

        private void Cc()
        {
            ConditionalCall(State.Flags.Carry);
        }

        private void Cpe()
        {
            ConditionalCall(State.Flags.Parity);
        }

        private void Cm()
        {
            ConditionalCall(State.Flags.Sign);
        }

        private void Rst(int num)
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

            IncreaseCycles(11);
        }

        private void Pchl()
        {
            State.PC = State.HL;
            IncreaseCycles(5);
        }

        private void Sphl()
        {
            State.SP = State.HL;
            IncreaseCycles(5);
        }

        private void Xchg()
        {
            ushort de = State.DE;
            State.DE = State.HL;
            State.HL = de;
            IncreaseCycles(5);
        }

        private void Hlt()
        {
            State.Halted = true;
            IncreaseCycles(7);
        }
    }
}