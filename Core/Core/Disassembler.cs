using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bEmu.Core
{
    public abstract class Disassembler : IDisassembler
    {
        protected readonly IMMU mmu;

        public Disassembler(IMMU mmu)
        {
            this.mmu = mmu;
        }

        public abstract Instruction GetInstruction(int pointer);
    }
}