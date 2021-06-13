using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : IAPU
    {
        private MMU mmu;

        public APU(ISystem system)
        {
            this.mmu = system.MMU as MMU;
            Channel2 = new Channel2(mmu);
            Channel3 = new Channel3(mmu);
        }

        public Channel2 Channel2 { get; }
        public Channel3 Channel3 { get; }

    }
}