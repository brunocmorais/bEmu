using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel3 : IGBSoundChannel
    {
        public APU APU { get; }
        public MMU MMU { get; }
        public SoundOscillator Oscillator { get; }
        private byte[] wavePattern = new byte[32];
        private double timeToEnd = 0;

        public Channel3(APU apu)
        {
            APU = apu;
            MMU = apu.System.MMU as MMU;
            Oscillator = new SoundOscillator();
        }

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
        
        public float SoundLength => (256 - MMU.IO[0x1B]) * (1.0f / 256.0f);

        public Channel3OutputLevel OutputLevel => (Channel3OutputLevel) ((MMU.IO[0x1C] & 0x60) >> 5);
            
        public int Frequency => 0x10000 / (0x800 - (((MMU.IO[0x1E] & 0x7) << 8) | MMU.IO[0x1D]));

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

        public byte Volume => 0xFF;

        public void StartSound()
        {
            timeToEnd = APU.Time + SoundLength;
        }

        public float GenerateWave(double time)
        {
            // if (time > timeToEnd)
            //     amplitude = 0f;

            return (float) Oscillator.GenerateCustomWave(WavePattern, time, Frequency, 0.25f);
        }
    }
}