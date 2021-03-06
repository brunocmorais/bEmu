using bEmu.Core.Audio;
using bEmu.Core.Memory;

namespace bEmu.Core.System
{

    public abstract class AudioSystem : RunnableSystem, IAudioSystem
    {
        protected readonly byte[] dummySoundBuffer = new byte[Audio.APU.BufferSize];
        public IAPU APU { get; protected set; }
        public byte[] SoundBuffer
        {
            get
            {
                if (APU != null)
                {
                    APU.UpdateBuffer();
                    return APU.Buffer;
                }

                return dummySoundBuffer;
            }
        }

        protected AudioSystem(IROM rom) : base(rom)
        {
        }
    }
}