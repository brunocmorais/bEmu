namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel2
    {
        private readonly MMU mmu;

        public Channel2(MMU mmu)
        {
            this.mmu = mmu;
        }

        public int Frequency => 0x20000 / (0x800 - (((mmu.IO[0x19] & 0x7) << 8) | mmu.IO[0x18]));
    }
}