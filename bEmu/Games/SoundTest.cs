using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bEmu
{
    public class SoundTest : Game
    {
        private GraphicsDeviceManager graphics;
        private Oscillator leftOscillator;
        private Oscillator rightOscillator;
        private Thread thread;
        private DynamicSoundEffectInstance instance;
        private int sampleRate = 22050;
        private byte[] xnaBuffer;
        private float[,] workingBuffer;
        const int channels = 2;
        const int samplesPerBuffer = 16;
        const int bytesPerSample = 2;
        private double time = 0;
        private bool running = true;

        public SoundTest()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 300;
        }

        protected override void LoadContent()
        {
            instance = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Stereo);
            instance.Play();

            xnaBuffer = new byte[channels * samplesPerBuffer * bytesPerSample];
            workingBuffer = new float[channels, samplesPerBuffer];

            leftOscillator = new Oscillator();
            rightOscillator = new Oscillator();

            thread = new Thread(() => 
            {
                while (running) 
                {
                    lock (instance)
                    {
                        if (instance.PendingBufferCount <= 128)
                            SubmitBuffer();
                    }
                }
            });

            thread.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                running = false;
                Exit();
            }

            HandleInputs();
        }

        private void HandleInputs()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                leftOscillator.Frequency += 2;
                rightOscillator.Frequency += 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                leftOscillator.Frequency -= 2;
                rightOscillator.Frequency -= 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                leftOscillator.Amplitude += 0.1;
                rightOscillator.Amplitude += 0.1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                //if (leftOscillator.Amplitude > 0 && rightOscillator.Amplitude > 0)
                {
                    leftOscillator.Amplitude = 0;
                    rightOscillator.Amplitude = 0;
                }
            }
        }

        private void SubmitBuffer()
        {
            FillWorkingBuffer();
            ConvertBuffer(workingBuffer, xnaBuffer);
            instance.SubmitBuffer(xnaBuffer);
        }

        private void ConvertBuffer(float[,] from, byte[] to)
        {
            int bufferSize = from.GetLength(1);

            for (int i = 0; i < bufferSize; i++)
            {
                for (int c = 0; c < channels; c++)
                {
                    float floatSample = MathHelper.Clamp(from[c, i], -1.0f, 1.0f);	
                    short shortSample = (short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);
                    int index = i * channels * bytesPerSample + c * bytesPerSample;
                    to[index] = (byte)shortSample;

                    if (!BitConverter.IsLittleEndian)
                    {
                        to[index] = (byte)(shortSample >> 8);
                        to[index + 1] = (byte)shortSample;
                    }
                    else
                    {
                        to[index] = (byte)shortSample;
                        to[index + 1] = (byte)(shortSample >> 8);
                    }
                }
            }
        }

        private void FillWorkingBuffer()
        {
            for (int i = 0; i < samplesPerBuffer; i++)
            {
                workingBuffer[0, i] = (float) leftOscillator.GenerateSquareWave(time);
                workingBuffer[1, i] = (float) rightOscillator.GenerateSquareWave(time);
                time += 1.0 / sampleRate;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }
    }

    public class Oscillator
    {
        public double Frequency = 440;
        public double Amplitude = 0.5;
        private Random random = new Random();

        public double GenerateSquareWave(double time)
        {
            return Math.Sin(Frequency * time * 2 * Math.PI) >= 0 ? Amplitude : -Amplitude;
        }
    }
}