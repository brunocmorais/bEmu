using System;

namespace bEmu.Core.CPU.MOS6502
{
    public abstract partial class MOS6502<TState, TMMU>
    {
        private void Jmp(AddressMode mode)
        {
            var word = GetNextWord();

            if (mode == AddressMode.Absolute)
                State.PC = word;
            else
            {
                State.PC = MMU[word];
                IncreaseCycles(2);
            }

            IncreaseCycles(3);
        }

        private void Nop(AddressMode mode)
        {
            IncreaseCycles(2);
        }

        private void Dex(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Tsx(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Tax(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Txs(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Txa(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sed(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Inx(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Cld(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Iny(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Clv(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Tay(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Tya(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Dey(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sei(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Pla(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Cli(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Pha(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sec(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Plp(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Clc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Php(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Inc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Dec(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Stx(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Ror(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Lsr(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Rol(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Asl(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sty(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bit(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Ldx(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sbc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Cmp(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Lda(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Sta(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Adc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Eor(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void And(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Ora(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Beq(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Cpx(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bne(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Cpy(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bcs(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Ldy(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bcc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bvs(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Rts(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bvc(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Rti(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bmi(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Jsr(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Bpl(AddressMode mode)
        {
            throw new NotImplementedException();
        }

        private void Brk(AddressMode mode)
        {
            throw new NotImplementedException();
        }
    }
}