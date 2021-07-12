namespace bEmu.Core.CPU
{
    public interface IDisassembler
    {
        Instruction GetInstruction(int pointer);
    }
}