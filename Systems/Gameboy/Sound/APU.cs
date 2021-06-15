using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : IAPU
    {
        private byte[] buffer;
        public const int BufferSize = 3172;
        public const int SampleRate = 22050;
        public double Time { get; private set; }
        public MMU MMU { get; }

        public APU(ISystem system)
        {
            MMU = system.MMU as MMU;
            Channel1 = new Channel1(this);
            Channel2 = new Channel2(this);
            Channel3 = new Channel3(this);
            Channel4 = new Channel4(this);
            buffer = new byte[BufferSize];
            Time = 0;
        }

        public Channel1 Channel1 { get; }
        public Channel2 Channel2 { get; }
        public Channel3 Channel3 { get; }
        public Channel4 Channel4 { get; }

        public byte[] UpdateBuffer()
        {
            for (int i = 0; i < BufferSize; i += 4)
            {
                float channel1 = Channel1.GenerateWave(Time);
                float channel2 = Channel2.GenerateWave(Time);
                float channel3 = Channel3.GenerateWave(Time);
                float channel4 = Channel4.GenerateWave(Time);
                
                byte value = (byte)((channel1 + channel2 + channel3 + channel4) * sbyte.MaxValue);
                
                buffer[i] = value;
                buffer[i + 1] = value;
                buffer[i + 2] = value;
                buffer[i + 3] = value;
                
                Time += 1.0f / SampleRate;
            }

            return buffer;
        }
    }
}