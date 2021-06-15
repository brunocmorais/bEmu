using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : Core.APU
    {
        private byte[] buffer;
        public override int BufferSize => 3172;
        public override int SampleRate => 22050;
        public double Time { get; private set; }
        public IGBSoundChannel[] Channels { get; }

        public APU(ISystem system) : base(system)
        {
            Channels = new IGBSoundChannel[] 
            {
                new Channel1(this),
                new Channel2(this),
                new Channel3(this),
                new Channel4(this)
            };
            buffer = new byte[BufferSize];
            Time = 0;
        }

        public override byte[] UpdateBuffer()
        {
            for (int i = 0; i < BufferSize; i += 4)
            {
                float channelSum = 0;

                foreach (var channel in Channels)
                    channelSum += channel.GenerateWave(Time);
                
                byte value = (byte)(channelSum * sbyte.MaxValue);
                
                buffer[i] = value;
                buffer[i + 1] = value;
                buffer[i + 2] = value;
                buffer[i + 3] = value;
                
                Time += 1.0f / SampleRate;
            }

            return buffer;
        }

        public void StartSound(GbSoundChannels channel)
        {
            switch (channel)
            {
                case GbSoundChannels.Channel1: Channels[0].StartSound(); break;
                case GbSoundChannels.Channel2: Channels[1].StartSound(); break;
                case GbSoundChannels.Channel3: Channels[2].StartSound(); break;
                case GbSoundChannels.Channel4: Channels[3].StartSound(); break;
            }
        }
    }
}