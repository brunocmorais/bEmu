using System;

namespace Intel8080
{
    public partial class CPU
    {
        public void Nop()
        {
            state.Cycles += 4;
        }

        public void Lxi(Register register)
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
                    state.SP = Util.Get16BitNumber(state.Memory[state.PC++], state.Memory[state.PC++]);
                    break;
            }
            
            state.Cycles += 10;
        }

        public void Stax(Register register)
        {
            if (register == Register.BC)
                state.Memory[state.BC] = state.A;
            else if (register == Register.DE)
                state.Memory[state.DE] = state.A;

            state.Cycles += 7;
        }

        public void Shld()
        {
            ushort value = state.HL;
            ushort addr = GetNextWord();
            WriteWordToMemory(addr, value);
            state.Cycles += 16;
        }

        public void Sta()
        {
            WriteByteToMemory(GetNextWord(), state.A);
            state.Cycles += 13;
        }

        public void Inx(Register registers)
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

        public void Inr(Register register)
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

        public void Dcr(Register register)
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

        public void Mvi(Register register)
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

        public void Rlc()
        {
            state.Flags.Carry = ((state.A & 0x80) >> 7) == 1;
            state.A <<= 1;

            if (state.Flags.Carry)
                state.A |= 1;

            state.Cycles += 4;
        }

        public void Rrc()
        {
            state.Flags.Carry = (state.A & 0x1) == 1;
            state.A >>= 1;

            if (state.Flags.Carry)
                state.A |= 0x80;

            state.Cycles += 4;
        }

        public void Ral()
        {
            bool previousCarry = state.Flags.Carry;
            state.Flags.Carry = ((state.A & 0x80) >> 7) == 1;
            state.A <<= 1;

            if (previousCarry)
                state.A |= 1;

            state.Cycles += 4;
        }

        public void Rar()
        {
            bool previousCarry = state.Flags.Carry;
            state.Flags.Carry = (state.A & 0x1) == 1;
            state.A >>= 1;

            if (previousCarry)
                state.A |= 0x80;

            state.Cycles += 4;
        }

        public void Daa()
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

        public void Stc()
        {
            state.Flags.Carry = true;
            state.Cycles += 4;
        }

        public void Dad(Register register)
        {
            ushort word = GetWordFromRegister(register);
            state.Flags.Carry = ((state.HL + word) & 0x10000) == 0x10000;
            state.HL += word;
            state.Cycles += 4;
        }

        public void Ldax(Register register)
        {
            byte value = GetByteFromRegister(register);
            state.A = value;
            state.Cycles += 7;
        }

        public void Lhld()
        {
            ushort addr = GetNextWord();
            state.HL = ReadWordFromMemory(addr);
            state.Cycles += 16;
        }

        public void Lda()
        {
            state.A = ReadByteFromMemory(GetNextWord());
            state.Cycles += 13;
        }

        public void Dcx(Register register)
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

        public void Cma()
        {
            state.A = (byte)~state.A;
            state.Cycles += 4;
        }

        public void Cmc()
        {
            state.Flags.Carry = !state.Flags.Carry;
            state.Cycles += 4;
        }

        public void Mov(Register registerA, Register registerB)
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

        public void Add(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A + value;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value);
            state.Cycles += 4;
        }

        public void Adi()
        {
            byte value = GetNextByte();
            int result = state.A + value;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value);
            state.Cycles += 7;
        }

        public void Aci()
        {
            byte value = GetNextByte();
            int carryValue = state.Flags.Carry ? 1 : 0;
            int result = state.A + value + carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value + carryValue);
            state.Cycles += 7;
        }

        public void Adc(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = state.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A + value + carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = (result & 0x100) == 0x100; 
            state.A = (byte) (state.A + value + carryValue);
            state.Cycles += 4;
        }

        public void Sub(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A - value;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value);
            state.Cycles += 4;   
        }

        public void Sui()
        {
            byte value = GetNextByte();
            int result = state.A - value;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value);
            state.Cycles += 7;   
        }

        public void Sbi()
        {
            byte value = GetNextByte();
            int carryValue = state.Flags.Carry ? 1 : 0;
            int result = state.A - value - carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value - carryValue);
            state.Cycles += 7;   
        }

        public void Sbb(Register register)
        {
            byte value = GetByteFromRegister(register);
            int carryValue = state.Flags.Carry ? 1 : 0;

            if (register == Register.HL)
                state.Cycles += 3;

            int result = state.A - value - carryValue;
            UpdateZSP((byte) result);
            state.Flags.Carry = result < 0; 
            state.A = (byte) (state.A - value - carryValue);
            state.Cycles += 4;   
        }

        public void Ana(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A &= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        public void Ani()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A &= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        public void Xra(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A ^= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        public void Xri()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A ^= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        public void Ora(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            state.Flags.Carry = false;
            state.A |= value;
            UpdateZSP(state.A);
            state.Cycles += 4;
        }

        public void Ori()
        {
            byte value = GetNextByte();
            state.Flags.Carry = false;
            state.A |= value;
            UpdateZSP(state.A);
            state.Cycles += 7;
        }

        public void Cmp(Register register)
        {
            byte value = GetByteFromRegister(register);

            if (register == Register.HL)
                state.Cycles += 3;

            ushort result = (ushort)(state.A - value);
            state.Flags.Carry = (result & 0xF000) == 0xF000;
            UpdateZSP((byte) ((state.A - value)));
            state.Cycles += 4;
        }

        public void Cpi()
        {
            byte value = GetNextByte();
            ushort result = (ushort)(state.A - value);
            state.Flags.Carry = (result & 0xF000) == 0xF000;
            UpdateZSP((byte) ((state.A - value)));
            state.Cycles += 7;
        }

        public void Ret()
        {
            state.PC = PopStack();
            state.Cycles += 10;
        }

        private void ConditionalRet(bool condition)
        {
            if (condition)
            {
                Ret();
                state.Cycles += 1;
            }
            else
                state.Cycles += 5;
        }

        public void Rnz()
        {
            ConditionalRet(!state.Flags.Zero);
        }

        public void Rnc()
        {
            ConditionalRet(!state.Flags.Carry);
        }

        public void Rpo()
        {
            ConditionalRet(!state.Flags.Parity);
        }

        public void Rp()
        {
            ConditionalRet(!state.Flags.Sign);
        }

        public void Rz()
        {
            ConditionalRet(state.Flags.Zero);
        }

        public void Rc()
        {
            ConditionalRet(state.Flags.Carry);
        }

        public void Rpe()
        {
            ConditionalRet(state.Flags.Parity);
        }

        public void Rm()
        {
            ConditionalRet(state.Flags.Sign);
        }

        public void Pop(Register register)
        {
            switch (register)
            {
                case Register.BC: state.BC = PopStack(); break;
                case Register.DE: state.DE = PopStack(); break;
                case Register.HL: state.HL = PopStack(); break;
            }

            state.Cycles += 10;
        }

        public void PopPsw()
        {
            ushort af = PopStack();
            state.A = (byte)(af >> 8);
            byte psw = (byte) (af & 0xFF);
            
            state.Flags.Sign = ((psw >> 7) & 1) == 1;
            state.Flags.Zero = ((psw >> 6) & 1) == 1;
            state.Flags.AuxiliaryCarry = ((psw >> 4) & 1) == 1;
            state.Flags.Parity = ((psw >> 2) & 1) == 1;
            state.Flags.Carry = ((psw >> 7) & 1) == 1;

            state.Cycles += 10;
        }

        public void Push(Register register)
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

        public void Jmp()
        {
            state.PC = GetNextWord();
            state.Cycles += 10;
        }

        private void Jmp(ushort addr)
        {
            state.PC = addr;
            state.Cycles += 10;
        }

        private void ConditionalJmp(bool condition)
        {
            if (condition)
                Jmp();
            else
                GetNextWord();

            state.Cycles += 3;
        }

        public void Jnz()
        {
            ConditionalJmp(!state.Flags.Zero);
        }

        public void Jnc()
        {
            ConditionalJmp(!state.Flags.Carry);
        }

        public void Jpo()
        {
            ConditionalJmp(!state.Flags.Parity);
        }

        public void Jp()
        {
            ConditionalJmp(!state.Flags.Sign);
        }

        public void Jz()
        {
            ConditionalJmp(state.Flags.Zero);
        }

        public void Jc()
        {
            ConditionalJmp(state.Flags.Carry);
        }

        public void Jpe()
        {
            ConditionalJmp(state.Flags.Parity);
        }

        public void Jm()
        {
            ConditionalJmp(state.Flags.Sign);
        }

        public void In()
		{
            byte port = GetNextByte();

			switch(port)
			{
                case 1:
                    state.A = state.Ports.Read1;
                    break;
                case 2:
                    state.A = state.Ports.Read2;
                    break;
                case 3:
                    ushort value = Util.Get16BitNumber(state.Ports.Shift0, state.Ports.Shift1);
                    state.A = (byte)((value >> (8 - state.Ports.Write2)) & 0xFF);
                    break;
                default:
                    break;
			}

            state.Cycles += 10;
		}

        public void Out()
        {
            byte port = GetNextByte();
            switch(port)
			{
                case 2:
                    state.Ports.Write2 = (byte)(state.A & 0x7);
                    break;
                case 4:
                    state.Ports.Shift0 = state.Ports.Shift1;
                    state.Ports.Shift1 = state.A;
                    break;
                default:
                    break;
			}

            state.Cycles += 10;
        }

        public void Xthl()
        {
            ushort value = ReadWordFromMemory(state.SP);
            WriteWordToMemory(state.SP, state.HL);
            state.HL = value;

            state.Cycles += 18;
        }

        public void Di()
        {
            state.EnableInterrupts = false;
            state.Cycles += 4;
        }

        public void Ei()
        {
            state.EnableInterrupts = true;
            state.Cycles += 4;
        }

        public void Call()
        {
            ushort addr = GetNextWord();

            // if (addr == 5)
            // {
            //     if (state.C == 9)
            //     {
            //         ushort offset = (ushort) (((state.D << 8) | (state.E)) + 3);
            //         char c = Convert.ToChar(state.Memory[offset]);

            //         while (c != '$')
            //         {
            //             Console.Write(c);
            //             c = Convert.ToChar(state.Memory[++offset]);
            //         }

            //         Hlt();
            //     }
            // }
            // else
            {
                PushStack(state.PC);
                Jmp(addr);
                state.Cycles += 7;
            }
        }

        private void Call(ushort addr)
        {
            PushStack(state.PC);
            Jmp(addr);
            state.Cycles += 7;
        }

        private void ConditionalCall(bool condition)
        {
            if (condition)
                Call();
            else
            {
                GetNextWord();
                state.Cycles += 11;
            }
        }

        public void Cnz()
        {
            ConditionalCall(!state.Flags.Zero);
        }

        public void Cnc()
        {
            ConditionalCall(!state.Flags.Carry);
        }

        public void Cpo()
        {
            ConditionalCall(!state.Flags.Parity);
        }

        public void Cp()
        {
            ConditionalCall(!state.Flags.Sign);
        }

        public void Cz()
        {
            ConditionalCall(state.Flags.Zero);
        }

        public void Cc()
        {
            ConditionalCall(state.Flags.Carry);
        }

        public void Cpe()
        {
            ConditionalCall(state.Flags.Parity);
        }

        public void Cm()
        {
            ConditionalCall(state.Flags.Sign);
        }

        public void Rst(int num)
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

        public void Pchl()
        {
            state.PC = state.HL;
            state.Cycles += 5;
        }

        public void Sphl()
        {
            state.SP = state.HL;
            state.Cycles += 5;
        }

        public void Xchg()
        {
            ushort de = state.DE;
            state.DE = state.HL;
            state.HL = de;
            state.Cycles += 5;
        }



        public void Hlt()
        {
            state.Halted = true;
            state.Cycles += 7;
        }
    }
}