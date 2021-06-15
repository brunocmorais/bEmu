using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel2 : IGBSoundChannel
    {
        public APU APU { get; }
        public SoundOscillator Oscillator { get; }
        private float amplitude = 0f;
        private double timeToEnd = 0;
        public MMU MMU { get; }

        //private double timeToEnvelope = 0;

        public Channel2(APU apu)
        {
            APU = apu;
            MMU = apu.System.MMU as MMU;
            Oscillator = new SoundOscillator();
        }

        public float SoundLength => (64 - (MMU.IO[0x16] & 0x3F)) * (1.0f / 256.0f);
        public int Frequency => 0x20000 / (0x800 - (((MMU.IO[0x19] & 0x7) << 8) | MMU.IO[0x18]));
        public bool StopSoundWhenTimeEnds => (MMU.IO[0x19] & 0x40) == 0x40;
        public float NumberEnvelopeSweep => (MMU.IO[0x17] & 0x7) * (1.0f / 64);
        public byte Volume => (byte)((MMU.IO[0x17] & 0xF0) >> 4);

        public void StartSound()
        {
            amplitude = 0.25f;
            timeToEnd = APU.Time + SoundLength;
        }

        public float GenerateWave(double time)
        {
            if (time > timeToEnd)
                amplitude = 0f;

            return (float) Oscillator.GenerateSquareWave(time, Frequency, amplitude * (Volume / 16.0f));
        }
    }
}