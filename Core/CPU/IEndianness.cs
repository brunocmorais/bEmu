using System;
using System.Collections.Generic;

namespace bEmu.Core.CPU
{
    public interface IEndianness
    {
        ushort GetWordFrom2Bytes(byte lsb, byte msb);
        uint GetDWordFrom4Bytes(byte b1, byte b2, byte b3, byte b4);
        void Get2BytesFromWord(ushort value, out byte msb, out byte lsb);
        IEnumerable<byte> ToBytes(int num);
        IEnumerable<byte> ToBytes(uint num);
        IEnumerable<byte> ToBytes(ulong num);
        IEnumerable<byte> ToBytes(short num);
        IEnumerable<byte> ToBytes(ushort num);
        IEnumerable<byte> ToBytes<T>(IEnumerable<T> itens);
        IEnumerable<byte> ToBytes(byte value);
        IEnumerable<byte> ToBytes(bool value);
        IEnumerable<byte> ToBytes(object num, Type type);
        T FromBytes<T>(IEnumerable<byte> bytes) where T : struct;
        T[] FromBytes<T>(IEnumerable<byte> bytes, int arraySize) where T : struct;
        
    }
}