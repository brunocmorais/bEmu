using bEmu.Core.System;

namespace bEmu.Systems.NES
{
    public class MMU : Core.Memory.MMU
    {
        public MMU(ISystem system) : base(system, 65536)
        {
        }
    }
}