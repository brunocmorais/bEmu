using bEmu.GameSystems;
using bEmu.Systems;

namespace bEmu.Factory
{
    public static class GameSystemFactory
    {
        public static IGameSystem Get(SupportedSystems system, IMainGame game, string rom)
        {
            switch (system)
            {
                case SupportedSystems.Chip8:
                    return new Chip8GameSystem(game, rom);
                case SupportedSystems.Generic8080:
                    return new Generic8080GameSystem(game, rom);
                case SupportedSystems.GameBoy:
                    return new GameboyGameSystem(game, rom);
                default:
                    return GetDummyGameSystem(game);
            }
        }

        public static GameSystem GetDummyGameSystem(IMainGame game)
        {
            return new GameSystem(game, string.Empty);
        }
    }
}