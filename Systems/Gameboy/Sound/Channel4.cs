using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel4 : VolumeEnvelopeChannel
    {
        public Channel4(APU apu) : base(apu) { }

        public override float SoundLength => (64 - (MMU.IO[0x20] & 0x3F)) * (1.0f / 256.0f);
        public override int Frequency => (int)(0x80000 / ((MMU.IO[0x22] & 0x7) == 0 ? 0.5f : MMU.IO[0x22] & 0x7) / Math.Pow(2, (MMU.IO[0x22] >> 4) + 1));
        public override bool StopSoundWhenTimeEnds => (MMU.IO[0x23] & 0x40) == 0x40;
        public override byte Volume { get; protected set; }
        public override float VolumeEnvelopeStepLength => (MMU.IO[0x21] & 0x7) * (1.0f / 64);
        public override byte InitialVolumeEnvelope => (byte)((MMU.IO[0x21] & 0xF0) >> 4);
        public override bool VolumeEnvelopeIncrease => (MMU.IO[0x21] & 0x8) == 0x8;
        public override byte WaveDuty => 0x02;

        public override float GenerateWave()
        {
            base.GenerateWave();
            return (float) Oscillator.GenerateWhiteNoise(0.25f * (Volume / 16.0f));
        }
    }
}