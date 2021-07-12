using bEmu.Core;
using bEmu.Core.Audio;

namespace bEmu.Systems.Chip8
{
    public class APU : Core.Audio.APU
    {
        private const int Frequency = 440;
        private const float Amplitude = 1.0f;
        private bool playingTone = false;

        public APU(ISystem system) : base(system) 
        { 
        }

        public override void UpdateBuffer()
        {
            for (int i = 0; i < BufferSize; i += 4)
            {
                float wave = 0;

                if (playingTone)
                    wave = (float) SoundOscillator.GenerateSineWave(Time, Frequency, Amplitude);
                
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