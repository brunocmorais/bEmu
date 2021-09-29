using System.IO;
using System.Linq;
using bEmu.Core.CPU;

namespace bEmu.Core.Extensions
{
    public static class StreamExtensions
    {
        public static void WriteUInt(this Stream stream, uint value)
        {
            var bytes = BitFacade.ToBytes(value).ToArray();

            for (int i = bytes.Length - 1; i >= 0; i--)
                stream.WriteByte(bytes[i]);
        }

        public static void WriteUShort(this Stream stream, ushort value)
        {
            BitFacade.Get2BytesFromWord(value, out byte msb, out byte lsb);
            stream.WriteByte(lsb);
            stream.WriteByte(msb);
        }

        public static void WriteString(this Stream stream, string value)
        {
            foreach (char c in value)
                stream.WriteByte((byte) c);
        }
    }
}