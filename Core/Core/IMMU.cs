using System;

namespace bEmu.Core
{
    public interface IMMU : ISaveable
    {
        ISystem System { get; }
        byte this[int addr] { get; set; }
        void LoadProgram(int startAddress = 0);
        void LoadProgram(byte[] bytes, int startAddress = 0);
        int Length { get; }
    }
}