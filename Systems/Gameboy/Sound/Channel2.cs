using bEmu.Core;
using bEmu.Core.Audio;

namespace bEmu.Systems.Gameboy.Sound
{
    public class Channel2 : VolumeEnvelopeChannel
    {
        public Channel2(APU apu) : base(apu) { }

        public override float SoundLength => (64 - (MMU.IO[0x16] & 0x3F)) * (1.0f / 256.0f);
        public override byte WaveDuty => (byte)((MMU.IO[0x16]) >> 6);
        public override int Frequency => 0x20000 / (0x800 - (((MMU.IO[0x19] & 0x7) << 8) | MMU.IO[0x18]));
        public override bool StopSoundWhenTimeEnds => (MMU.IO[0x19] & 0x40) == 0x40;
        public override float VolumeEnvelopeStepLength => (MMU.IO[0x17] & 0x7) * (1.0f / 64);
        public override byte InitialVolumeEnvelope => (byte)((MMU.IO[0x17] & 0xF0) >> 4);
        public override bool VolumeEnvelopeIncrease => (MMU.IO[0x17] & 0x8) == 0x8;
        public override byte Volume { get; protected set; }

        public override float GenerateWave()
        {
            if (base.GenerateWave() == 0)
                return 0;
                
            return (float) SoundOscillator.GenerateSquareWave(APU.Time, Frequency, 0.25f * (Volume / 16.0f), APU.GetWaveDuty(WaveDuty));
        }
    }
}