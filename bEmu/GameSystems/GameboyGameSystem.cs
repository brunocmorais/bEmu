using bEmu.Classes;
using bEmu.Core;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public class GameboyGameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.GameBoy;
        private DynamicSoundEffectInstance sound;
        private SoundOscillator oscillator;
        private const int BufferSize = 2048;
        private const int SampleRate = 22050;
        private byte[] buffer;
        private double time = 0;
        private Systems.Gameboy.Sound.APU apu => this.System.APU as Systems.Gameboy.Sound.APU;
        
        public GameboyGameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new GameboyOptions(mainGame, MainGame.Options);

            if (MainGame.Options.Size < 2) 
                MainGame.Options.Size = 2;

            oscillator = new SoundOscillator();
            sound = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Stereo);
            buffer = new byte[BufferSize];
            sound.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (sound.PendingBufferCount < 4)
            {
                for (int i = 0; i < BufferSize; i += 4)
                {
                    float channel3 = (float)oscillator.GenerateCustomWave(apu.Channel3.WavePattern, time, apu.Channel3.Frequency, 2);
                    float channel2 = (float)oscillator.GenerateSquareWave(time, apu.Channel2.Frequency, 0.5);
                    float waveValue = channel2 + channel3;
                    
                    waveValue = MathHelper.Clamp(waveValue, -1.0f, 1.0f);
                    byte value = (byte)(waveValue * sbyte.MaxValue);
                    
                    buffer[i] = value;
                    buffer[i + 1] = value;
                    buffer[i + 2] = value;
                    buffer[i + 3] = value;
                    
                    time += 1.0f / SampleRate;
                }

                sound.SubmitBuffer(buffer);
            }
        }
    }
}