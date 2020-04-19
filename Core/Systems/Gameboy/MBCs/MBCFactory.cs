using System;
using bEmu.Core.Systems.MBCs;

namespace bEmu.Core.Systems.Gameboy.MBCs
{
    public static class MBCFactory
    {
        public static IMBC GetMBC(byte type)
        {
            switch (type)
            {
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                    return new MBC1();
                case 0x05:
                case 0x06:
                    return new MBC2();
                case 0x0F:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                    return new MBC3();
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                case 0x1E:
                    return new MBC5();
                default: throw new Exception("Tipo de cartucho não suportado.");
            }
        }
    }
}