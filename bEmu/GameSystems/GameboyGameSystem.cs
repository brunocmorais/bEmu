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

        public override void Initialize() 
        { 
            System.MMU.LoadProgram();
        }

        public override void Update(GameTime gameTime)
        {
            System.ResetCycles();
        }

        
        public override void UpdateGamePad(KeyboardState keyboardState) 
        { 
            var joypad = ((Systems.Gameboy.MMU) System.MMU).Joypad;

            if (keyboardState.IsKeyDown(Keys.Z))
                joypad.Column1 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.X))
                joypad.Column1 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.RightShift))
                joypad.Column1 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Enter))
                joypad.Column1 &= 0x7;
            if (keyboardState.IsKeyDown(Keys.Right))
                joypad.Column2 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.Left))
                joypad.Column2 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.Up))
                joypad.Column2 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Down))
                joypad.Column2 &= 0x7;

            if (keyboardState.IsKeyUp(Keys.Z))
                joypad.Column1 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.X))
                joypad.Column1 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.RightShift))
                joypad.Column1 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Enter))
                joypad.Column1 |= 0x8;
            if (keyboardState.IsKeyUp(Keys.Right))
                joypad.Column2 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.Left))
                joypad.Column2 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.Up))
                joypad.Column2 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Down))
                joypad.Column2 |= 0x8;

            System.UpdateGamePad(joypad);
        }

        public override void StopGame()
        {
            System.Stop();
        }
    }
}