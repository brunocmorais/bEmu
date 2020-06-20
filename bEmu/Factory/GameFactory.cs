using System.IO;
using bEmu.Core.Systems;
using bEmu.Exceptions;
using Microsoft.Xna.Framework;

namespace bEmu.Factory
{
    public static class GameFactory
    {
        public static Game GetGame(SupportedSystems system, string rom)
        {
            switch (system)
            {
                case SupportedSystems.Generic8080:
                    string gameToRun = Path.GetFileNameWithoutExtension(rom);
                    
                    if (gameToRun == "invaders" || gameToRun == "invadpt2")
                        return new SpaceInvadersGame(rom);
                    else
                        return new Generic8080Game(rom);
                case SupportedSystems.Chip8:
                    return new Chip8Game(rom);
                case SupportedSystems.GameBoy:
                    return new GameboyGame(rom);
                default:
                    throw new SystemNotSupportedException();
            }
        }
    }
}