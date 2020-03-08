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

        public static byte GetMostSignificantNibble(byte value)
        {
            return (byte) ((value & 0xF0) >> 4);
        }

        public static byte GetLeastSignificantNibble(byte value)
        {
            return (byte) ((value & 0x0F));
        }

        public static void SetMostSignificantNibble(byte nibble, ref byte value)
        {
            value &= (byte) (nibble << 4);
        }

        public static void SetLeastSignificantNibble(byte nibble, ref byte value)
        {
            value &= nibble;
        }
    }
}