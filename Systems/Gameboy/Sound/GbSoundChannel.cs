using bEmu.Core;

namespace bEmu.Systems.Gameboy.Sound
{
    public abstract class GbSoundChannel : IGbSoundChannel
    {
        public int CycleToEnd { get; protected set; }
        public APU APU { get; }
        public MMU MMU { get; }
        public abstract float SoundLength { get; }
        public abstract byte Volume { get; protected set; }
        public abstract int Frequency { get; }

        public GbSoundChannel(APU apu)
        {
            APU = apu;
            MMU = apu.System.MMU as MMU;
        }

        public virtual void StartSound()
        {
            CycleToEnd = (int) (APU.Cycles + (SoundLength * APU.CycleCount));
        }

        public abstract float GenerateWave();
    }
}