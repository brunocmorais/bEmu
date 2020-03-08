using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bEmu.Core
{
    public abstract class Disassembler : IDisassembler
    {
        protected readonly byte[] codeBuffer;
        
        public Disassembler(string fileName)
        {
            codeBuffer = File.ReadAllBytes(fileName);
        }

        public Disassembler(byte[] codebuffer)
        {
            this.codeBuffer = codebuffer;
        }

        public abstract Instruction GetInstruction(int pointer);
        public IEnumerable<Instruction> GetInstructions()
        {
            int pointer = 0;

            while (pointer < codeBuffer.Length)
            {
                var instruction = GetInstruction(pointer);
                pointer += instruction.Length;
                yield return instruction;
            }
        }

        public string GetInstructionsText()
        {
            var sb = new StringBuilder();
            var instructions = GetInstructions();

            foreach (var instruction in instructions)
                sb.AppendLine(instruction.ToString());

            return sb.ToString();
        }
    }
}