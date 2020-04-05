using System;
using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Util;

namespace bEmu.Core.CPUs.LR35902
{
    public partial class LR35902<TState> : CPUs.Intel8080.Intel8080<TState> where TState : State
    {
        private void Bit(int bitNumber, Register register)
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

        private void Set(int bitNumber, Register register)
        {
            byte value = GetByteFromRegister(register);
            value |= (byte) (0x1 << bitNumber);

            SetByteToRegister(register, value);

            if (register == Register.HL)
                IncreaseCycles(8);

            IncreaseCycles(8);
        }

        private void Res(int bitNumber, Register register)
        {
            byte value = GetByteFromRegister(register);
            value &= (byte) ~(0x1 << bitNumber);
            SetByteToRegister(register, value);

            if (register == Register.HL)
                IncreaseCycles(8);

            IncreaseCycles(8);
        }

        private void Rlc(Register register)
        {
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) << 1));

            if (State.Flags.Carry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x80) >> 7) == 1;

            IncreaseCycles(4);
        }

        private void Rrc(Register register)
        {
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) >> 1));

            if (State.Flags.Carry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 0x80));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x1)) == 1;

            IncreaseCycles(4);
        }

        private void Rl(Register register)
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x80) >> 7) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) << 1));

            if (previousCarry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Rr(Register register)
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x1)) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) >> 1));

            if (previousCarry)
                SetByteToRegister(register, (byte) (GetByteFromRegister(register) | 0x80));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Sla(Register register)
        {
            State.Flags.Carry = ((GetByteFromRegister(register) & 0x80) >> 7) == 1;
            SetByteToRegister(register, (byte) (GetByteFromRegister(register) << 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Sra(Register register)
        {
            byte value = GetByteFromRegister(register);
            byte bit7 = (byte) (value >> 7); 
            State.Flags.Carry = (value & 0x1) == 1;
            SetByteToRegister(register, (byte) ((value >> 1) | (bit7 << 7)));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Swap(Register register)
        {
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

        private void Srl(Register register)
        {
            byte value = GetByteFromRegister(register);
            State.Flags.Carry = (value & 0x1) == 1;
            SetByteToRegister(register, (byte) (value >> 1));

            State.Flags.Zero = CheckZero(GetByteFromRegister(register));
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Jp(ushort addr)
        {
            State.PC = addr;
            IncreaseCycles(10);
        }

        private void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                IncreaseCycles(12);
            }
        }

        private void Call(ushort addr)
        {
            PushStack(State.PC);
            Jp(addr);
        }

        private void Call()
        {
            ushort addr = GetNextWord();
            PushStack(State.PC);
            Jp(addr);
            IncreaseCycles(14);
        }

        private void CallC()
        {
            ConditionalCall(State.Flags.Carry);
        }

        private void CallZ()
        {
            ConditionalCall(State.Flags.Zero);
        }

        private void Adc()
        {
            byte value = GetNextByte();
            int carryValue = State.Flags.Carry ? 1 : 0;
            int result = State.A + value + carryValue;
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarryAdd(State.A, value);
            State.A = (byte) (State.A + value + carryValue);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            IncreaseCycles(8);
        }

        private void Sbc()
        {
            byte value = GetNextByte();
            int carryValue = State.Flags.Carry ? 1 : 0;
            int result = State.A - value - carryValue;
            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarrySub(State.A, value); 
            State.A = (byte) (State.A - value - carryValue);
            State.Flags.Subtract = true;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(7);   
        }

        private void Xor()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A ^= value;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            IncreaseCycles(8);
        }

        private void Cp()
        {
            byte value = GetNextByte();                
            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;                
            State.Flags.Zero = CheckZero((byte) (State.A - value));
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = CheckHalfCarrySub((byte) (State.A - value), value);
            IncreaseCycles(7);
        }

        private void Ei()
        {
            State.EnableInterrupts = true;
        }

        private void RetZ()
        {
            ConditionalRet(State.Flags.Zero);
        }

        private void RetC()
        {
            ConditionalRet(State.Flags.Carry);
        }

        private void AddSP()
        {
            sbyte value = (sbyte) GetNextByte();
            ushort sp = State.SP;
            State.SP = (ushort) (sp + value);

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.Carry = (((sp ^ value ^ State.SP) & 0x100) == 0x100); 
            State.Flags.HalfCarry = (((sp ^ value ^ State.SP) & 0x10) == 0x10); 
            
            IncreaseCycles(16);
        }

        private void Ld_HL_SPr8()
        {
            sbyte value = (sbyte) GetNextByte();
            ushort sp = State.SP;
            State.HL = (ushort) (State.SP + value);

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.Carry = (((sp ^ value ^ State.HL) & 0x100) == 0x100); 
            State.Flags.HalfCarry = (((sp ^ value ^ State.HL) & 0x10) == 0x10); 

            IncreaseCycles(12);
        }

        private void Ret()
        {
            State.PC = PopStack();
            IncreaseCycles(16);
        }

        private void Reti()
        {
            Ret();
            State.EnableInterrupts = true;
        }

        private void Jp_HL()
        {
            Jp(State.HL);
            IncreaseCycles(6);
        }

        private void Ld_SPHL()
        {
            State.SP = State.HL;
            IncreaseCycles(8);
        }

        private void JpZ()
        {
            ConditionalJmp(State.Flags.Zero);
        }

        private void Ld_a16_A()
        {
            MMU[GetNextWord()] = State.A;
            IncreaseCycles(16);
        }

        private void Ld_A_a16()
        {
            State.A = MMU[GetNextWord()];
            IncreaseCycles(16);
        }

        private void Add_A_d8()
        {
            byte value = GetNextByte();
            int result = State.A + value;

            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarryAdd(State.A, value);
            State.A = (byte) (State.A + value);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;

            IncreaseCycles(8);
        }

        private void Sub_d8()
        {
            byte value = GetNextByte();
            int result = State.A - value;
            State.Flags.Carry = result < 0; 
            State.A = (byte) (State.A - value);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = CheckHalfCarrySub(State.A, value);
            IncreaseCycles(7);   
        }

        private void And_d8()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.Flags.HalfCarry = true;
            State.Flags.Subtract = false;
            State.A &= value;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(8);
        }

        private void Or_d8()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;
            State.A |= value;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(8);
        }

        private void Rst(ushort addr)
        {
            Call(addr);
            IncreaseCycles(6);
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

            IncreaseCycles(16);
        }

        private void CallNC()
        {
            ConditionalCall(!State.Flags.Carry);
        }

        private void CallNZ()
        {
            ConditionalCall(!State.Flags.Zero);
        }

        private void JpNZ()
        {
            ConditionalJmp(!State.Flags.Zero);
        }

        private void JpNC()
        {
            ConditionalJr(!State.Flags.Carry);
        }

        private void Ld_C_A()
        {
            MMU[0xFF00 + State.C] = State.A;
            IncreaseCycles(8);
        }

        private void Ld_A_C()
        {
            State.A = MMU[0xFF00 + State.C];
            IncreaseCycles(8);
        }

        private void JpC()
        {
            ConditionalJmp(State.Flags.Carry);
        }

        private void Jp()
        {
            ushort addr = GetNextWord();
            State.PC = addr;
            IncreaseCycles(16);
        }

        private void Di()
        {
            State.EnableInterrupts = false;
            IncreaseCycles(4);
        }

        private void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC = PopStack(); break;
                case Register.DE: State.DE = PopStack(); break;
                case Register.HL: State.HL = PopStack(); break;
            }

            IncreaseCycles(12);
        }

        private void PopPsw()
        {
            ushort af = PopStack();
            State.A = (byte)(af >> 8);
            byte psw = (byte) (af & 0xFF);
            
            State.Flags.Zero = ((psw >> 7) & 1) == 1;
            State.Flags.Subtract = ((psw >> 6) & 1) == 1;
            State.Flags.HalfCarry = ((psw >> 5) & 1) == 1;
            State.Flags.Carry = ((psw >> 4) & 1) == 1;

            IncreaseCycles(10);
        }

        private void Ldh_A_a8()
        {
            byte value = GetNextByte();
            State.A = MMU[0xFF00 + value];
            IncreaseCycles(12);
        }

        private void Ldh_a8_A()
        {
            byte value = GetNextByte();
            MMU[0xFF00 + value] = State.A;
            IncreaseCycles(12);
        }

        private void RetNC()
        {
            ConditionalRet(!State.Flags.Carry);
        }

        private void RetNZ()
        {
            ConditionalRet(!State.Flags.Zero);
        }

        private void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                IncreaseCycles(4);
            }
            else
                IncreaseCycles(8);
        }

        private void Cp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            ushort result = (ushort)(State.A - value);
            State.Flags.Carry = (result & 0xF000) == 0xF000;
            State.Flags.Zero = CheckZero((byte) result);
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = CheckHalfCarrySub(State.A, value);

            IncreaseCycles(4);
        }

        private void Or(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            State.Flags.Carry = false;
            State.A |= value;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;
            IncreaseCycles(4);
        }

        private void Xor(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            State.Flags.Carry = false;
            State.A ^= value;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;
            IncreaseCycles(4);
        }

        private void And(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            State.Flags.Carry = false;
            State.A &= value;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.HalfCarry = true;
            State.Flags.Subtract = false;
            IncreaseCycles(4);
        }

        private void Sbc(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A - value;
            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarrySub(State.A, value);
            State.A = (byte) (State.A - value);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            IncreaseCycles(4);   
        }

        private void Sub(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A - value;
            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarrySub(State.A, value);
            State.A = (byte) (State.A - value);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            IncreaseCycles(4);   
        }

        private void Adc(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = State.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A + value + carryValue;
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarryAdd(State.A, value, (byte) carryValue);
            State.A = (byte) (State.A + value + carryValue);
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            IncreaseCycles(4);
        }

        private void Add(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A + value;

            State.Flags.Subtract = false;
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarryAdd(State.A, value);
            State.A = (byte) (State.A + value);
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(4);
        }

        private void Halt()
        {
            State.Halted = true;
            IncreaseCycles(4);
        }

        private void Ld(Register registerA, Register registerB)
        {
            byte value = GetByteFromRegister(registerB);

            if (registerB == Register.HL)
                IncreaseCycles(4);

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

            IncreaseCycles(4);
        }

        private void Ccf()
        {
            State.Flags.Carry = !State.Flags.Carry;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        private void Cpl()
        {
            State.A = (byte) ~State.A;
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = true;
            IncreaseCycles(4);
        }

        private void Rra()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (previousCarry)
                State.A |= 0x80;

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Rrca()
        {
            State.Flags.Carry = (State.A & 0x1) == 1;
            State.A >>= 1;

            if (State.Flags.Carry)
                State.A |= 0x80;

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void LdA(Register register, Action action)
        {
            byte value = GetByteFromRegister(register);
            State.A = value;

            if (action == Action.Increment)
                State.HL++;
            else if (action == Action.Decrement)
                State.HL--;

            IncreaseCycles(8);
        }

        private void AddHL(Register register)
        {
            ushort word = GetWordFromRegister(register);
            State.Flags.Carry = ((State.HL + word) & 0x10000) == 0x10000;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = CheckHalfCarryAdd(State.HL, word);
            State.HL += word;
            IncreaseCycles(8);
        }

        private void Jrc()
        {
            ConditionalJr(State.Flags.Carry);
        }

        private void Jrz()
        {
            ConditionalJr(State.Flags.Zero);
        }

        private void Jr()
        {
            sbyte value = (sbyte) GetNextByte();
            int result = State.PC + value;
            State.PC = (ushort) result;
            IncreaseCycles(12);
        }

        private void ConditionalJr(bool condition)
        {
            if (condition)
                Jr();
            else
            {
                GetNextByte();
                IncreaseCycles(8);
            }
        }

        private void Ld_SP()
        {
            var addr = GetNextWord();
            BitUtils.Get2BytesFromWord(State.SP, out byte msb, out byte lsb);
            MMU[addr++] = lsb;
            MMU[addr++] = msb;

            IncreaseCycles(20);
        }

        private void Scf()
        {
            State.Flags.Carry = true;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        private void Daa()
        {
            bool carry = State.Flags.Carry;
            byte correction = 0;

            byte lsb = (byte) (State.A & 0x0F);
            byte msb = (byte) (State.A >> 4);

            if (State.Flags.HalfCarry || lsb > 9) {
                correction += 0x06;
            }
            if (State.Flags.Carry || msb > 9 || (msb >= 9 && lsb > 9)) {
                correction += 0x60;
                carry = true;
            }

            State.A += correction;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.HalfCarry = false;
            State.Flags.Carry = carry;

            IncreaseCycles(4);
        }

        private void Rla()
        {
            bool previousCarry = State.Flags.Carry;
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (previousCarry)
                State.A |= 1;

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Rlca()
        {
            State.Flags.Carry = ((State.A & 0x80) >> 7) == 1;
            State.A <<= 1;

            if (State.Flags.Carry)
                State.A |= 1;

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        private void Ld_d8(Register register)
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
                    IncreaseCycles(4); 
                    break;
            }

            IncreaseCycles(8);   
        }

        private void DecRegPair(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC--; break;
                case Register.DE: State.DE--; break;
                case Register.HL: State.HL--; break;
                case Register.SP: State.SP--; break;
            }

            IncreaseCycles(8);
        }

        private void DecRef()
        {
            MMU[State.HL]--;
            IncreaseCycles(12);
            State.Flags.Zero = CheckZero(MMU[State.HL]);
            State.Flags.HalfCarry = CheckHalfCarrySub(1, MMU[State.HL]);
            State.Flags.Subtract = true;
        }

        private void Dec(Register register)
        {
            switch (register)
            {
                case Register.A: 
                    State.A--; 
                    State.Flags.Zero = CheckZero(State.A);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.A);
                    break;
                case Register.B: 
                    State.B--; 
                    State.Flags.Zero = CheckZero(State.B);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.B);
                    break;
                case Register.C: 
                    State.C--; 
                    State.Flags.Zero = CheckZero(State.C);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.C);
                    break;
                case Register.D: 
                    State.D--; 
                    State.Flags.Zero = CheckZero(State.D);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.D);
                    break;
                case Register.E: 
                    State.E--; 
                    State.Flags.Zero = CheckZero(State.E);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.E);
                    break;
                case Register.H: 
                    State.H--; 
                    State.Flags.Zero = CheckZero(State.H);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.H);
                    break;
                case Register.L: 
                    State.L--; 
                    State.Flags.Zero = CheckZero(State.L);
                    State.Flags.HalfCarry = CheckHalfCarrySub(1, State.L);
                    break;
            }

            State.Flags.Subtract = true;
            IncreaseCycles(4);
        }

        private void IncRegPair(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC++; break;
                case Register.DE: State.DE++; break;
                case Register.HL: State.HL++; break;
                case Register.SP: State.SP++; break;
            }

            IncreaseCycles(8);
        }

        private void IncRef()
        {
            MMU[State.HL]++;
            IncreaseCycles(12);

            State.Flags.Zero = CheckZero(MMU[State.HL]);
            State.Flags.HalfCarry = CheckHalfCarryAdd(1, MMU[State.HL]);
            State.Flags.Subtract = false;
        }

        private void Inc(Register register)
        {
            switch (register)
            {
                case Register.A: 
                    State.A++; 
                    State.Flags.Zero = CheckZero(State.A);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.A);
                    break;
                case Register.B: 
                    State.B++; 
                    State.Flags.Zero = CheckZero(State.B);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.B);
                    break;
                case Register.C: 
                    State.C++; 
                    State.Flags.Zero = CheckZero(State.C);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.A);
                    break;
                case Register.D: 
                    State.D++; 
                    State.Flags.Zero = CheckZero(State.D);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.D);
                    break;
                case Register.E: 
                    State.E++; 
                    State.Flags.Zero = CheckZero(State.E);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.E);
                    break;
                case Register.H: 
                    State.H++; 
                    State.Flags.Zero = CheckZero(State.H);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.H);
                    break;
                case Register.L: 
                    State.L++; 
                    State.Flags.Zero = CheckZero(State.L);
                    State.Flags.HalfCarry = CheckHalfCarryAdd(1, State.L);
                    break;
            }

            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        private void LD_A(Register register, Action action)
        {
            if (register == Register.BC)
                MMU[State.BC] = State.A;
            else if (register == Register.DE)
                MMU[State.DE] = State.A;
            else if (register == Register.HL)
                MMU[State.HL] = State.A;

            if (action == Action.Increment)
                State.HL++;
            else if (action == Action.Decrement)
                State.HL--;

            IncreaseCycles(8);
        }

        private void LD_d16(Register register)
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
            
            IncreaseCycles(12);
        }

        private void JrNC()
        {
            ConditionalJr(!State.Flags.Carry);
            IncreaseCycles(8);
        }

        private void JrNZ()
        {
            ConditionalJr(!State.Flags.Zero);
            IncreaseCycles(8);
        }

        private void ConditionalJmp(bool condition)
        {
            if (condition)
                Jp();
            else
                GetNextWord();

            IncreaseCycles(4);
        }

        private void Nop()
        {
            IncreaseCycles(4);
        }

        private void Stop()
        {
            GetNextByte();
            State.Halted = true;
        }
    }
}