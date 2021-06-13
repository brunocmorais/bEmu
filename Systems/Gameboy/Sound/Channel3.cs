namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel3
    {
        private readonly MMU mmu;
        private byte[] wavePattern = new byte[32];

        public Channel3(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte[] WavePattern
        {
            get
            {
                for (int i = 0x30; i < 0x34; i++) 
                    for (int j = 0; j < 8; j++)
                        wavePattern[((i - 0x30) * 8) + j] = (byte)((mmu.IO[i] >> ((28 - (4 * (j % 8)))) & 0xF) >> Shifter);

                return wavePattern;        
            }
        }

        public bool ChannelOn => (mmu.IO[0x1A] & 0x80) == 0x80;
        
        public byte SoundLength => mmu.IO[0x1B];

        public Channel3OutputLevel OutputLevel => (Channel3OutputLevel) ((mmu.IO[0x1C] & 0b1100000) >> 5);
            
        public int Frequency => 0x10000 / (0x800 - (((mmu.IO[0x1E] & 0x7) << 8) | mmu.IO[0x1D]));

        private int Shifter
        {
            get
            {
                if (!ChannelOn)
                    return 8;

                switch (OutputLevel)
                {
                    case Channel3OutputLevel.Mute: return 8;
                    case Channel3OutputLevel.Volume100: return 0;
                    case Channel3OutputLevel.Volume50: return 1;
                    case Channel3OutputLevel.Volume25: return 2;
                    default: return 0;
                }
            }
        }
    }
}