using System;
using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel1 : SweepEnvelopeChannel, ISweepEnvelopeChannel
    {
        public Channel1(APU apu) : base(apu) { }
        public override int SweepFrequency { get; set; }
        public override float SoundLength => (64 - (MMU.IO[0x11] & 0x3F)) * (1.0f / 256.0f);
        public override byte WaveDuty => (byte)((MMU.IO[0x11]) >> 6);
        public override int Frequency => 0x20000 / (0x800 - (((MMU.IO[0x14] & 0x7) << 8) | MMU.IO[0x13]));
        public override bool StopSoundWhenTimeEnds => (MMU.IO[0x14] & 0x40) == 0x40;
        public override byte NumberSweepShift
        {
            get => (byte)((MMU.IO[0x10] & 0x7));
            set => MMU.IO[0x10] = (byte)((MMU.IO[0x10] & 0xF8) | value);
        }
        public override bool SweepDecrease => (MMU.IO[0x10] & 0x8) == 0x8;
        public override byte SweepTime => (byte)((MMU.IO[0x10]) >> 4);
        public override float VolumeEnvelopeStepLength => (MMU.IO[0x12] & 0x7) * (1.0f / 64);
        public override byte InitialVolumeEnvelope => (byte)((MMU.IO[0x12] & 0xF0) >> 4);
        public override bool VolumeEnvelopeIncrease => (MMU.IO[0x12] & 0x8) == 0x8;
        public override byte Volume { get; protected set; }

        public override float GenerateWave()
        {
            if (base.GenerateWave() == 0)
                return 0;

            int frequency = SweepFrequency != 0 ? SweepFrequency : Frequency;

            return (float) Oscillator.GenerateSquareWave(APU.Time, frequency, 0.25f * (Volume / 16.0f), APU.GetWaveDuty(WaveDuty));
        }
    }
}