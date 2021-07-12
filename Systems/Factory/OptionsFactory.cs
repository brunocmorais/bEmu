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
            IOptions options;
            int size;

            switch (game.System.Type)
            {
                case SystemType.Chip8:
                    size = 5;
                    break;
                case SystemType.Generic8080:
                case SystemType.GameBoy:
                    size = 2;
                    break;
                default:
                    size = 1;
                    break;
            }

            if (game.System.Type == SystemType.GameBoy)
                options = new Systems.Gameboy.Options(game, size);
            else
                options = new Core.System.Options(game, size);

            return options;
        }
    }
}