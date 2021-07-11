using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public interface IGbSoundChannel
    {
        void StartSound();
        float GenerateWave();
        float SoundLength { get; }
        byte Volume { get; }
        APU APU { get; }
        SoundOscillator Oscillator { get; }
        MMU MMU { get; }
        int Frequency { get; }
        int CycleToEnd { get; }
    }
}