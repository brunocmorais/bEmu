using System.Collections.Generic;
using bEmu.Core;

namespace bEmu.Core
{
    public interface IDisassembler
    {
        Instruction GetInstruction(int pointer);
    }
}