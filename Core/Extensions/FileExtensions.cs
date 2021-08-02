using System.IO;
using System.Text;

namespace bEmu.Core.Extensions
{
    public static class FileExtensions
    {
        public static byte[] ToBytes(this FileStream fileStream)
        {
            int index = 0, b;
            var bytes = new byte[fileStream.Length];

            fileStream.Position = index;

            while ((b = fileStream.ReadByte()) != -1)
                bytes[index++] = (byte) b;

            fileStream.Dispose();

            return bytes;
        }

        public static string ToUTF8String(this FileStream fileStream)
        {
            return Encoding.UTF8.GetString(ToBytes(fileStream));
        }
    }
}