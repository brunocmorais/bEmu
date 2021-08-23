using System;
using bEmu.Core.System;

namespace bEmu.Core.Memory
{
    public interface IMMU : ISaveable
    {
        IRunnableSystem System { get; }
        byte this[int addr] { get; set; }
        void LoadProgram();
        int Length { get; }
    }
}