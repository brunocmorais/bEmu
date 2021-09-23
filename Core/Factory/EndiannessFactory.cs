using System;
using bEmu.Core.Enums;
using bEmu.Core.Factory;

namespace bEmu.Core.CPU
{
    public class EndiannessFactory : Factory<EndiannessFactory, Endianness, IEndianness>
    {
        public override IEndianness Get(Endianness type, params object[] parameters)
        {
            if (type == Endianness.LittleEndian)
                return LittleEndian.Instance;
            else if (type == Endianness.BigEndian)
                return BigEndian.Instance;
            else
                throw new ArgumentException("Invalid endianness type");
        }
    }
}