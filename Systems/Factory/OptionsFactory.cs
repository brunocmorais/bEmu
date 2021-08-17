using System;
using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.GUI;
using bEmu.Core.Factory;

namespace bEmu.Systems.Factory
{
    public class OptionsFactory : Factory<OptionsFactory, SystemType, IOptions>
    {
        public override IOptions Get(SystemType type, params object[] parameters)
        {
            var game = parameters[0] as IMain;

            switch (type)
            {
                case SystemType.Chip8:
                    return new Systems.Chip8.Options(game, 5);
                case SystemType.Generic8080:
                    return new Systems.Generic8080.Options(game, 2);
                case SystemType.GameBoy:
                    return new Systems.Gameboy.Options(game, 2);
                default:
                    return new Core.System.Options(game, 1);
            }
        }
    }
}