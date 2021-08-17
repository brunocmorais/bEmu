using System.Collections.Generic;

namespace bEmu.Core.CPU
{
    public interface IDisassembler
    {
        Instruction GetInstruction(int pointer);
        IEnumerable<Instruction> GetInstructions();
    }
}