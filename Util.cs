using System.Linq;

namespace Intel8080
{
    public static class Util
    {
        public static ushort Get16BitNumber(byte a, byte b)
        {
            return (ushort)((b << 8) | a);
        }

        public static void WordTo2Bytes(ushort value, out byte a, out byte b)
        {
            a = (byte) ((value & 0xFF00) >> 8);
            b = (byte) ((value & 0xFF));
        }

        public static bool CheckZero(byte value)
        {
            return value == 0;
        }

        public static bool CheckSign(byte value)
        {
            return (value & 0x80) == 0x80;
        }

        public static bool CheckParity(byte value)
        {
            byte numberOfOneBits = 0;

            for (int i = 0; i < 8; i++) 
                numberOfOneBits += (byte)((value >> i) & 1);

            return (numberOfOneBits & 1) == 0;
        }

        public static bool CheckZero(ushort value)
        {
            return value == 0;
        }

        public static bool CheckSign(ushort value)
        {
            return (value & 0x8000) == 0x8000;
        }

        public static bool CheckParity(ushort value)
        {
            ushort numberOfOneBits = 0;

            for (int i = 0; i < 16; i++) 
                numberOfOneBits += (ushort)((value >> i) & 1);

            return (numberOfOneBits & 1) == 0;
        }

        public static byte[] CombineArrays(params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;

            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }
    }
}