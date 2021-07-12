using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.System;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : Core.Audio.APU
    {
        public IGbSoundChannel[] Channels { get; }
        public IVolumeEnvelopeChannel[] VolumeEnvelopeChannels { get; }
        public ISweepEnvelopeChannel SweepEnvelopeChannel { get; }
        public int CycleCount => System.CycleCount * 60;
        public int Cycles { get; private set; }
        public int VolumeSO1 => System.MMU[0xFF24] & 0x7;
        public int VolumeSO2 => (System.MMU[0xFF24] & 0x70) >> 4;
        public bool SoundOn => (System.MMU[0xFF26] & 0x80) == 0x80;

        public APU(ISystem system) : base(system)
        {
            var channel1 = new Channel1(this);
            var channel2 = new Channel2(this);
            var channel3 = new Channel3(this);
            var channel4 = new Channel4(this);

            Channels = new IGbSoundChannel[] 
            {
                channel1,
                channel2,
                channel3,
                channel4
            };

            VolumeEnvelopeChannels = new IVolumeEnvelopeChannel[] 
            { 
                channel1,
                channel2,
                channel4
            };

            SweepEnvelopeChannel = channel1;

            Time = 0;
        }

        public override void Update(int lastCycleCount)
        {
            Cycles += lastCycleCount;
        }

        public override void UpdateBuffer()
        {
            if (!SoundOn)
                return;

            for (int i = 0; i < BufferSize; i += 4)
            {
                float leftChannel = 0, rightChannel = 0;

                for (int j = 0; j < 4; j++)
                {
                    float wave = Channels[j].GenerateWave();

                    if (OutputChannelTo(1, j + 1))
                        rightChannel += wave * (VolumeSO1 / 7f);
                    
                    if (OutputChannelTo(2, j + 1))
                        leftChannel += wave * (VolumeSO2 / 7f);
                }
                
                byte leftValue = (byte)(Math.Clamp(leftChannel, -1.0f, 1.0f) * sbyte.MaxValue);
                byte rightValue = (byte)(Math.Clamp(rightChannel, -1.0f, 1.0f) * sbyte.MaxValue);
                
                Buffer[i] = leftValue;
                Buffer[i + 1] = leftValue;
                Buffer[i + 2] = rightValue;
                Buffer[i + 3] = rightValue;
                
                Time += 1.0f / SampleRate;
            }
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

        public void StartVolumeEnvelope(GbSoundChannels channel)
        {
            switch (channel)
            {
                case GbSoundChannels.Channel1: VolumeEnvelopeChannels[0].StartVolumeEnvelope(); break;
                case GbSoundChannels.Channel2: VolumeEnvelopeChannels[1].StartVolumeEnvelope(); break;
                case GbSoundChannels.Channel4: VolumeEnvelopeChannels[2].StartVolumeEnvelope(); break;
            }
        }

        public void StartSweepEnvelope()
        {
            SweepEnvelopeChannel.StartSweepEnvelope();
        }

        public bool OutputChannelTo(int terminal, int channel)
        {
            int mask = terminal == 1 ? 0x1 : 0x10;

            switch (channel)
            {
                case 1: return (System.MMU[0xFF25] & mask) == (mask);
                case 2: return (System.MMU[0xFF25] & mask << 1) == (mask << 1);
                case 3: return (System.MMU[0xFF25] & mask << 2) == (mask << 2);
                case 4: return (System.MMU[0xFF25] & mask << 3) == (mask << 3);
                default: return false;
            }
        }

        public static WaveDuty GetWaveDuty(byte value)
        {
            switch (value)
            {
                case 0x00: return WaveDuty.Quarter;
                case 0x01: return WaveDuty.Half;
                case 0x02: return WaveDuty.Normal;
                case 0x03: return WaveDuty.HalfPlus;
                default:   return WaveDuty.Normal;
            }
        }
    }
}