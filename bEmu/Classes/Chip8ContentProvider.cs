using Microsoft.Xna.Framework.Audio;

namespace bEmu.Classes
{
    public static class Chip8ContentProvider
    {
        public static SoundEffect Tone { get; private set; }
        public static SoundEffectInstance SoundEffectInstance { get; private set; }

        public static void Initialize(IMainGame mainGame)
        {
            Tone = mainGame.Content.Load<SoundEffect>("Chip8/tone");
            SoundEffectInstance = Tone.CreateInstance();
        }
    }
}