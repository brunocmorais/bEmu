using bEmu.Classes;
using bEmu.Components;
using bEmu.Systems;
using Microsoft.Xna.Framework;

namespace bEmu.GameSystems
{

    public class Chip8GameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.Chip8;
        
        public Chip8GameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 5;
        }

        public override void Initialize(int address)
        {
            base.Initialize(0x200);
        }
    }
}