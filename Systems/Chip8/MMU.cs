using bEmu.Core;
using bEmu.Core.System;

namespace bEmu.Systems.Chip8
{
    public class MMU : Core.Memory.MMU
    {
        public MMU(IRunnableSystem system) : base(system, 0x1000) { }
    }
}