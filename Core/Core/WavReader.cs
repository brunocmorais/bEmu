using System.IO;

namespace bEmu.Core
{
    public static class WavReader
    {
        private const int WavDataStart = 42;

        public static byte[] GetBytes(string path)
        {
            using (var reader = File.OpenRead(path))
            {
                byte[] bytes = new byte[reader.Length - WavDataStart];
                int index = 0, b = 0;

                if (reader.Length > WavDataStart)
                    reader.Seek(WavDataStart, SeekOrigin.Begin);
                
                while ((b = reader.ReadByte()) != -1)
                    bytes[index++] = (byte) b;

                return bytes;
            }
        }
    }
}