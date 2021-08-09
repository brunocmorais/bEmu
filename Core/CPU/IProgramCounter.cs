using System;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{    
    public interface IProgramCounter<T> where T : struct
    {
        Number<T> PC { get; set; }
    }
}