using bEmu.Core.Util;

namespace bEmu.Core.CPUs.LR35902
{
    public abstract partial class LR35902<TState, TMMU> : CPU<TState, TMMU> 
        where TState : State
        where TMMU : MMU
    {
        protected void Jp(ushort addr)
        {
            State.PC = addr;
            IncreaseCycles(10);
        }

        protected void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                IncreaseCycles(12);
            }
        }

        protected void Call(ushort addr)
        {
            PushStack(State.PC);
            Jp(addr);
        }

        protected void Call()
        {
            ushort addr = GetNextWord();
            PushStack(State.PC);
            Jp(addr);
            IncreaseCycles(14);
        }

        protected void CallC()
        {
            ConditionalCall(State.Flags.Carry);
        }

        protected void CallZ()
        {
            ConditionalCall(State.Flags.Zero);
        }

        protected void Adc()
        {
            byte value = GetNextByte();
            int result = State.A + value;

            if (State.Flags.Carry)
                result++;

            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            IncreaseCycles(8);
        }

        protected void Sbc()
        {
            byte value = GetNextByte();
            int result = State.A - value;

            if (State.Flags.Carry)
                result--;

            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result); 
            State.A = (byte) result;
            State.Flags.Subtract = true;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(8);   
        }

        protected void Xor()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.A ^= value;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = false;
            IncreaseCycles(8);
        }

        protected void Cp()
        {
            byte value = GetNextByte();                
            byte result = (byte) (State.A - value);
            State.Flags.Carry = value > State.A;
            State.Flags.Zero = CheckZero(result);
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, result);
            IncreaseCycles(8);
        }

        protected void Ei()
        {
            State.EnableInterrupts = true;
            IncreaseCycles(4);
        }

        protected void RetZ()
        {
            ConditionalRet(State.Flags.Zero);
        }

        protected void RetC()
        {
            ConditionalRet(State.Flags.Carry);
        }

        protected void AddSP()
        {
            sbyte value = (sbyte) GetNextByte();
            ushort sp = State.SP;
            State.SP = (ushort) (sp + value);

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.Carry = (((sp ^ value ^ State.SP) & 0x100) == 0x100); 
            State.Flags.HalfCarry = CheckHalfCarry(sp, (ushort) value, State.SP);
            
            IncreaseCycles(16);
        }

        protected void Ld_HL_SPr8()
        {
            sbyte value = (sbyte) GetNextByte();
            ushort sp = State.SP;
            State.HL = (ushort) (State.SP + value);

            State.Flags.Zero = false;
            State.Flags.Subtract = false;
            State.Flags.Carry = (((sp ^ value ^ State.HL) & 0x100) == 0x100); 
            State.Flags.HalfCarry = CheckHalfCarry(sp, (ushort) value, State.HL);

            IncreaseCycles(12);
        }

        protected void Ret()
        {
            State.PC = PopStack();
            IncreaseCycles(16);
        }

        protected void Reti()
        {
            Ret();
            State.EnableInterrupts = true;
        }

        protected void Jp_HL()
        {
            Jp(State.HL);
            IncreaseCycles(4);
        }

        protected void Ld_SPHL()
        {
            State.SP = State.HL;
            IncreaseCycles(8);
        }

        protected void JpZ()
        {
            ConditionalJmp(State.Flags.Zero);
        }

        protected void Ld_a16_A()
        {
            MMU[GetNextWord()] = State.A;
            IncreaseCycles(16);
        }

        protected void Ld_A_a16()
        {
            State.A = MMU[GetNextWord()];
            IncreaseCycles(16);
        }

        protected void Add_A_d8()
        {
            byte value = GetNextByte();
            int result = State.A + value;

            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;

            IncreaseCycles(8);
        }

        protected void Sub_d8()
        {
            byte value = GetNextByte();
            int result = State.A - value;
            State.Flags.Carry = result < 0; 
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            IncreaseCycles(7);   
        }

        protected void And_d8()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.Flags.HalfCarry = true;
            State.Flags.Subtract = false;
            State.A &= value;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(8);
        }

        protected void Or_d8()
        {
            byte value = GetNextByte();
            State.Flags.Carry = false;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;
            State.A |= value;
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(8);
        }

        protected void Rst(ushort addr)
        {
            Call(addr);
            IncreaseCycles(6);
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

            IncreaseCycles(16);
        }

        protected void CallNC()
        {
            ConditionalCall(!State.Flags.Carry);
        }

        protected void CallNZ()
        {
            ConditionalCall(!State.Flags.Zero);
        }

        protected void JpNZ()
        {
            ConditionalJmp(!State.Flags.Zero);
        }

        protected void JpNC()
        {
            ConditionalJmp(!State.Flags.Carry);
        }

        protected void Ld_C_A()
        {
            MMU[0xFF00 + State.C] = State.A;
            IncreaseCycles(8);
        }

        protected void Ld_A_C()
        {
            State.A = MMU[0xFF00 + State.C];
            IncreaseCycles(8);
        }

        protected void JpC()
        {
            ConditionalJmp(State.Flags.Carry);
        }

        protected void Jp()
        {
            ushort addr = GetNextWord();
            State.PC = addr;
            IncreaseCycles(16);
        }

        protected void Di()
        {
            State.EnableInterrupts = false;
            IncreaseCycles(4);
        }

        protected void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: State.BC = PopStack(); break;
                case Register.DE: State.DE = PopStack(); break;
                case Register.HL: State.HL = PopStack(); break;
            }

            IncreaseCycles(12);
        }

        protected void PopPsw()
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

        protected void Ldh_A_a8()
        {
            State.A = MMU[0xFF00 | GetNextByte()];
            IncreaseCycles(12);
        }

        protected void Ldh_a8_A()
        {
            MMU[0xFF00 | GetNextByte()] = State.A;
            IncreaseCycles(12);
        }

        protected void RetNC()
        {
            ConditionalRet(!State.Flags.Carry);
        }

        protected void RetNZ()
        {
            ConditionalRet(!State.Flags.Zero);
        }

        protected void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                IncreaseCycles(4);
            }
            else
                IncreaseCycles(8);
        }

        protected void Cp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A - value;
            State.Flags.Carry = result < 0;
            State.Flags.Zero = CheckZero((byte) result);
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);

            IncreaseCycles(4);
        }

        protected void Or(Register register)
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

        protected void Xor(Register register)
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

        protected void And(Register register)
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

        protected void Sbc(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A - value;

            if (State.Flags.Carry)
                result--;

            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            IncreaseCycles(4);
        }

        protected void Sub(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A - value;
            State.Flags.Carry = result < 0;
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = true;
            IncreaseCycles(4);   
        }

        protected void Adc(Register register)
        {
            if (register == Register.HL)
                IncreaseCycles(4);

            byte value = GetByteFromRegister(register);
            int result = State.A + value;

            if (State.Flags.Carry)
                result++;

            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) result;
            State.Flags.Zero = CheckZero(State.A);
            State.Flags.Subtract = false;
            IncreaseCycles(4);
        }

        protected void Add(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                IncreaseCycles(4);

            int result = State.A + value;

            State.Flags.Subtract = false;
            State.Flags.Carry = (result & 0x100) == 0x100; 
            State.Flags.HalfCarry = CheckHalfCarry(State.A, value, (byte) result);
            State.A = (byte) (result);
            State.Flags.Zero = CheckZero(State.A);
            IncreaseCycles(4);
        }

        protected void Halt()
        {
            State.Halted = true;
            IncreaseCycles(4);
        }

        protected void Ld(Register registerA, Register registerB)
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
                case Register.HL: WriteByteToMemory(State.HL, value); break;
            }

            IncreaseCycles(4);
        }

        protected void Ccf()
        {
            State.Flags.Carry = !State.Flags.Carry;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        protected void Cpl()
        {
            State.A = (byte) ~State.A;
            State.Flags.Subtract = true;
            State.Flags.HalfCarry = true;
            IncreaseCycles(4);
        }

        protected void Rra()
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

        protected void Rrca()
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

        protected void LdA(Register register, Action action)
        {
            byte value = GetByteFromRegister(register);
            State.A = value;

            if (action == Action.Increment)
                State.HL++;
            else if (action == Action.Decrement)
                State.HL--;

            IncreaseCycles(8);
        }

        protected void AddHL(Register register)
        {
            ushort word = GetWordFromRegister(register);
            int result = (State.HL + word);
            State.Flags.Carry = (((State.HL ^ word ^ result) & 0x10000) == 0x10000);
            State.Flags.Subtract = false;
            State.Flags.HalfCarry = (((State.HL ^ word ^ result) & 0x1000) == 0x1000);
            State.HL += word;
            IncreaseCycles(8);
        }

        protected void Jrc()
        {
            ConditionalJr(State.Flags.Carry);
        }

        protected void Jrz()
        {
            ConditionalJr(State.Flags.Zero);
        }

        protected void Jr()
        {
            sbyte value = (sbyte) GetNextByte();
            int result = State.PC + value;
            State.PC = (ushort) result;
            IncreaseCycles(12);
        }

        protected void ConditionalJr(bool condition)
        {
            if (condition)
                Jr();
            else
            {
                GetNextByte();
                IncreaseCycles(8);
            }
        }

        protected void Ld_SP()
        {
            var addr = GetNextWord();
            BitUtils.Get2BytesFromWord(State.SP, out byte msb, out byte lsb);
            MMU[addr++] = lsb;
            MMU[addr++] = msb;

            IncreaseCycles(20);
        }

        protected void Scf()
        {
            State.Flags.Carry = true;
            State.Flags.HalfCarry = false;
            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        protected void Daa()
        {
            if (!State.Flags.Subtract) 
            {
                if (State.Flags.Carry || State.A > 0x99) 
                { 
                    State.A += 0x60; 
                    State.Flags.Carry = true; 
                }

                if (State.Flags.HalfCarry || (State.A & 0x0F) > 0x09) 
                    State.A += 0x6; 
            } 
            else 
            {
                if (State.Flags.Carry) 
                    State.A -= 0x60; 

                if (State.Flags.HalfCarry) 
                    State.A -= 0x6; 
            }

            State.Flags.Zero = CheckZero(State.A);
            State.Flags.HalfCarry = false;

            IncreaseCycles(4);
        }

        protected void Rla()
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

        protected void Rlca()
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

        protected void Ld_d8(Register register)
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

        protected void DecRegPair(Register register)
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

        protected void DecRef()
        {
            byte reference = MMU[State.HL];
            MMU[State.HL]--;
            IncreaseCycles(12);
            State.Flags.Zero = CheckZero(MMU[State.HL]);
            State.Flags.HalfCarry = CheckHalfCarry(reference, (byte) 1, MMU[State.HL]);
            State.Flags.Subtract = true;
        }

        protected void Dec(Register register)
        {
            byte regValue;

            switch (register)
            {
                case Register.A: 
                    regValue = State.A;
                    State.A--; 
                    State.Flags.Zero = CheckZero(State.A);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.A);
                    break;
                case Register.B: 
                    regValue = State.B;
                    State.B--; 
                    State.Flags.Zero = CheckZero(State.B);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.B);
                    break;
                case Register.C: 
                    regValue = State.C;
                    State.C--; 
                    State.Flags.Zero = CheckZero(State.C);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.C);
                    break;
                case Register.D: 
                    regValue = State.D;
                    State.D--; 
                    State.Flags.Zero = CheckZero(State.D);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.D);
                    break;
                case Register.E: 
                    regValue = State.E;
                    State.E--; 
                    State.Flags.Zero = CheckZero(State.E);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.E);
                    break;
                case Register.H: 
                    regValue = State.H;
                    State.H--; 
                    State.Flags.Zero = CheckZero(State.H);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.H);
                    break;
                case Register.L: 
                    regValue = State.L;
                    State.L--; 
                    State.Flags.Zero = CheckZero(State.L);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.L);
                    break;
            }

            State.Flags.Subtract = true;
            IncreaseCycles(4);
        }

        protected void IncRegPair(Register register)
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

        protected void IncRef()
        {
            byte reference = MMU[State.HL];
            MMU[State.HL]++;
            IncreaseCycles(12);

            State.Flags.Zero = CheckZero(MMU[State.HL]);
            State.Flags.HalfCarry = CheckHalfCarry(reference, (byte) 1, MMU[State.HL]);
            State.Flags.Subtract = false;
        }

        protected void Inc(Register register)
        {
            byte regValue;
            switch (register)
            {
                case Register.A: 
                    regValue = State.A;
                    State.A++; 
                    State.Flags.Zero = CheckZero(State.A);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.A);
                    break;
                case Register.B: 
                    regValue = State.B;
                    State.B++; 
                    State.Flags.Zero = CheckZero(State.B);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.B);
                    break;
                case Register.C: 
                    regValue = State.C;
                    State.C++; 
                    State.Flags.Zero = CheckZero(State.C);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.C);
                    break;
                case Register.D: 
                    regValue = State.D;
                    State.D++; 
                    State.Flags.Zero = CheckZero(State.D);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.D);
                    break;
                case Register.E: 
                    regValue = State.E;
                    State.E++; 
                    State.Flags.Zero = CheckZero(State.E);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.E);
                    break;
                case Register.H: 
                    regValue = State.H;
                    State.H++; 
                    State.Flags.Zero = CheckZero(State.H);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.H);
                    break;
                case Register.L: 
                    regValue = State.L;
                    State.L++; 
                    State.Flags.Zero = CheckZero(State.L);
                    State.Flags.HalfCarry = CheckHalfCarry(regValue, (byte) 1, State.L);
                    break;
            }

            State.Flags.Subtract = false;

            IncreaseCycles(4);
        }

        protected void LD_A(Register register, Action action)
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

        protected void LD_d16(Register register)
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

        protected void JrNC()
        {
            ConditionalJr(!State.Flags.Carry);
        }

        protected void JrNZ()
        {
            ConditionalJr(!State.Flags.Zero);
        }

        protected void ConditionalJmp(bool condition)
        {
            if (condition)
                Jp();
            else
            {
                GetNextWord();
                IncreaseCycles(12);
            }            
        }

        protected void Nop()
        {
            IncreaseCycles(4);
        }

        protected virtual void Stop()
        {
            byte value = GetNextByte();

            if (value != 0)
                State.Halted = true;

            IncreaseCycles(4);
        }
    }
}