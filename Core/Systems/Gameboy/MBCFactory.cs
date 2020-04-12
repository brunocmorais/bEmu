using System;

namespace bEmu.Core.Systems.Gameboy
{
    public static class MBCFactory
    {
        public static IMBC GetMBC(byte type)
        {
            switch (type)
            {
                case 0x0:
                case 0x1:
                case 0x2:
                case 0x3:
                    return new MBC1();
                default:
                    throw new Exception("Tipo de cartucho não suportado.");
            }
        }
    }
}