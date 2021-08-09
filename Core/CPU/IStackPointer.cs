using System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{    public interface IStackPointer<T> where T : struct
    {
        Number<T> SP { get; set; }
    }
}