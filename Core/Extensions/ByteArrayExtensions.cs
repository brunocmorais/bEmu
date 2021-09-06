using System.Text;

namespace bEmu.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] GetArray(this byte[] array, int start, int end)
        {
            byte[] @return = new byte[end - start + 1];

            for (int i = 0; i < @return.Length; i++)
                @return[i] = array[i + start];

            return @return;
        }

        public static string GetString(this byte[] array, int start, int end)
        {
            var sb = new StringBuilder();
            
            for (int i = start; i < end; i++)
            {
                if (array[i] == 0)
                    break;

                sb.Append((char) array[i]);
            }

            return sb.ToString();
        }
    }
}