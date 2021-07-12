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
    }
}