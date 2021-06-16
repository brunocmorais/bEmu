using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public interface IGBSoundChannel
    {
        void StartSound();
        float GenerateWave(int currentCycle);
        float SoundLength { get; }
        byte Volume { get; }
        APU APU { get; }
        SoundOscillator Oscillator { get; }
        MMU MMU { get; }
    }
}