namespace bEmu.Systems.Gameboy.Sound
{
    public interface IVolumeEnvelopeChannel
    {
        float VolumeEnvelopeStepLength { get; }
        byte InitialVolumeEnvelope { get; }
        bool VolumeEnvelopeIncrease { get; }
        bool StopSoundWhenTimeEnds { get; }
        byte WaveDuty { get; }
        int CycleToVolumeEnvelope { get; }

        void StartVolumeEnvelope();
        void UpdateVolumeEnvelope();
    }
}