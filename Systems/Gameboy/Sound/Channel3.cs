using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel3
    {
        private readonly APU apu;
        private byte[] wavePattern = new byte[32];
        private SoundOscillator oscillator;
        private float amplitude = 0;
        private double timeToEnd = 0;

        public Channel3(APU apu)
        {
            this.apu = apu;
            this.oscillator = new SoundOscillator();
        }

        public byte[] WavePattern
        {
            get
            {
                for (int i = 0x30; i < 0x40; i++) 
                {
                    wavePattern[((i - 0x30) * 2)] = (byte)(((apu.MMU.IO[i] >> 4) & 0xF) >> Shifter);
                    wavePattern[((i - 0x30) * 2) + 1] = (byte)(((apu.MMU.IO[i]) & 0xF) >> Shifter);
                }

                return wavePattern;        
            }
        }

        public bool ChannelOn => (apu.MMU.IO[0x1A] & 0x80) == 0x80;
        
        public byte SoundLength => apu.MMU.IO[0x1B];

        public Channel3OutputLevel OutputLevel => (Channel3OutputLevel) ((apu.MMU.IO[0x1C] & 0b1100000) >> 5);
            
        public int Frequency => 0x10000 / (0x800 - (((apu.MMU.IO[0x1E] & 0x7) << 8) | apu.MMU.IO[0x1D]));

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

        public void StartSound()
        {
            amplitude = 0.25f;
            timeToEnd = apu.Time + SoundLength;
        }

        public float GenerateWave(double time)
        {
            // if (time > timeToEnd)
            //     amplitude = 0f;

            return (float) oscillator.GenerateCustomWave(WavePattern, time, Frequency, 0.25);
        }
    }
}