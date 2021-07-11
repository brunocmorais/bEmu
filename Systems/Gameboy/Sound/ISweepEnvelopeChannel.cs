namespace bEmu.Systems.Gameboy.Sound
{
    public interface ISweepEnvelopeChannel
    {
        byte NumberSweepShift { get; set; }
        bool SweepDecrease { get; }
        byte SweepTime { get; }
        int SweepFrequency { get; set; }
        int CycleToSweepEnvelope { get; }

        void StartSweepEnvelope();
        void UpdateSweepEnvelope();
    }
}