using System.Reflection;

namespace bEmu.Core.System
{
    public interface IDebugger
    {
        ISystem System { get; }
        int BreakpointAddress { get; set; }
        int AccessBreakpointAddress { get; set; }
        bool IsStopped { get; set; }

        byte GetByteFromMemoryAddress(int address);
        PropertyInfo[] GetRegisters();
        string GetRegisterValue(string registerName);
        ushort GetWordFromMemoryAddress(int address);
        string PrintRegisters();
        void SetByteToMemoryAddress(int address, byte value);
        void SetWordToMemoryAddress(int address, ushort word);
        void StepInstruction();
    }
}