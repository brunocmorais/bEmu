using System;

namespace bEmu.Core
{
    public class SoundOscillator
    {
        private Random random = new Random();

        public double GenerateSquareWave(double time, double frequency, double amplitude)
        {
            return Math.Sin(frequency * time * 2 * Math.PI) >= 0 ? amplitude : -amplitude;
        }

        public double GenerateWhiteNoise(double time, double frequency, double amplitude)
        {
            return (random.NextDouble() - random.NextDouble()) * amplitude;
        }
    }
}