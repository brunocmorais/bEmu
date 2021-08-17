using System.Collections.Generic;
using bEmu.Core.Memory;

namespace bEmu.Core.CPU
{
    public abstract class Disassembler : IDisassembler
    {
        protected readonly IMMU mmu;

        public Disassembler(IMMU mmu)
        {
            this.mmu = mmu;
        }

        public abstract Instruction GetInstruction(int pointer);

        public virtual IEnumerable<Instruction> GetInstructions()
        {
            int pointer = 0;

            while (pointer < mmu.Length)
            {
                var instruction = GetInstruction(pointer);
                pointer += instruction.Length;
                yield return instruction;
            } 
        }
    }
}