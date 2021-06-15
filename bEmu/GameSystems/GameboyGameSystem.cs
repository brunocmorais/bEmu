using bEmu.Classes;
using bEmu.Core;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{
    public class GameboyGameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.GameBoy;
        private DynamicSoundEffectInstance sound;
        private Systems.Gameboy.Sound.APU apu => this.System.APU as Systems.Gameboy.Sound.APU;
        
        public GameboyGameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new GameboyOptions(mainGame, MainGame.Options);

            if (MainGame.Options.Size < 2) 
                MainGame.Options.Size = 2;

            sound = new DynamicSoundEffectInstance(Systems.Gameboy.Sound.APU.SampleRate, AudioChannels.Stereo);
            sound.Play();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (sound.PendingBufferCount < 3)
                sound.SubmitBuffer(apu.UpdateBuffer());
        }
    }
}