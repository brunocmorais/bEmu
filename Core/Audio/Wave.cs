using System.Collections.Generic;
using System.IO;
using bEmu.Core.Extensions;

namespace bEmu.Core.Audio
{
    public class Wave
    {
        const int HeaderSize = 44;
        public List<byte> Data { get; }
        public byte[] Header { get; }
        public int Length => Header.Length + Data.Count;

        public Wave()
        {
            Header = new byte[HeaderSize];
            Data = new List<byte>();
        }

        public void AddByte(byte b)
        {
            Data.Add(b);
        }

        public void AddBytes(IEnumerable<byte> bytes)
        {
            this.Data.AddRange(bytes);
        }

        public static Wave From(FileStream reader)
        {
            var wave = new Wave();
            reader.Position = 0;
            int b;

            for (int i = 0; i < HeaderSize; i++)
                wave.Header[i] = (byte)reader.ReadByte();

            while ((b = reader.ReadByte()) != -1)
                wave.AddByte((byte) b);

            reader.Dispose();
            return wave;
        }

        public void FillHeader()
        {
            using (var stream = new MemoryStream())
            {

                stream.WriteString("RIFF");
                stream.WriteUInt((uint)(36 + Length));
                stream.WriteString("WAVE");
                stream.WriteString("fmt ");
                stream.WriteUInt(16);
                stream.WriteUShort(1);
                stream.WriteUShort(2);
                stream.WriteUInt(APU.SampleRate);
                stream.WriteUInt((APU.SampleRate * APU.BitsPerSample / 8 * 2));
                stream.WriteUShort(2 * APU.BitsPerSample / 8);
                stream.WriteUShort(APU.BitsPerSample);
                stream.WriteString("data");
                stream.WriteUInt((uint) Length);

                var bytes = stream.ToArray();

                for (int i = 0; i < HeaderSize; i++)
                    Header[i] = bytes[i];
            }
        }

        public byte[] ToBytes()
        {
            FillHeader();

            using (var stream = new MemoryStream())
            {
                foreach (byte b in Header)
                    stream.WriteByte(b);

                foreach (byte b in Data)
                    stream.WriteByte(b);

                return stream.ToArray();
            }
        }
    }
}