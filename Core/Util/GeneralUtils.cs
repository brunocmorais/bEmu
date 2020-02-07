namespace bEmu.Core.Util
{
    public static class GeneralUtils
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
    }
}