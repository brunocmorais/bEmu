using System;
using System.Collections.Generic;
using System.IO;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Factory;
using bEmu.Systems.Generic8080;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace bEmu.GameSystems
{
    public class Generic8080GameSystem : GameSystem
    {
        public override SupportedSystems Type => SupportedSystems.Generic8080;
        private byte lastWrite3;
        private byte lastWrite5;
        
        public Generic8080GameSystem(IMainGame mainGame, string rom) : base(mainGame, rom)
        {
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 2;
        }

        public override void Initialize(int address)
        {
            string json = File.ReadAllText("Content/Generic8080/games.json");
            var games = JsonConvert.DeserializeObject<IList<GameInfo>>(json);
            (System as Systems.Generic8080.System).LoadZipFile(games);
        }

        public override void Update()
        {
            base.Update();
            UpdateSounds();
        }

        public override void UpdateGamePad(KeyboardState keyboardState)
        {
            byte read1 = 0;
            
            if (Keyboard.GetState().IsKeyDown(Keys.D5))
                read1 = (byte) (read1 | (1 << 0));
            
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
                read1 = (byte) (read1 | (1 << 2));
            
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                read1 = (byte) (read1 | (1 << 4));

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                read1 = (byte) (read1 | (1 << 5));

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                read1 = (byte) (read1 | (1 << 6));

            System.UpdateGamePad(new Systems.Generic8080.GamePad(read1));
        }

        private void UpdateSounds()
        {            
            Systems.Generic8080.State state = (Systems.Generic8080.State) System.State;
            byte write3 = state.Ports.Write3;
            byte write5 = state.Ports.Write5;

            bool playShot = ((write3 & (1 << 1)) >> 1) == 1;
            bool wasPlayingShot = ((lastWrite3 & (1 << 1)) >> 1) == 1;
            bool playExplosion = ((write3 & (1 << 2)) >> 2) == 1;
            bool wasPlayingExplosion = ((lastWrite3 & (1 << 2)) >> 2) == 1;
            bool playInvaderDie = ((write3 & (1 << 3)) >> 3) == 1;
            bool wasPlayingInvaderDie = ((lastWrite3 & (1 << 3)) >> 3) == 1;

            if (playShot && !wasPlayingShot)
                Generic8080ContentProvider.Shot.Play();
            if (playInvaderDie && !wasPlayingInvaderDie)
                Generic8080ContentProvider.InvaderDie.Play();
            if (playExplosion && !wasPlayingExplosion)
                Generic8080ContentProvider.Explosion.Play();

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
                Generic8080ContentProvider.FastInvader1.Play();
            if (playFastInvader2 && !wasPlayingFastInvader2)
                Generic8080ContentProvider.FastInvader2.Play();
            if (playFastInvader3 && !wasPlayingFastInvader3)
                Generic8080ContentProvider.FastInvader3.Play();
            if (playFastInvader4 && !wasPlayingFastInvader4)
                Generic8080ContentProvider.FastInvader4.Play();
            if (playUfoLowPitch && !wasPlayingUfoLowPitch)
                Generic8080ContentProvider.UfoLowPitch.Play();

            Generic8080ContentProvider.UfoHighPitch.IsLooped = (write3 & 1) == 1;

            if (Generic8080ContentProvider.UfoHighPitch.IsLooped)
                Generic8080ContentProvider.UfoHighPitch.Play();
            else
                Generic8080ContentProvider.UfoHighPitch.Stop();

            lastWrite3 = write3;
            lastWrite5 = write5;
        }
    }
}