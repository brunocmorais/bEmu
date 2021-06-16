using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel4 : IGBSoundChannel
    {
        public APU APU { get; }
        public MMU MMU { get; }
        public SoundOscillator Oscillator { get; }
        private int cycleToEnd = 0;

        public Channel4(APU apu)
        {
            APU = apu;
            MMU = apu.System.MMU as MMU;
            Oscillator = new SoundOscillator();
        }

        public float SoundLength => (64 - (MMU.IO[0x20] & 0x3F)) * (1.0f / 256.0f);
        public int Frequency => 0x20000 / (0x800 - (((MMU.IO[0x19] & 0x7) << 8) | MMU.IO[0x18]));
        public bool StopSoundWhenTimeEnds => (MMU.IO[0x19] & 0x40) == 0x40;
        public byte Volume => (byte)((MMU.IO[0x21] & 0xF0) >> 4);

        public void StartSound()
        {
            cycleToEnd = (int) (APU.Cycles + (0.05 * APU.CycleCount)); // TODO: somar com SoundLength
        }

        public float GenerateWave(int currentCycle)
        {
            if (currentCycle > cycleToEnd)
                return 0;

            return (float) Oscillator.GenerateWhiteNoise(0.25f * (Volume / 16.0f));
        }
    }
}