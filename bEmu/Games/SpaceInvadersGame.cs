using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Core.CPUs.Intel8080;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace bEmu
{
    public class SpaceInvadersGame : Generic8080Game
    {
        private Texture2D backdrop;
        private SoundEffect shot;
        private SoundEffect invaderDie;
        private SoundEffect explosion;
        private SoundEffect ufoLowPitch;
        private SoundEffectInstance ufoHighPitch;
        private SoundEffect fastInvader1;
        private SoundEffect fastInvader2;
        private SoundEffect fastInvader3;
        private SoundEffect fastInvader4;
        private Color backdropColor = Color.FromNonPremultiplied(255, 255, 255, 160);

        public SpaceInvadersGame(string rom) : base(rom) { }

        protected override void LoadContent()
        {
            backdrop = Content.Load<Texture2D>("Generic8080/SpaceInvaders/backdrop");
            shot = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/shot");
            invaderDie = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/invader_die");
            explosion = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/explosion");
            ufoLowPitch = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_lowpitch");
            fastInvader1 = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader1");
            fastInvader2 = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader2");
            fastInvader3 = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader3");
            fastInvader4 = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader4");
            ufoHighPitch = Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_highpitch").CreateInstance();
            ufoHighPitch.IsLooped = false;
            Alpha = 160;

            base.LoadContent();
        }

        protected override void Draw (GameTime gameTime)
		{
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin ();
            SpriteBatch.Draw(backdrop, new Rectangle(0, 0, Width * Options.Size, Height * Options.Size), backdropColor);
            base.Draw(gameTime);
            SpriteBatch.End ();
		}

        protected override void UpdateSounds()
        {
            byte write3 = state.Ports.Write3;
            byte write5 = state.Ports.Write5;

            bool playShot = ((write3 & (1 << 1)) >> 1) == 1;
            bool wasPlayingShot = ((lastWrite3 & (1 << 1)) >> 1) == 1;
            bool playExplosion = ((write3 & (1 << 2)) >> 2) == 1;
            bool wasPlayingExplosion = ((lastWrite3 & (1 << 2)) >> 2) == 1;
            bool playInvaderDie = ((write3 & (1 << 3)) >> 3) == 1;
            bool wasPlayingInvaderDie = ((lastWrite3 & (1 << 3)) >> 3) == 1;

            if (playShot && !wasPlayingShot)
                shot.Play();
            if (playInvaderDie && !wasPlayingInvaderDie)
                invaderDie.Play();
            if (playExplosion && !wasPlayingExplosion)
                explosion.Play();

            bool playFastInvader1 = ((write5 & (1 << 0)) >> 0) == 1;
            bool wasPlayingFastInvader1 = ((lastWrite5 & (1 << 0)) >> 0) == 1;
            bool playFastInvader2 = ((write5 & (1 << 1)) >> 1) == 1;
            bool wasPlayingFastInvader2 = ((lastWrite5 & (1 << 1)) >> 1) == 1;
            bool playFastInvader3 = ((write5 & (1 << 2)) >> 2) == 1;
            bool wasPlayingFastInvader3 = ((lastWrite5 & (1 << 2)) >> 2) == 1;
            bool playFastInvader4 = ((write5 & (1 << 3)) >> 3) == 1;
            bool wasPlayingFastInvader4 = ((lastWrite5 & (1 << 3)) >> 3) == 1;
            bool playUfoLowPitch = ((write5 & (1 << 4)) >> 4) == 1;
            bool wasPlayingUfoLowPitch = ((lastWrite5 & (1 << 4)) >> 4) == 1;

            if (playFastInvader1 && !wasPlayingFastInvader1)
                fastInvader1.Play();
            if (playFastInvader2 && !wasPlayingFastInvader2)
                fastInvader2.Play();
            if (playFastInvader3 && !wasPlayingFastInvader3)
                fastInvader3.Play();
            if (playFastInvader4 && !wasPlayingFastInvader4)
                fastInvader4.Play();
            if (playUfoLowPitch && !wasPlayingUfoLowPitch)
                ufoLowPitch.Play();

            ufoHighPitch.IsLooped = (write3 & 1) == 1;

            if (ufoHighPitch.IsLooped)
                ufoHighPitch.Play();
            else
                ufoHighPitch.Stop();

            lastWrite3 = write3;
            lastWrite5 = write5;
        }
    }
}