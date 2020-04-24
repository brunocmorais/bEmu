using System;

namespace bEmu.Core
{
    public class SoundOscillator
    {
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        private Random random = new Random();

        public double GenerateSquareWave(double time)
        {
            return Math.Sin(Frequency * time * 2 * Math.PI) >= 0 ? Amplitude : -Amplitude;
        }

        public double GenerateWhiteNoise(double time)
        {
            return (random.NextDouble() - random.NextDouble()) * Amplitude;
        }
    }
}