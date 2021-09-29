using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{

    public class LittleEndian : Singleton<LittleEndian>, IEndianness
    {
        public ushort GetWordFrom2Bytes(byte lsb, byte msb)
        {
            return (ushort)((msb << 8) | lsb);
        }

        public uint GetDWordFrom4Bytes(byte b1, byte b2, byte b3, byte b4)
        {
            return (uint)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1);
        }

        public void Get2BytesFromWord(ushort value, out byte msb, out byte lsb)
        {
            msb = (byte) ((value & 0xFF00) >> 8);
            lsb = (byte) ((value & 0xFF));
        }

        public IEnumerable<byte> ToBytes(int num)
        {
            for (int i = 0; i < sizeof(int); i++)
                yield return (byte)((num >> ((sizeof(int) - 1 - i) * 8)) & 0xFF);
        }

        public IEnumerable<byte> ToBytes(uint num)
        {
            for (int i = 0; i < sizeof(uint); i++)
                yield return (byte)((num >> ((sizeof(uint) - 1 - i) * 8)) & 0xFF);
        }

        public IEnumerable<byte> ToBytes(ulong num)
        {
            for (int i = 0; i < sizeof(ulong); i++)
                yield return (byte)((num >> ((sizeof(ulong) - 1 - i) * 8)) & 0xFF);
        }

        public IEnumerable<byte> ToBytes(short num)
        {
            for (int i = 0; i < sizeof(short); i++)
                yield return (byte)((num >> ((sizeof(short) - 1 - i) * 8)) & 0xFF);
        }

        public IEnumerable<byte> ToBytes(ushort num)
        {
            for (int i = 0; i < sizeof(ushort); i++)
                yield return (byte)((num >> ((sizeof(ushort) - 1 - i) * 8)) & 0xFF);
        }

        public IEnumerable<byte> ToBytes<T>(IEnumerable<T> itens)
        {
            foreach (var item in itens)
                foreach (var @byte in ToBytes(item, typeof(T)))
                    yield return @byte;
        }

        public IEnumerable<byte> ToBytes(byte value) => Enumerable.Empty<byte>().Append(value);

        public IEnumerable<byte> ToBytes(bool value) => Enumerable.Empty<byte>().Append((byte)(value ? 1 : 0));

        public IEnumerable<byte> ToBytes(object num, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32: return ToBytes((int) num);
                case TypeCode.UInt32: return ToBytes((uint) num);
                case TypeCode.Int16: return ToBytes((short) num);
                case TypeCode.UInt16: return ToBytes((ushort) num);
                case TypeCode.Boolean: return ToBytes((bool) num);
                case TypeCode.Byte: return ToBytes((byte) num);
                default: throw new NotImplementedException();
            }
        }
        
        public T FromBytes<T>(IEnumerable<byte> bytes) where T : struct
        {
            ulong num = 0;
            
            for (int i = 0; i < bytes.Count(); i++)
            {
                ulong v = (ulong) (bytes.ElementAt(i) << ((bytes.Count() - 1 - i) * 8));
                num |= v;
            }

            return (T) Convert.ChangeType(num, typeof(T));
        }

        public T[] FromBytes<T>(IEnumerable<byte> bytes, int arraySize) where T : struct
        {
            var array = new T[arraySize];
            int dataSize = bytes.Count() / arraySize;

            for (int i = 0; i < array.Length; i++)
            {
                ulong num = 0;

                for (int j = dataSize - 1; j >= 0; j--)
                {
                    ulong value = (ulong) (bytes.ElementAt((i * dataSize) + (dataSize - 1 - j)) << (j * 8));
                    num |= value;
                }

                array[i] = (T) Convert.ChangeType(num, typeof(T));
            }

            return array;
        }
    }
}