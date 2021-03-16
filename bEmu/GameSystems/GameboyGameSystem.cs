using bEmu.Classes;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public class GameboyGameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.GameBoy;
        
        public GameboyGameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new GameboyOptions(mainGame, MainGame.Options);

            if (MainGame.Options.Size < 2) 
                MainGame.Options.Size = 2;
        }
    }
}