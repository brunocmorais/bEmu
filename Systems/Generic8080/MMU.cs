using bEmu.Core;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.MMU
    {
        public MMU(ISystem system) : base(system, 0x10000) { }
    }
}