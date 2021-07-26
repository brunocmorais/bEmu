using System;
using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.GUI;

namespace bEmu.Systems.Factory
{
    public static class OptionsFactory
    {
        public static IOptions Build(IMain game)
        {
            switch (game.System.Type)
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