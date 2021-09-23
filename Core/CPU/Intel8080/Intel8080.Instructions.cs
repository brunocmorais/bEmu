using System;
using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Util;

namespace bEmu.Core.CPU.Intel8080
{
    public abstract partial class Intel8080<TState, TMMU>
    {
        protected void Nop()
        {
            IncreaseCycles(4);
        }

        protected void Lxi(Register register)
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
                    State.SP = Endianness.GetWordFrom2Bytes(MMU[State.PC++], MMU[State.PC++]);
                    break;
            }
            
            IncreaseCycles(10);
        }

        protected void Stax(Register register)
        {
            if (register == Register.BC)
                MMU[State.BC] = State.A;
            else if (register == Register.DE)
                MMU[State.DE] = State.A;

            IncreaseCycles(7);
        }

        protected void Shld()
        {
            ushort value = State.HL;
            ushort addr = GetNextWord();
            WriteWordToMemory(addr, value);
            IncreaseCycles(16);
        }

        protected void Sta()
        {
            WriteByteToMemory(GetNextWord(), State.A);
            IncreaseCycles(13);
        }

        protected void Inx(Register registers)
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

        protected void Inr(Register register)
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

        protected void Dcr(Register register)
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

        protected void Mvi(Register register)
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

        protected void Rlc()
        {
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (State.Flags.Carry)
                State.A |= 1;

            IncreaseCycles(4);
        }

        protected void Rrc()
        {
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (State.Flags.Carry)
                State.A |= 0x80;

            IncreaseCycles(4);
        }

        protected void Ral()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (previousCarry)
                State.A |= 1;

            IncreaseCycles(4);
        }

        protected void Rar()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (previousCarry)
                State.A |= 0x80;

            IncreaseCycles(4);
        }

        protected void Daa()
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

        protected void Stc()
        {
            State.Flags.Carry = true;
            IncreaseCycles(4);
        }

        protected void Dad(Register register)
        {
            ushort word = GetWordFromRegister(register);
            State.Flags.Carry = ((State.HL + word) & 0x10000) == 0x10000;
            State.HL += word;
            IncreaseCycles(4);
        }

        protected void Ldax(Register register)
        {
            byte value = GetByteFromRegister(register);
            State.A = value;
            IncreaseCycles(7);
        }

        protected void Lhld()
        {
            ushort addr = GetNextWord();
            State.HL = ReadWordFromMemory(addr);
            IncreaseCycles(16);
        }

        protected void Lda()
        {
            State.A = ReadByteFromMemory(GetNextWord());
            IncreaseCycles(13);
        }

        protected void Dcx(Register register)
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

        protected void Cma()
        {
            State.A = (byte)~State.A;
            IncreaseCycles(4);
        }

        protected void Cmc()
        {
            State.Flags.Carry = !State.Flags.Carry;
            IncreaseCycles(4);
        }

        protected void Mov(Register registerA, Register registerB)
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

        protected void Add(Register register)
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

        protected void Adi()
        {
            byte value = GetNextByte();
            int result = State.A + value;
            UpdateFlags((byte) result);
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.AuxiliaryCarry = CheckAuxiliaryCarryAdd(State.A, value);
            State.A = (byte) (State.A + value);
            IncreaseCycles(7);
        }

        protected void Aci()
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

        protected void Adc(Register register)
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

        protected void Sub(Register register)
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

        protected void Sui()
        {
            byte value = GetNextByte();
            int result = State.A - value;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value);
            IncreaseCycles(7);   
        }

        protected void Sbi()
        {
            byte value = GetNextByte();
            int carryValue = State.Flags.Carry ? 1 : 0;
            int result = State.A - value - carryValue;
            UpdateFlags((byte) result);
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value - carryValue);
            IncreaseCycles(7);   
        }

        protected void Sbb(Register register)
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

        protected void Ana(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A &= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        protected void Ani()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A &= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        protected void Xra(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A ^= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        protected void Xri()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A ^= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        protected void Ora(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            State.Flags.Carry = false;
            State.A |= value;
            UpdateFlags(State.A);
            IncreaseCycles(4);
        }

        protected void Ori()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A |= value;
            UpdateFlags(State.A);
            IncreaseCycles(7);
        }

        protected void Cmp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(3);

            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;
            UpdateFlags((byte) ((State.A - value)));
            IncreaseCycles(4);
        }

        protected void Cpi()
        {
            byte value = GetNextByte();                
            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;                
            UpdateFlags((byte) ((State.A - value)));
            IncreaseCycles(7);
        }

        protected void Ret()
        {
            State.PC = PopStack();
            IncreaseCycles(10);
        }

        protected void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                IncreaseCycles(1);
            }
            else
                IncreaseCycles(5);
        }

        protected void Rnz()
        {
            ConditionalRet(!State.Flags.Zero);
        }

        protected void Rnc()
        {
            ConditionalRet(!State.Flags.Carry);
        }

        protected void Rpo()
        {
            ConditionalRet(!State.Flags.Parity);
        }

        protected void Rp()
        {
            ConditionalRet(!State.Flags.Sign);
        }

        protected void Rz()
        {
            ConditionalRet(State.Flags.Zero);
        }

        protected void Rc()
        {
            ConditionalRet(State.Flags.Carry);
        }

        protected void Rpe()
        {
            ConditionalRet(State.Flags.Parity);
        }

        protected void Rm()
        {
            ConditionalRet(State.Flags.Sign);
        }

        protected void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC = PopStack(); break;
                case Register.DE: State.DE = PopStack(); break;
                case Register.HL: State.HL = PopStack(); break;
            }

            IncreaseCycles(10);
        }

        protected void PopPsw()
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

        protected void Push(Register register)
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

        protected void Jmp()
        {
            ushort addr = GetNextWord();
            State.PC = addr;
            IncreaseCycles(10);
        }

        protected void Jmp(ushort addr)
        {
            State.PC = addr;
            IncreaseCycles(10);
        }

        protected void ConditionalJmp(bool condition)
        {
            if (condition)
                Jmp();
            else
                GetNextWord();

            IncreaseCycles(3);
        }

        protected void Jnz()
        {
            ConditionalJmp(!State.Flags.Zero);
        }

        protected void Jnc()
        {
            ConditionalJmp(!State.Flags.Carry);
        }

        protected void Jpo()
        {
            ConditionalJmp(!State.Flags.Parity);
        }

        protected void Jp()
        {
            ConditionalJmp(!State.Flags.Sign);
        }

        protected void Jz()
        {
            ConditionalJmp(State.Flags.Zero);
        }

        protected void Jc()
        {
            ConditionalJmp(State.Flags.Carry);
        }

        protected void Jpe()
        {
            ConditionalJmp(State.Flags.Parity);
        }

        protected void Jm()
        {
            ConditionalJmp(State.Flags.Sign);
        }

        public abstract void In();

        public abstract void Out();

        protected void Xthl()
        {
            ushort value = ReadWordFromMemory(State.SP);
            WriteWordToMemory(State.SP, State.HL);
            State.HL = value;

            IncreaseCycles(18);
        }

        protected void Di()
        {
            State.EnableInterrupts = false;
            IncreaseCycles(4);
        }

        protected void Ei()
        {
            State.EnableInterrupts = true;
            IncreaseCycles(4);
        }

        protected void Call()
        {
            ushort addr = GetNextWord();
            PushStack(State.PC);
            Jmp(addr);
            IncreaseCycles(7);
        }

        protected void Call(ushort addr)
        {
            PushStack(State.PC);
            Jmp(addr);
            IncreaseCycles(7);
        }

        protected void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                IncreaseCycles(11);
            }
        }

        protected void Cnz()
        {
            ConditionalCall(!State.Flags.Zero);
        }

        protected void Cnc()
        {
            ConditionalCall(!State.Flags.Carry);
        }

        protected void Cpo()
        {
            ConditionalCall(!State.Flags.Parity);
        }

        protected void Cp()
        {
            ConditionalCall(!State.Flags.Sign);
        }

        protected void Cz()
        {
            ConditionalCall(State.Flags.Zero);
        }

        protected void Cc()
        {
            ConditionalCall(State.Flags.Carry);
        }

        protected void Cpe()
        {
            ConditionalCall(State.Flags.Parity);
        }

        protected void Cm()
        {
            ConditionalCall(State.Flags.Sign);
        }

        protected void Rst(int num)
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

        protected void Pchl()
        {
            State.PC = State.HL;
            IncreaseCycles(5);
        }

        protected void Sphl()
        {
            State.SP = State.HL;
            IncreaseCycles(5);
        }

        protected void Xchg()
        {
            ushort de = State.DE;
            State.DE = State.HL;
            State.HL = de;
            IncreaseCycles(5);
        }

        protected void Hlt()
        {
            State.Halted = true;
            IncreaseCycles(7);
        }
    }
}