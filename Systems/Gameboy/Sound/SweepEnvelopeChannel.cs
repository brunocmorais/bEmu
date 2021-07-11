namespace bEmu.Systems.Gameboy.Sound
{
    public abstract class SweepEnvelopeChannel : VolumeEnvelopeChannel, ISweepEnvelopeChannel
    {
        protected SweepEnvelopeChannel(APU apu) : base(apu) { }

        public int CycleToSweepEnvelope { get; protected set; }
        public abstract byte NumberSweepShift { get; set; }
        public abstract bool SweepDecrease { get; }
        public abstract byte SweepTime { get; }
        public abstract int SweepFrequency { get; set; }

        public override void StartSound()
        {
            base.StartSound();
            SweepFrequency = 0;
        }

        public virtual void StartSweepEnvelope()
        {
            CycleToSweepEnvelope = (int) (APU.Cycles + ((SweepTime / 128f) * APU.CycleCount));
            //StartSound();
        }

        public virtual void UpdateSweepEnvelope()
        {
            if (SweepTime == 0 || CycleToSweepEnvelope == 0)
                return;

            if (APU.Cycles >= CycleToSweepEnvelope)
            {
                CycleToSweepEnvelope = (int) (APU.Cycles + ((SweepTime / 128f) * APU.CycleCount));
                int frequency = SweepFrequency != 0 ? SweepFrequency : Frequency;
                int newFrequency = frequency >> NumberSweepShift;

                if (SweepDecrease)
                    SweepFrequency = frequency - newFrequency;
                else
                    SweepFrequency = frequency + newFrequency;
            }
        }

        public override float GenerateWave()
        {
            return base.GenerateWave();
            //UpdateSweepEnvelope();
        }
    }
}