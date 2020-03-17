using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bEmu.Core
{
    public abstract class Disassembler : IDisassembler
    {
        protected readonly ISystem system;

        public Disassembler(ISystem system)
        {
            this.system = system;
        }

        public abstract Instruction GetInstruction(int pointer);
    }
}