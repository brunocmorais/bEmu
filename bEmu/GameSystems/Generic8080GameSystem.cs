using System;
using System.Collections.Generic;
using System.IO;
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
    public class Generic8080GameSystem : IGameSystem
    {
        public SupportedSystems Type => SupportedSystems.Generic8080;
        private const int CycleCount = 8000;
        private TimeSpan lastInterruptTime;
        private int lastInterrupt;
        private byte lastWrite3;
        private byte lastWrite5;
        private int cycle = 0;
        public ISystem System { get; }
        private readonly Systems.Generic8080.PPU gpu;
        private readonly Systems.Generic8080.State state;
        private readonly Systems.Generic8080.MMU mmu;

        private SoundEffect shot;
        private SoundEffect invaderDie;
        private SoundEffect explosion;
        private SoundEffect ufoLowPitch;
        private SoundEffectInstance ufoHighPitch;
        private SoundEffect fastInvader1;
        private SoundEffect fastInvader2;
        private SoundEffect fastInvader3;
        private SoundEffect fastInvader4;
        public int Width => 224;
        public int Height => 256;
        public IMainGame MainGame { get; }
        public int Frame { get => gpu.Frame; set => gpu.Frame = value; }
        public int Frameskip { get => gpu.Frameskip; set => gpu.Frameskip = value; }
        public Framebuffer Framebuffer => gpu.Framebuffer;
        public int RefreshRate => 8;

        public Generic8080GameSystem(IMainGame mainGame, string rom)
        {
            System = SystemFactory.Get(SupportedSystems.Generic8080, rom) as Systems.Generic8080.System;
            gpu = System.PPU as Systems.Generic8080.PPU;
            state = System.State as Systems.Generic8080.State;
            mmu = System.MMU as Systems.Generic8080.MMU;
            MainGame = mainGame;
            MainGame.Options = new Options(MainGame);
            MainGame.Options.Size = 2;
        }

        public void Initialize()
        {
            lastInterruptTime = TimeSpan.Zero;
            lastInterrupt = 1;
        }

        public void LoadContent()
        {
            shot = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/shot");
            invaderDie = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/invader_die");
            explosion = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/explosion");
            ufoLowPitch = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_lowpitch");
            fastInvader1 = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader1");
            fastInvader2 = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader2");
            fastInvader3 = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader3");
            fastInvader4 = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/fastinvader4");
            ufoHighPitch = MainGame.Content.Load<SoundEffect>("Generic8080/SpaceInvaders/ufo_highpitch").CreateInstance();
            ufoHighPitch.IsLooped = false;
            string json = File.ReadAllText("Content/Generic8080/games.json");
            var games = JsonConvert.DeserializeObject<IList<GameInfo>>(json);
            (System as Systems.Generic8080.System).LoadZipFile(games);
            state.UpdatePorts(1, 0x01);
            state.UpdatePorts(2, 0x00);
        }

        public void Update(GameTime gameTime)
        {
            lock (this)
            {
                cycle = CycleCount;
                
                if (state.EnableInterrupts)
                {
                    lastInterruptTime = gameTime.TotalGameTime;
                    lastInterrupt = lastInterrupt == 1 ? 2 : 1;

                    if (lastInterrupt == 1)
                        Frame++;

                    GenerateInterrupt(lastInterrupt);
                }

                UpdateSounds();
            }
        }

        public void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                {
                    System.Runner.StepCycle();
                }
            }
        }

        public void UpdateGamePad(KeyboardState keyboardState)
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

            state.UpdatePorts(1, read1);
        }

        private void GenerateInterrupt(int interruptNumber)
        {
            (System.Runner as CPU).GenerateInterrupt(interruptNumber);
        }

        private void UpdateSounds()
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

        public void StopGame() { }
    }
}