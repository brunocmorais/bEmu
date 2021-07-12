using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.System;

namespace bEmu.Systems.Factory
{
    public static class SystemFactory
    {
        public static ISystem Get(SystemType system, string rom)
        {
            switch (system)
            {
                case SystemType.Chip8:
                    return new Chip8.System(rom);
                case SystemType.GameBoy:
                    return new Gameboy.System(rom);
                case SystemType.Generic8080:
                    return new Generic8080.System(rom);
                default:
                    return new EmptySystem();
            }
        }
    }
}