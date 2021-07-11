namespace bEmu.Systems.Gameboy.Sound
{
    public abstract class VolumeEnvelopeChannel : GbSoundChannel, IVolumeEnvelopeChannel
    {
        public int CycleToVolumeEnvelope { get; protected set; }
        public abstract float VolumeEnvelopeStepLength { get; }
        public abstract byte InitialVolumeEnvelope { get; }
        public abstract bool VolumeEnvelopeIncrease { get; }
        public abstract bool StopSoundWhenTimeEnds { get; }
        public abstract byte WaveDuty { get; }

        public VolumeEnvelopeChannel(APU apu) : base(apu) { }

        public override void StartSound()
        {
            base.StartSound();
            Volume = InitialVolumeEnvelope;
        }

        public virtual void StartVolumeEnvelope()
        {
            CycleToVolumeEnvelope = (int) (APU.Cycles + (VolumeEnvelopeStepLength * APU.CycleCount));
            StartSound();
        }

        public virtual void UpdateVolumeEnvelope()
        {
            if (VolumeEnvelopeStepLength == 0 || CycleToVolumeEnvelope == 0)
                return;

            if (APU.Cycles > CycleToVolumeEnvelope)
            {
                CycleToVolumeEnvelope = (int) (APU.Cycles + (VolumeEnvelopeStepLength * APU.CycleCount));

                if (VolumeEnvelopeIncrease && Volume < 0xF)
                    Volume++;

                if (!VolumeEnvelopeIncrease && Volume > 0)
                    Volume--;
            }
        }

        public override float GenerateWave()
        {
            if (APU.Cycles > CycleToEnd && StopSoundWhenTimeEnds)
                return 0;

            UpdateVolumeEnvelope();

            return 1;
        }
    }
}