using bEmu.Core.System;

namespace bEmu.Core.Audio
{
    public interface IAPU
    {
        IAudioSystem System { get; }
        byte[] Buffer { get; }
        double Time { get; }

        void UpdateBuffer();
        void Update(int cycles);
    }
}