using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class APU : Core.APU
    {
        public IGBSoundChannel[] Channels { get; }
        public int CycleCount => System.CycleCount * 60;
        public int Cycles { get; private set; }
        public int VolumeSO1 => System.MMU[0xFF24] & 0x7;
        public int VolumeSO2 => (System.MMU[0xFF24] & 0x70) >> 4;
        public bool SoundOn => (System.MMU[0xFF26] & 0x80) == 0x80;

        public APU(ISystem system) : base(system)
        {
            Channels = new IGBSoundChannel[] 
            {
                new Channel1(this),
                new Channel2(this),
                new Channel3(this),
                new Channel4(this)
            };
            Time = 0;
        }

        public override void Update(int lastCycleCount)
        {
            Cycles += lastCycleCount;
        }

        public override void UpdateBuffer()
        {
            for (int i = 0; i < BufferSize; i += 4)
            {
                float leftChannel = 0, rightChannel = 0;

                if (SoundOn)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        float wave = Channels[j].GenerateWave(Cycles);

                        if (OutputChannelTo(1, j + 1))
                            rightChannel += wave * (VolumeSO1 / 7f);
                        
                        if (OutputChannelTo(2, j + 1))
                            leftChannel += wave * (VolumeSO2 / 7f);
                    }
                }
                
                byte leftValue = (byte)(leftChannel * sbyte.MaxValue);
                byte rightValue = (byte)(rightChannel * sbyte.MaxValue);
                
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
    }
}