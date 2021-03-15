using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{

    public class GameSystem : IGameSystem
    {
        public virtual SupportedSystems Type => 0;
        public virtual IMainGame MainGame { get; }
        public virtual ISystem System { get; }

        public GameSystem(IMainGame mainGame, string rom)
        {
            System = SystemFactory.Get(Type, rom);
            MainGame = mainGame;

            if (Type == 0)
            {
                MainGame.Options = new Options(MainGame);
                MainGame.Options.Size = 1;
            }
        }

        public virtual void Initialize(int address) 
        { 
            if (System.MMU != null)
                System.MMU.LoadProgram(address);
        }

        public virtual void Update() 
        { 
            if (MainGame.IsRunning && System.PPU.Frame <= MainGame.LastRenderedFrame)
            {
                if (System.Cycles < 0)
                    System.ResetCycles();

                System.Update();
            }
        }

        public virtual void UpdateGamePad(KeyboardState keyboardState) { }
        
        public virtual void StopGame() 
        { 
            System.Stop();
        }
    }
}