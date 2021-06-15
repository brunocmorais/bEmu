using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel1 : IGBSoundChannel
    {
        public APU APU { get; }
        public SoundOscillator Oscillator { get; }
        public MMU MMU { get; }
        private float amplitude = 0f;
        private double timeToEnd = 0;
        //private double timeToEnvelope = 0;

        public Channel1(APU apu)
        {
            APU = apu;
            MMU = apu.System.MMU as MMU;
            Oscillator = new SoundOscillator();
        }

        public float SoundLength => (64 - (MMU.IO[0x11] & 0x3F)) * (1.0f / 256.0f);
        public int Frequency => 0x20000 / (0x800 - (((MMU.IO[0x14] & 0x7) << 8) | MMU.IO[0x13]));
        public bool StopSoundWhenTimeEnds => (MMU.IO[0x14] & 0x40) == 0x40;
        public float NumberEnvelopeSweep => (MMU.IO[0x12] & 0x7) * (1.0f / 64);
        public byte Volume => (byte)((MMU.IO[0x12] & 0xF0) >> 4);

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