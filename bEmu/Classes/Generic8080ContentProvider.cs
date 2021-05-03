using Microsoft.Xna.Framework.Audio;

namespace bEmu.Classes
{

    public static class Generic8080ContentProvider
    {
        public static SoundEffect Shot { get; private set; }
        public static SoundEffect InvaderDie { get; private set; }
        public static SoundEffect Explosion { get; private set; }
        public static SoundEffect UfoLowPitch { get; private set; }
        public static SoundEffectInstance UfoHighPitch { get; private set; }
        public static SoundEffect FastInvader1 { get; private set; }
        public static SoundEffect FastInvader2 { get; private set; }
        public static SoundEffect FastInvader3 { get; private set; }
        public static SoundEffect FastInvader4 { get; private set; }

        public static void Initialize(IMainGame mainGame)
        {
            Shot = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/shot");
            InvaderDie = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/invader_die");
            Explosion = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/explosion");
            UfoLowPitch = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_lowpitch");
            FastInvader1 = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader1");
            FastInvader2 = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader2");
            FastInvader3 = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader3");
            FastInvader4 = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader4");
            UfoHighPitch = mainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_highpitch").CreateInstance();
            UfoHighPitch.IsLooped = false;
        }
    }
}