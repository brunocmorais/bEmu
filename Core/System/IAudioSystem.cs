using bEmu.Core.Audio;

namespace bEmu.Core.System
{
    public interface IAudioSystem : IRunnableSystem
    {
        IAPU APU { get; }
        byte[] SoundBuffer { get; }
    }
}