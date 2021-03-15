using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Chip8;
using bEmu.Systems.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{

    public class Chip8GameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.Chip8;
        private readonly Keys[] keys = new Keys[]
        {
            Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.Q, Keys.W, Keys.E, Keys.R,
            Keys.A, Keys.S, Keys.D, Keys.F,
            Keys.Z, Keys.X, Keys.C, Keys.V
        };
        
        public Chip8GameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 5;
        }

        public override void Initialize()
        {
            System.MMU.LoadProgram(0x200);
        }

        public override void Update(GameTime gameTime)
        {
            Systems.Chip8.State state = (Systems.Chip8.State) System.State;

            if (state.Delay > 0)
                state.Delay--;

            if (state.Sound > 0)
                state.Sound--;

            UpdateSound();
            System.ResetCycles();
        }

        private void UpdateSound()
        {
            Systems.Chip8.State state = (Systems.Chip8.State) System.State;

            if (state.Sound == 0)
            {
                Chip8ContentProvider.SoundEffectInstance.IsLooped = false;
                Chip8ContentProvider.SoundEffectInstance.Stop();
            }
            else if (state.Sound > 0 && !Chip8ContentProvider.SoundEffectInstance.IsLooped)
            {
                Chip8ContentProvider.SoundEffectInstance.IsLooped = true;
                Chip8ContentProvider.SoundEffectInstance.Play();
            }
        }

        public override void UpdateGamePad(KeyboardState keyboardState)
        {
            var gamePad = new Systems.Chip8.GamePad();

            for (int i = 0; i < keys.Length; i++)
                gamePad.Keys[BIOS.Keyboard[i]] = keyboardState.IsKeyDown(keys[i]);

            System.UpdateGamePad(gamePad);
        }
    }
}