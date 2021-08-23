using bEmu.Core.System;

namespace bEmu.Systems.NES
{
    public class MMU : Core.Memory.MMU
    {
        public MMU(IRunnableSystem system) : base(system, 65536)
        {
        }
    }
}