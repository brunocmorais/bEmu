using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Extensions;
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
        
        public Chip8GameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 5;
        }

        public override void Initialize(int address)
        {
            base.Initialize(0x200);
        }

        public override void Update()
        {
            base.Update();
            var state = (Systems.Chip8.State) System.State;

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
    }
}