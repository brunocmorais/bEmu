using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.Factory;
using bEmu.Core.System;

namespace bEmu.Systems.Factory
{
    public class SystemFactory : Factory<SystemFactory, SystemType, IRunnableSystem>
    {
        public override IRunnableSystem Get(SystemType type, params object[] parameters)
        {
            var rom = parameters[0] as string;

            switch (type)
            {
                case SystemType.Chip8:
                    return new Chip8.System(rom);
                case SystemType.GameBoy:
                    return new Gameboy.System(rom);
                case SystemType.Generic8080:
                    return new Generic8080.System(rom);
                default:
                    //return new EmptySystem();
                    throw new Exception("Sistema não suportado.");
            }
        }

        public ISystem GetEmptySystem() => new EmptySystem();
    }
}