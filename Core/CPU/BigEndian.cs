using System;
using System.Collections.Generic;
using bEmu.Core.Util;

namespace bEmu.Core.CPU
{
    public class BigEndian : Singleton<BigEndian>, IEndianness
    {
        public T FromBytes<T>(IEnumerable<byte> bytes) where T : struct
        {
            throw new NotImplementedException();
        }

        public T[] FromBytes<T>(IEnumerable<byte> bytes, int arraySize) where T : struct
        {
            throw new NotImplementedException();
        }

        public void Get2BytesFromWord(ushort value, out byte msb, out byte lsb)
        {
            throw new NotImplementedException();
        }

        public uint GetDWordFrom4Bytes(byte b1, byte b2, byte b3, byte b4)
        {
            throw new NotImplementedException();
        }

        public ushort GetWordFrom2Bytes(byte lsb, byte msb)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(int num)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(uint num)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(ulong num)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(short num)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(ushort num)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes<T>(IEnumerable<T> itens)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(byte value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(bool value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte> ToBytes(object num, Type type)
        {
            throw new NotImplementedException();
        }
    }
}