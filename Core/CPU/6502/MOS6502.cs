using bEmu.Core.Exceptions;
using bEmu.Core.Memory;
using bEmu.Core.System;

namespace bEmu.Core.CPU.MOS6502
{
    public abstract partial class MOS6502<TState, TMMU> : CPU<TState, TMMU>
        where TState : State
        where TMMU : MMU
    {
        public MOS6502(ISystem system, int clock) : base(system, clock) { }

        public override IOpcode StepCycle()
        {
            var opcode = new Opcode(GetNextByte());

            switch (opcode.Lo)
            {
                case 0x0:
                    switch (opcode.Hi)
                    {
                        case 0x0: Brk(AddressMode.Implied); break;
                        case 0x1: Bpl(AddressMode.Relative); break;
                        case 0x2: Jsr(AddressMode.Absolute); break;
                        case 0x3: Bmi(AddressMode.Relative); break;
                        case 0x4: Rti(AddressMode.Implied); break;
                        case 0x5: Bvc(AddressMode.Relative); break;
                        case 0x6: Rts(AddressMode.Implied); break;
                        case 0x7: Bvs(AddressMode.Relative); break;
                        case 0x9: Bcc(AddressMode.Relative); break;
                        case 0xA: Ldy(AddressMode.Immediate); break;
                        case 0xB: Bcs(AddressMode.Relative); break;
                        case 0xC: Cpy(AddressMode.Immediate); break;
                        case 0xD: Bne(AddressMode.Relative); break;
                        case 0xE: Cpx(AddressMode.Immediate); break;
                        case 0xF: Beq(AddressMode.Relative); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x1:
                    switch (opcode.Hi)
                    {
                        case 0x0: Ora(AddressMode.XIndirect); break;
                        case 0x1: Ora(AddressMode.IndirectY); break;
                        case 0x2: And(AddressMode.XIndirect); break;
                        case 0x3: And(AddressMode.IndirectY); break;
                        case 0x4: Eor(AddressMode.XIndirect); break;
                        case 0x5: Eor(AddressMode.IndirectY); break;
                        case 0x6: Adc(AddressMode.XIndirect); break;
                        case 0x7: Adc(AddressMode.IndirectY); break;
                        case 0x8: Sta(AddressMode.XIndirect); break;
                        case 0x9: Sta(AddressMode.IndirectY); break;
                        case 0xA: Lda(AddressMode.XIndirect); break;
                        case 0xB: Lda(AddressMode.IndirectY); break;
                        case 0xC: Cmp(AddressMode.XIndirect); break;
                        case 0xD: Cmp(AddressMode.IndirectY); break;
                        case 0xE: Sbc(AddressMode.XIndirect); break;
                        case 0xF: Sbc(AddressMode.IndirectY); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x2: Ldx(AddressMode.Immediate); break;
                case 0x4:
                    switch (opcode.Hi)
                    {
                        case 0x2: Bit(AddressMode.ZeroPage); break;
                        case 0x8: Sty(AddressMode.ZeroPage); break;
                        case 0x9: Sty(AddressMode.ZeroPageX); break;
                        case 0xA: Ldy(AddressMode.ZeroPage); break;
                        case 0xB: Ldy(AddressMode.ZeroPageX); break;
                        case 0xC: Cpy(AddressMode.ZeroPage); break;
                        case 0xE: Cpx(AddressMode.ZeroPage); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x5:
                    switch (opcode.Hi)
                    {
                        case 0x0: Ora(AddressMode.ZeroPage); break;
                        case 0x1: Ora(AddressMode.ZeroPageX); break;
                        case 0x2: And(AddressMode.ZeroPage); break;
                        case 0x3: And(AddressMode.ZeroPageX); break;
                        case 0x4: Eor(AddressMode.ZeroPage); break;
                        case 0x5: Eor(AddressMode.ZeroPageX); break;
                        case 0x6: Adc(AddressMode.ZeroPage); break;
                        case 0x7: Adc(AddressMode.ZeroPageX); break;
                        case 0x8: Sta(AddressMode.ZeroPage); break;
                        case 0x9: Sta(AddressMode.ZeroPageX); break;
                        case 0xA: Lda(AddressMode.ZeroPage); break;
                        case 0xB: Lda(AddressMode.ZeroPageX); break;
                        case 0xC: Cmp(AddressMode.ZeroPage); break;
                        case 0xD: Cmp(AddressMode.ZeroPageX); break;
                        case 0xE: Sbc(AddressMode.ZeroPage); break;
                        case 0xF: Sbc(AddressMode.ZeroPageX); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x6:
                    switch (opcode.Hi)
                    {
                        case 0x0: Asl(AddressMode.ZeroPage); break;
                        case 0x1: Asl(AddressMode.ZeroPageX); break;
                        case 0x2: Rol(AddressMode.ZeroPage); break;
                        case 0x3: Rol(AddressMode.ZeroPageX); break;
                        case 0x4: Lsr(AddressMode.ZeroPage); break;
                        case 0x5: Lsr(AddressMode.ZeroPageX); break;
                        case 0x6: Ror(AddressMode.ZeroPage); break;
                        case 0x7: Ror(AddressMode.ZeroPageX); break;
                        case 0x8: Stx(AddressMode.ZeroPage); break;
                        case 0x9: Stx(AddressMode.ZeroPageY); break;
                        case 0xA: Ldx(AddressMode.ZeroPage); break;
                        case 0xB: Ldx(AddressMode.ZeroPageY); break;
                        case 0xC: Dec(AddressMode.ZeroPage); break;
                        case 0xD: Dec(AddressMode.ZeroPageX); break;
                        case 0xE: Inc(AddressMode.ZeroPage); break;
                        case 0xF: Inc(AddressMode.ZeroPageX); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x8:
                    switch (opcode.Hi)
                    {
                        case 0x0: Php(AddressMode.Implied); break;
                        case 0x1: Clc(AddressMode.Implied); break;
                        case 0x2: Plp(AddressMode.Implied); break;
                        case 0x3: Sec(AddressMode.Implied); break;
                        case 0x4: Pha(AddressMode.Implied); break;
                        case 0x5: Cli(AddressMode.Implied); break;
                        case 0x6: Pla(AddressMode.Implied); break;
                        case 0x7: Sei(AddressMode.Implied); break;
                        case 0x8: Dey(AddressMode.Implied); break;
                        case 0x9: Tya(AddressMode.Implied); break;
                        case 0xA: Tay(AddressMode.Implied); break;
                        case 0xB: Clv(AddressMode.Implied); break;
                        case 0xC: Iny(AddressMode.Implied); break;
                        case 0xD: Cld(AddressMode.Implied); break;
                        case 0xE: Inx(AddressMode.Implied); break;
                        case 0xF: Sed(AddressMode.Implied); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0x9:
                    switch (opcode.Hi)
                    {
                        case 0x0: Ora(AddressMode.Immediate); break;
                        case 0x1: Ora(AddressMode.AbsoluteY); break;
                        case 0x2: And(AddressMode.Immediate); break;
                        case 0x3: And(AddressMode.AbsoluteY); break;
                        case 0x4: Eor(AddressMode.Immediate); break;
                        case 0x5: Eor(AddressMode.AbsoluteY); break;
                        case 0x6: Adc(AddressMode.Immediate); break;
                        case 0x7: Adc(AddressMode.AbsoluteY); break;
                        case 0x9: Sta(AddressMode.AbsoluteY); break;
                        case 0xA: Lda(AddressMode.Immediate); break;
                        case 0xB: Lda(AddressMode.AbsoluteY); break;
                        case 0xC: Cmp(AddressMode.Immediate); break;
                        case 0xD: Cmp(AddressMode.AbsoluteY); break;
                        case 0xE: Sbc(AddressMode.Immediate); break;
                        case 0xF: Sbc(AddressMode.AbsoluteY); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0xA:
                    switch (opcode.Hi)
                    {
                        case 0x0: Asl(AddressMode.Accumulator); break;
                        case 0x2: Rol(AddressMode.Accumulator); break;
                        case 0x4: Lsr(AddressMode.Accumulator); break;
                        case 0x6: Ror(AddressMode.Accumulator); break;
                        case 0x8: Txa(AddressMode.Implied); break;
                        case 0x9: Txs(AddressMode.Implied); break;
                        case 0xA: Tax(AddressMode.Implied); break;
                        case 0xB: Tsx(AddressMode.Implied); break;
                        case 0xC: Dex(AddressMode.Implied); break;
                        case 0xE: Nop(AddressMode.Implied); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0xC:
                    switch (opcode.Hi)
                    {
                        case 0x2: Bit(AddressMode.Absolute); break;
                        case 0x4: Jmp(AddressMode.Absolute); break;
                        case 0x6: Jmp(AddressMode.Indirect); break;
                        case 0x8: Sty(AddressMode.Absolute); break;
                        case 0xA: Ldy(AddressMode.Absolute); break;
                        case 0xB: Ldy(AddressMode.AbsoluteX); break;
                        case 0xC: Cpy(AddressMode.Absolute); break;
                        case 0xE: Cpx(AddressMode.Absolute); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0xD:
                    switch (opcode.Hi)
                    {
                        case 0x0: Ora(AddressMode.Absolute); break;
                        case 0x1: Ora(AddressMode.AbsoluteX); break;
                        case 0x2: And(AddressMode.Absolute); break;
                        case 0x3: And(AddressMode.AbsoluteX); break;
                        case 0x4: Eor(AddressMode.Absolute); break;
                        case 0x5: Eor(AddressMode.AbsoluteX); break;
                        case 0x6: Adc(AddressMode.Absolute); break;
                        case 0x7: Adc(AddressMode.AbsoluteX); break;
                        case 0x8: Sta(AddressMode.Absolute); break;
                        case 0x9: Sta(AddressMode.AbsoluteX); break;
                        case 0xA: Lda(AddressMode.Absolute); break;
                        case 0xB: Lda(AddressMode.AbsoluteX); break;
                        case 0xC: Cmp(AddressMode.Absolute); break;
                        case 0xD: Cmp(AddressMode.AbsoluteX); break;
                        case 0xE: Sbc(AddressMode.Absolute); break;
                        case 0xF: Sbc(AddressMode.AbsoluteX); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                case 0xE:
                    switch (opcode.Hi)
                    {
                        case 0x0: Asl(AddressMode.Absolute); break;
                        case 0x1: Asl(AddressMode.AbsoluteX); break;
                        case 0x2: Rol(AddressMode.Absolute); break;
                        case 0x3: Rol(AddressMode.AbsoluteX); break;
                        case 0x4: Lsr(AddressMode.Absolute); break;
                        case 0x5: Lsr(AddressMode.AbsoluteX); break;
                        case 0x6: Ror(AddressMode.Absolute); break;
                        case 0x7: Ror(AddressMode.AbsoluteX); break;
                        case 0x8: Stx(AddressMode.Absolute); break;
                        case 0xA: Ldx(AddressMode.Absolute); break;
                        case 0xB: Ldx(AddressMode.AbsoluteY); break;
                        case 0xC: Dec(AddressMode.Absolute); break;
                        case 0xD: Dec(AddressMode.AbsoluteX); break;
                        case 0xE: Inc(AddressMode.Absolute); break;
                        case 0xF: Inc(AddressMode.AbsoluteX); break;
                        default: throw new OpcodeNotImplementedException(opcode);
                    }
                    break;
                default: throw new OpcodeNotImplementedException(opcode);
            }

            return opcode;
        }
    }
}