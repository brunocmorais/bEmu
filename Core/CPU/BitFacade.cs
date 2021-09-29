using System;
using System.Collections.Generic;

namespace bEmu.Core.CPU
{
    public static class BitFacade
    {
        private static LittleEndian littleEndian = LittleEndian.Instance;

        public static ushort GetWordFrom2Bytes(byte lsb, byte msb)
        {
            return littleEndian.GetWordFrom2Bytes(lsb, msb);
        }

        public static uint GetDWordFrom4Bytes(byte b1, byte b2, byte b3, byte b4)
        {
            return littleEndian.GetDWordFrom4Bytes(b1, b2, b3, b4);
        }

        public static void Get2BytesFromWord(ushort value, out byte msb, out byte lsb)
        {
            littleEndian.Get2BytesFromWord(value, out msb, out lsb);
        }

        public static IEnumerable<byte> ToBytes(int num)
        {
            return littleEndian.ToBytes(num);
        }

        public static IEnumerable<byte> ToBytes(uint num)
        {
            return littleEndian.ToBytes(num);
        }

        public static IEnumerable<byte> ToBytes(ulong num)
        {
            return littleEndian.ToBytes(num);
        }

        public static IEnumerable<byte> ToBytes(short num)
        {
            return littleEndian.ToBytes(num);
        }

        public static IEnumerable<byte> ToBytes(ushort num)
        {
            return littleEndian.ToBytes(num);
        }

        public static IEnumerable<byte> ToBytes<T>(IEnumerable<T> itens)
        {
            return littleEndian.ToBytes<T>(itens);
        }

        public static IEnumerable<byte> ToBytes(byte value)
        {
            return littleEndian.ToBytes(value);
        }

        public static IEnumerable<byte> ToBytes(bool value)
        {
            return littleEndian.ToBytes(value);
        }

        public static IEnumerable<byte> ToBytes(object num, Type type)
        {
            return littleEndian.ToBytes(num, type);
        }
        
        public static T FromBytes<T>(IEnumerable<byte> bytes) where T : struct
        {
            return littleEndian.FromBytes<T>(bytes);
        }

        public static T[] FromBytes<T>(IEnumerable<byte> bytes, int arraySize) where T : struct
        {
            return littleEndian.FromBytes<T>(bytes, arraySize);
        }
    }
}