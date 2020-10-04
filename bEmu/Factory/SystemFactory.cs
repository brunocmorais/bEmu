using System;
using bEmu.Core;
using bEmu.Systems;

namespace bEmu.Factory
{
    public static class SystemFactory
    {
        public static ISystem Get(SupportedSystems system, string rom)
        {
            switch (system)
            {
                case SupportedSystems.Chip8:       return new Systems.Chip8.System(rom);
                case SupportedSystems.GameBoy:     return new Systems.Gameboy.System(rom);
                case SupportedSystems.Generic8080: return new Systems.Generic8080.System(rom);
                default: throw new ArgumentException("Sistema não suportado.");
            }
        }
    }
}