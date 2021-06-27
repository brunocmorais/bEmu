using bEmu.Core;

namespace bEmu.Systems.Chip8
{
    public class APU : Core.APU
    {
        private const int Frequency = 440;
        private const float Amplitude = 1.0f;
        private SoundOscillator oscillator;
        private bool playingTone = false;

        public APU(ISystem system) : base(system) 
        { 
            oscillator = new SoundOscillator();
        }

        public override void UpdateBuffer()
        {
            for (int i = 0; i < BufferSize; i += 4)
            {
                float wave = 0;

                if (playingTone)
                    wave = (float) oscillator.GenerateSineWave(Time, Frequency, Amplitude);
                
                byte value = (byte)(wave * sbyte.MaxValue);
                
                Buffer[i] = value;
                Buffer[i + 1] = value;
                Buffer[i + 2] = value;
                Buffer[i + 3] = value;
                
                Time += 1.0f / SampleRate;
            }
        }

        public override void Update(int cycles)
        {
            playingTone = ((Systems.Chip8.State)System.State).Sound > 0;
        }
    }
}