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

        public double GenerateSineWave(double time, double frequency, double amplitude)
        {
            return Math.Sin(frequency * time * 2 * Math.PI) * amplitude;
        }

        public double GenerateWhiteNoise(double amplitude)
        {
            return (random.NextDouble() - random.NextDouble()) * amplitude;
        }

        public double GenerateCustomWave(byte[] bytes, double time, double frequency, double amplitude)
        {
            var angle = frequency * time;
            return ((float) bytes[(int)((angle - (int) angle) * bytes.Length)] / (0xF)) * amplitude * 2;
        }
    }
}