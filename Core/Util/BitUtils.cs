namespace bEmu.Core.Util
{
    public static class BitUtils
    {
        public static ushort GetWordFrom2Bytes(byte lsb, byte msb)
        {
            return (ushort)((msb << 8) | lsb);
        }

        public static void Get2BytesFromWord(ushort value, out byte msb, out byte lsb)
        {
            msb = (byte) ((value & 0xFF00) >> 8);
            lsb = (byte) ((value & 0xFF));
        }
    }
}