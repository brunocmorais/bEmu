namespace bEmu.Systems.Gameboy.GPU
{
    public class MonochromePaletteData : IPaletteData
    {
        private readonly MMU mmu;

        public MonochromePaletteData(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte BGP => mmu.IO[0x47];
        public byte OBP0 => mmu.IO[0x48];
        public byte OBP1 => mmu.IO[0x49];
    }
}