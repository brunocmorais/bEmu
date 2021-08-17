using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.Factory;
using bEmu.Core.System;

namespace bEmu.Systems.Factory
{
    // public static class SystemFactory
    // {
    //     public static ISystem Get(SystemType system, string rom)
    //     {
    //         switch (system)
    //         {
    //             case SystemType.Chip8:
    //                 return new Chip8.System(rom);
    //             case SystemType.GameBoy:
    //                 return new Gameboy.System(rom);
    //             case SystemType.Generic8080:
    //                 return new Generic8080.System(rom);
    //             default:
    //                 return new EmptySystem();
    //         }
    //     }
    // }

    public class SystemFactory : Factory<SystemFactory, SystemType, ISystem>
    {
        public override ISystem Get(SystemType type, params object[] parameters)
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
                    return new EmptySystem();
            }
        }
    }
}