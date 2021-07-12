using bEmu.Core;
using bEmu.Core.Audio;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel3 : GbSoundChannel
    {
        private byte[] wavePattern = new byte[32];

        public Channel3(APU apu) : base(apu) { }

        public byte[] WavePattern
        {
            get
            {
                for (int i = 0x30; i < 0x40; i++) 
                {
                    wavePattern[((i - 0x30) * 2)] = (byte)(((MMU.IO[i] >> 4) & 0xF) >> Shifter);
                    wavePattern[((i - 0x30) * 2) + 1] = (byte)(((MMU.IO[i]) & 0xF) >> Shifter);
                }

                return wavePattern;        
            }
        }

        public bool ChannelOn => (MMU.IO[0x1A] & 0x80) == 0x80;
        public override float SoundLength => (256 - MMU.IO[0x1B]) * (1.0f / 256.0f);
        public Channel3OutputLevel OutputLevel => (Channel3OutputLevel) ((MMU.IO[0x1C] & 0x60) >> 5);
        public override int Frequency => 0x10000 / (0x800 - (((MMU.IO[0x1E] & 0x7) << 8) | MMU.IO[0x1D]));

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

        public override byte Volume { get; protected set; }

        public override float GenerateWave()
        {
            return (float) SoundOscillator.GenerateCustomWave(WavePattern, APU.Time, Frequency, 0.25f);
        }
    }
}