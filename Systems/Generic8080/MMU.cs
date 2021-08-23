using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Core.IO;
using bEmu.Core.System;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.Memory.MMU
    {
        public MMU(IRunnableSystem system) : base(system, 0x10000) 
        {
        }
    }
}