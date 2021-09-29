using System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU.MOS6502
{
    public abstract partial class MOS6502<TState, TMMU>
    {
        private void Jmp(AddressMode mode)
        {
            if (mode == AddressMode.Indirect)
                IncreaseCycles(2);

            var addr = GetAddressByMode(mode);            
            State.PC = addr;

            IncreaseCycles(3);
        }

        private void Nop(AddressMode mode)
        {
            IncreaseCycles(2);
        }

        private void Dex(AddressMode mode)
        {
            State.X--;
            UpdateNegativeAndZero(State.X);
            
            IncreaseCycles(2);
        }

        private void Tsx(AddressMode mode)
        {
            State.X = State.SP;
            UpdateNegativeAndZero(State.X);

            IncreaseCycles(2);
        }

        private void Tax(AddressMode mode)
        {
            State.X = State.A;
            UpdateNegativeAndZero(State.X);

            IncreaseCycles(2);
        }

        private void Txs(AddressMode mode)
        {
            State.SP = State.X;
            IncreaseCycles(2);
        }

        private void Txa(AddressMode mode)
        {
            State.A = State.X;
            UpdateNegativeAndZero(State.A);

            IncreaseCycles(2);
        }

        private void Sed(AddressMode mode)
        {
            State.Flags.Decimal = true;
            IncreaseCycles(2);
        }

        private void Inx(AddressMode mode)
        {
            State.X++;
            UpdateNegativeAndZero(State.X);
            
            IncreaseCycles(2);
        }

        private void Cld(AddressMode mode)
        {
            State.Flags.Decimal = false;
            IncreaseCycles(2);
        }

        private void Iny(AddressMode mode)
        {
            State.Y++;
            UpdateNegativeAndZero(State.Y);

            IncreaseCycles(2);
        }

        private void Clv(AddressMode mode)
        {
            State.Flags.Overflow = false;
            IncreaseCycles(2);
        }

        private void Tay(AddressMode mode)
        {
            State.Y = State.A;
            UpdateNegativeAndZero(State.Y);

            IncreaseCycles(2);
        }

        private void Tya(AddressMode mode)
        {
            State.A = State.Y;
            UpdateNegativeAndZero(State.A);

            IncreaseCycles(2);
        }

        private void Dey(AddressMode mode)
        {
            State.Y--;
            UpdateNegativeAndZero(State.Y);

            IncreaseCycles(2);
        }

        private void Sei(AddressMode mode)
        {
            State.Flags.DisableInterrupt = true;
            IncreaseCycles(2);
        }

        private void Pla(AddressMode mode)
        {
            State.A = PullStack();
            UpdateNegativeAndZero(State.A);
            IncreaseCycles(4);
        }

        private void Cli(AddressMode mode)
        {
            State.Flags.DisableInterrupt = false;
            IncreaseCycles(2);
        }

        private void Pha(AddressMode mode)
        {
            PushStack(State.A);
            IncreaseCycles(3);
        }

        private void Sec(AddressMode mode)
        {
            State.Flags.Carry = true;
            IncreaseCycles(2);
        }

        private void Plp(AddressMode mode)
        {
            State.SR = PullStack();
            IncreaseCycles(4);
        }

        private void Clc(AddressMode mode)
        {
            State.Flags.Carry = false;
            IncreaseCycles(2);
        }

        private void Php(AddressMode mode)
        {
            PushStack(State.SR);
            IncreaseCycles(3);
        }

        private void Inc(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            MMU[addr]++;
            UpdateNegativeAndZero(MMU[addr]);

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 5; break;
                case AddressMode.ZeroPageX: cycles = 6; break;
                case AddressMode.Absolute: cycles = 6; break;
                case AddressMode.AbsoluteX: cycles = 7; break;
            }

            IncreaseCycles(cycles);
        }

        private void Dec(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            MMU[addr]--;

            UpdateNegativeAndZero(MMU[addr]);
            State.Flags.Carry = MMU[addr] == 0xFF;

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 5; break;
                case AddressMode.ZeroPageX: cycles = 6; break;
                case AddressMode.Absolute: cycles = 3; break;
                case AddressMode.AbsoluteX: cycles = 7; break;
            }

            IncreaseCycles(cycles);
        }

        private void Stx(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            MMU[addr] = State.X;

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 3; break;
                case AddressMode.ZeroPageY: cycles = 4; break;
                case AddressMode.Absolute: cycles = 4; break;
            }

            IncreaseCycles(cycles);
        }

        private void Ror(AddressMode mode)
        {
            if (mode == AddressMode.Accumulator)
            {
                bool carry = (State.A & 0x1) == 0x1;
                State.A = (byte)((State.Flags.Carry ? 0x80 : 0x00) | (State.A >> 1));
                UpdateNegativeAndZero(State.A);
                State.Flags.Carry = carry;

                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                bool carry = (MMU[addr] & 0x1) == 0x1;
                MMU[addr] = (byte)((State.Flags.Carry ? 0x80 : 0x00) | (MMU[addr] >> 1));
                UpdateNegativeAndZero(MMU[addr]);
                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 5; break;
                    case AddressMode.ZeroPageX: cycles = 6; break;
                    case AddressMode.Absolute: cycles = 6; break;
                    case AddressMode.AbsoluteX: cycles = 7; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Lsr(AddressMode mode)
        {
            if (mode == AddressMode.Accumulator)
            {
                bool carry = (State.A & 0x1) == 0x1;
                State.A >>= 1;
                State.Flags.Carry = carry;
                UpdateNegativeAndZero(State.A);

                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                bool carry = (MMU[addr] & 0x1) == 0x1;
                MMU[addr] >>= 1;
                UpdateNegativeAndZero(MMU[addr]);
                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 5; break;
                    case AddressMode.ZeroPageX: cycles = 6; break;
                    case AddressMode.Absolute: cycles = 6; break;
                    case AddressMode.AbsoluteX: cycles = 7; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Rol(AddressMode mode)
        {
            if (mode == AddressMode.Accumulator)
            {
                bool carry = (State.A & 0x80) == 0x80;
                State.A = (byte)((State.Flags.Carry ? 0x1 : 0x0) | (State.A << 1));
                State.Flags.Carry = carry;
                UpdateNegativeAndZero(State.A);

                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                bool carry = (MMU[addr] & 0x80) == 0x80;
                MMU[addr] = (byte)((State.Flags.Carry ? 0x1 : 0x0) | (MMU[addr] << 1));
                UpdateNegativeAndZero(MMU[addr]);
                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 5; break;
                    case AddressMode.ZeroPageX: cycles = 6; break;
                    case AddressMode.Absolute: cycles = 6; break;
                    case AddressMode.AbsoluteX: cycles = 7; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Asl(AddressMode mode)
        {
            if (mode == AddressMode.Accumulator)
            {
                bool carry = (State.A & 0x80) == 0x80;
                State.A <<= 1;
                State.Flags.Carry = carry;
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                bool carry = (MMU[addr] & 0x80) == 0x80;
                MMU[addr] <<= 1;
                UpdateNegativeAndZero(MMU[addr]);
                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 5; break;
                    case AddressMode.ZeroPageX: cycles = 6; break;
                    case AddressMode.Absolute: cycles = 6; break;
                    case AddressMode.AbsoluteX: cycles = 7; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Sty(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            MMU[addr] = State.Y;

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 3; break;
                case AddressMode.ZeroPageX: cycles = 4; break;
                case AddressMode.Absolute: cycles = 4; break;
            }

            IncreaseCycles(cycles);
        }

        private void Bit(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            State.Flags.Negative = (MMU[addr] & 0x80) == 0x80;
            State.Flags.Overflow = (MMU[addr] & 0x40) == 0x40;
            State.Flags.Zero = CheckZero((byte)(MMU[addr] & State.A));

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 3; break;
                case AddressMode.Absolute: cycles = 4; break;
            }

            IncreaseCycles(cycles);
        }

        private void Ldx(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.X = MMU[GetNextByte()];
                UpdateNegativeAndZero(State.X);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.X = MMU[addr];
                UpdateNegativeAndZero(State.X);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 3; break;
                    case AddressMode.ZeroPageY: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Sbc(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                byte b = GetNextByte();
                var result = (State.A - MMU[b] - (State.Flags.Carry ? 1 : 0));
                State.Flags.Carry = result < 0;
                State.Flags.Overflow = result < -0x80;
                State.A = (byte) result;
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                var result = (State.A - MMU[addr] - (State.Flags.Carry ? 1 : 0));
                State.Flags.Carry = result < 0;
                State.Flags.Overflow = result < -0x80;
                State.A = (byte) result;
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Cmp(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                byte b = GetNextByte();
                var result = (byte)(State.A - MMU[b]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.A < MMU[b];
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                var result = (byte)(State.A - MMU[addr]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.A < MMU[addr];

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Lda(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.A = MMU[GetNextByte()];
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.A = MMU[addr];
                UpdateNegativeAndZero(State.A);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Sta(AddressMode mode)
        {
            var addr = GetAddressByMode(mode);
            MMU[addr] = State.A;

            sbyte cycles = 0;

            switch (mode)
            {
                case AddressMode.ZeroPage: cycles = 3; break;
                case AddressMode.ZeroPageX: cycles = 4; break;
                case AddressMode.Absolute: cycles = 4; break;
                case AddressMode.AbsoluteX: cycles = 5; break;
                case AddressMode.AbsoluteY: cycles = 5; break;
                case AddressMode.XIndirect: cycles = 6; break;
                case AddressMode.IndirectY: cycles = 6; break;
            }

            IncreaseCycles(cycles);
        }

        private void Adc(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                byte b = GetNextByte();
                var result = (State.A + MMU[b] + (State.Flags.Carry ? 1 : 0));
                State.Flags.Carry = result > 0xFF;
                State.Flags.Overflow = result > 0x7F;
                State.A = (byte) result;
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                var result = (State.A + MMU[addr] + (State.Flags.Carry ? 1 : 0));
                State.Flags.Carry = result > 0xFF;
                State.Flags.Overflow = result > 0x7F;
                State.A = (byte) result;
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Eor(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.A ^= MMU[GetNextByte()];
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.A ^= MMU[addr];
                UpdateNegativeAndZero(State.A);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void And(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.A &= MMU[GetNextByte()];
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.A &= MMU[addr];
                UpdateNegativeAndZero(State.A);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Ora(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.A |= MMU[GetNextByte()];
                UpdateNegativeAndZero(State.A);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.A |= MMU[addr];
                UpdateNegativeAndZero(State.A);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 2; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                    case AddressMode.AbsoluteY: cycles = 4; break;
                    case AddressMode.XIndirect: cycles = 6; break;
                    case AddressMode.IndirectY: cycles = 5; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Beq(AddressMode mode)
        {
            ConditionalBranch(State.Flags.Zero);
        }

        private void Cpx(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                byte b = GetNextByte();
                var result = (byte)(State.X - MMU[b]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.X < MMU[b];
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                var result = (byte)(State.X - MMU[addr]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.X < MMU[addr];

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Bne(AddressMode mode)
        {
            ConditionalBranch(!State.Flags.Zero);
        }

        private void Cpy(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                byte b = GetNextByte();
                var result = (byte)(State.Y - MMU[b]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.Y < MMU[b];
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                var result = (byte)(State.Y - MMU[addr]);
                UpdateNegativeAndZero(result);
                State.Flags.Carry = State.Y < MMU[addr];

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                }

                IncreaseCycles(cycles);
            }
        }

        protected void Branch()
        {
            ushort addr = GetNextWord();
            
            LittleEndian.Get2BytesFromWord(State.PC, out byte lsb, out byte msb);
            PushStack(lsb);
            PushStack(msb);
            State.PC = addr;
        }

        protected void ConditionalBranch(bool condition)
        {
            if (condition)
                Branch();
            else
                GetNextWord();

            IncreaseCycles(2);
        }

        private void Bcs(AddressMode mode)
        {
            ConditionalBranch(State.Flags.Carry);
        }

        private void Ldy(AddressMode mode)
        {
            if (mode == AddressMode.Immediate)
            {
                State.Y = MMU[GetNextByte()];
                UpdateNegativeAndZero(State.Y);
                IncreaseCycles(2);
            }
            else
            {
                var addr = GetAddressByMode(mode);
                State.Y = MMU[addr];
                UpdateNegativeAndZero(State.Y);

                sbyte cycles = 0;

                switch (mode)
                {
                    case AddressMode.ZeroPage: cycles = 3; break;
                    case AddressMode.ZeroPageX: cycles = 3; break;
                    case AddressMode.Absolute: cycles = 4; break;
                    case AddressMode.AbsoluteX: cycles = 4; break;
                }

                IncreaseCycles(cycles);
            }
        }

        private void Bcc(AddressMode mode)
        {
            ConditionalBranch(!State.Flags.Carry);
        }

        private void Bvs(AddressMode mode)
        {
            ConditionalBranch(State.Flags.Overflow);
        }

        private void Rts(AddressMode mode)
        {
            State.PC = LittleEndian.GetWordFrom2Bytes(PullStack(), PullStack());
            IncreaseCycles(6);
        }

        private void Bvc(AddressMode mode)
        {
            ConditionalBranch(!State.Flags.Overflow);
        }

        private void Rti(AddressMode mode)
        {
            bool breakF = State.Flags.Break;
            State.SR = PullStack();
            State.SR |= 0x20;
            State.SR &= (byte) (breakF ? 0x10 : 0xEF); 
            State.PC = LittleEndian.GetWordFrom2Bytes(PullStack(), PullStack());

            IncreaseCycles(6);
        }

        private void Bmi(AddressMode mode)
        {
            ConditionalBranch(State.Flags.Negative);
        }

        private void Jsr(AddressMode mode)
        {
            if (mode == AddressMode.Indirect)
                IncreaseCycles(2);

            var addr = GetAddressByMode(mode);
            PushStack((byte)(addr >> 4));
            State.PC = addr;

            IncreaseCycles(3);
        }

        private void Bpl(AddressMode mode)
        {
            ConditionalBranch(!State.Flags.Negative);
        }

        private void Brk(AddressMode mode)
        {
            LittleEndian.Get2BytesFromWord(State.PC, out byte msb, out byte lsb);
            PushStack(msb);
            PushStack(lsb);
            PushStack((byte)(State.SR | 0x10));
            State.PC = (ushort)((MMU[IrqVectorL] << 8) | MMU[IrqVectorH]);
            IncreaseCycles(7);
        }

        public void Rst()
        {
            State.A = 0x00;
            State.Y = 0x00;
            State.X = 0x00;
            State.PC = (ushort)((MMU[RstVectorL] << 8) | MMU[RstVectorH]);
            State.SP = 0xFD;
        }

        public void Irq()
        {
            if (!State.Flags.DisableInterrupt)
            {
                State.Flags.Break = false;
                LittleEndian.Get2BytesFromWord(State.PC, out byte msb, out byte lsb);
                PushStack(msb);
                PushStack(lsb);
                PushStack(State.SR);
                State.Flags.DisableInterrupt = true;
                State.PC = (ushort)((MMU[IrqVectorL] << 8) | MMU[IrqVectorH]);
            }
        }

        public void Nmi()
        {
            State.Flags.Break = false;
            LittleEndian.Get2BytesFromWord(State.PC, out byte msb, out byte lsb);
            PushStack(msb);
            PushStack(lsb);
            PushStack(State.SR);
            State.Flags.DisableInterrupt = true;
            State.PC = (ushort)((MMU[NmiVectorL] << 8) | MMU[NmiVectorH]);
        }
    }
}