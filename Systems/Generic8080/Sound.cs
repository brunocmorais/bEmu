using System.IO;
using bEmu.Core;
using bEmu.Core.Audio;

namespace bEmu.Systems.Generic8080
{
    public class Sound
    {
        public bool IsPlaying => counter != -1;
        private readonly bool isLooped;
        private int counter = -1;
        private readonly byte[] bytes;
        public sbyte Next
        {
            get 
            {
                if (counter == -1)
                    return 0;

                if (counter >= bytes.Length - 2)
                {
                    if (isLooped)
                        counter = 0;
                    else
                    {
                        counter = -1;
                        return 0;
                    }
                }

                if (counter == -1)
                    return 0;

                return (sbyte)(bytes[counter++]);
            }
        }

        public Sound(FileStream fileStream, bool isLooped = false)
        {
            bytes = WavReader.Instance.Read(fileStream).Data.ToArray();
            this.isLooped = isLooped;
        }

        public void Play()
        {
            counter = 0;
        }

        public void Stop()
        {
            counter = -1;
        }
    }
}