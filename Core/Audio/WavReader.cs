using System.IO;
using bEmu.Core.IO;
using bEmu.Core.Util;

namespace bEmu.Core.Audio
{
    public class WavReader : Singleton<WavReader>, IReader<Wave>
    {
        public Wave Read(FileStream stream)
        {
            var wave = new Wave();
            stream.Position = 0;
            int b;

            for (int i = 0; i < Wave.HeaderSize; i++)
                wave.Header[i] = (byte)stream.ReadByte();

            while ((b = stream.ReadByte()) != -1)
                wave.AddByte((byte) b);

            stream.Dispose();
            return wave;
        }

    }
}