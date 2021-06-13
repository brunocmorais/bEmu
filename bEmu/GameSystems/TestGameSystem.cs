using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using bEmu.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public class TestGameSystem : GameSystem
    {
        private DynamicSoundEffectInstance sound;
        private SoundOscillator oscillator;
        private double time = 0;
        private const int BufferSize = 2048;
        private const int SampleRate = 22050;
        private int Frequency = 440;
        private double Amplitude = 0;
        private byte[] buffer;
        private byte[] file;
        //private int counter = 42;
        private double millis = 0;
        private int noteTime = 0;
        private byte[] wave = {
            //0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF,
            0x0, 0x0, 0x1, 0x1, 0x2, 0x2, 0x3, 0x3, 0x4, 0x4, 0x5, 0x5, 0x6, 0x6, 0x7, 0x7, 0x8, 0x8, 
            0x9, 0x9, 0xA, 0xA, 0xB, 0xB, 0xC, 0xC, 0xD, 0xD, 0xE, 0xE, 0xF, 0xF,
        };
        public TestGameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            oscillator = new SoundOscillator();
            sound = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Stereo);
            buffer = new byte[BufferSize];
            sound.Play();
            file = File.ReadAllBytes("/home/bruno/teste3.wav");
        }

        public override void Update(GameTime gameTime)
        {
            if (Amplitude > 0)
            {
                while (sound.PendingBufferCount < 4)
                {
                    for (int i = 0; i < BufferSize; i += 4)
                    {
                        float waveValue = (float) oscillator.GenerateCustomWave(wave, time, Frequency, Amplitude);
                        //waveValue = MathHelper.Clamp(waveValue, -1.0f, 1.0f);
                        byte value = (byte)(waveValue * sbyte.MaxValue);
                        buffer[i] = value;
                        buffer[i + 1] = value;
                        buffer[i + 2] = value;
                        buffer[i + 3] = value;
                        time += 1.0f / SampleRate;
                    }

                    // for (int i = 0; i < BufferSize; i += 2)
                    // {
                    //     if (counter == file.Length)
                    //         counter = 42;

                    //     buffer[i] = file[counter++];
                    //     buffer[i + 1] = file[counter++];
                    // }

                    sound.SubmitBuffer(buffer);
                }

                if (millis == 0)
                    millis = gameTime.TotalGameTime.TotalMilliseconds;

                if (gameTime.TotalGameTime.TotalMilliseconds - millis >= noteTime / 10.0)
                {
                    Amplitude -= 0.1;
                    millis = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
            else
            {
                millis = 0;
                Amplitude = 0;
            }
        }

        public override void UpdateGamePad(KeyboardState keyboardState)
        {
            base.UpdateGamePad(keyboardState);

            if (keyboardState.GetPressedKeys().Contains(Keys.Right))
                PlayNote(440, 1000);

            if (keyboardState.GetPressedKeys().Contains(Keys.Left))
                PlayNote(880, 500);
        }

        private void PlayNote(int frequency, int time)
        {
            Frequency = frequency;
            Amplitude = 1;
            noteTime = time;
        }
    }
}