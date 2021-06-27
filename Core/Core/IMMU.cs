using System;

namespace bEmu.Core
{
    public interface IMMU : ISaveable
    {
        ISystem System { get; }
        byte this[int addr] { get; set; }
        void LoadProgram();
        void LoadProgram(byte[] bytes, int startAddress = 0);
        int Length { get; }
    }
}