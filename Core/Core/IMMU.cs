using System;

namespace bEmu.Core
{
    public interface IMMU
    {
        byte this[int addr] { get; set; }
        void LoadProgram(string fileName, int startAddress = 0);
        void LoadProgram(byte[] bytes, int startAddress = 0);
        int Length { get; }
        //ISystem System { get; }
    }
}