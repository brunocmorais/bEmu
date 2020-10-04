using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Components;
using bEmu.Core;
using bEmu.Factory;
using bEmu.Systems;
using bEmu.Systems.Generic8080;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Generic8080 = bEmu.Systems.Generic8080;
using MMU = bEmu.Systems.Generic8080.MMU;
using State = bEmu.Systems.Generic8080.State;

namespace bEmu
{
    public class Generic8080Game : BaseGame
    {
        protected const int Delay = 8;
        protected const int CycleCount = 8000;
        protected int Alpha = 255;
        protected TimeSpan lastInterruptTime;
        protected int lastInterrupt;
        protected byte lastWrite3;
        protected byte lastWrite5;
        protected string[] fileNames;
        protected string[] memoryPositions;
        private int cycle = 0;
        protected readonly Generic8080.System system;
        protected readonly Generic8080.PPU gpu;
        protected readonly State state;
        protected readonly MMU mmu;

        public Generic8080Game(string rom) : base(SystemFactory.Get(SupportedSystems.Generic8080, rom), 224, 256, 2)
        {
            system = System as Generic8080.System;
            gpu = Gpu as Generic8080.PPU;
            state = State as State;
            mmu = Mmu as Generic8080.MMU;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, Delay);
            lastInterruptTime = TimeSpan.Zero;
            lastInterrupt = 1;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            string json = File.ReadAllText("Content/Generic8080/games.json");
            var games = JsonConvert.DeserializeObject<IList<GameInfo>>(json);
            system.LoadZipFile(games);
            state.UpdatePorts(1, 0x01);
            state.UpdatePorts(2, 0x00);
            IsRunning = true;
            StartMainThread();
        }

        protected override void Update(GameTime gameTime)
        {
            lock (this)
            {
                cycle = CycleCount;
                
                if (state.EnableInterrupts)
                {
                    lastInterruptTime = gameTime.TotalGameTime;
                    lastInterrupt = lastInterrupt == 1 ? 2 : 1;
                    GenerateInterrupt(lastInterrupt);
                }

                UpdateSounds();
            }

            base.Update(gameTime);
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

            state.UpdatePorts(1, read1);
        }

        protected override void Draw (GameTime gameTime)
		{
            if (this is SpaceInvadersGame)
            {
                gpu.UpdateFrameBuffer();
                base.Draw (gameTime);
            }
            else
            {
                GraphicsDevice.Clear (Color.Black);
                gpu.UpdateFrameBuffer();
                SpriteBatch.Begin();
                base.Draw (gameTime);
                SpriteBatch.End();
            }
		}

        protected virtual void GenerateInterrupt(int interruptNumber)
        {
            (system.Runner as CPU).GenerateInterrupt(interruptNumber);
        }

        protected virtual void UpdateSounds()
        {            
            byte write3 = state.Ports.Write3;
            byte write5 = state.Ports.Write5;            
            
            lastWrite3 = write3;
            lastWrite5 = write5;
        }

        public override void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                {
                    system.Runner.StepCycle();
                }   
            }
        }

        protected override void OnOptionChanged(object sender, OnOptionChangedEventArgs e)
        {
            base.OnOptionChanged(sender, e);
        }

        public override void ResetGame()
        {
            base.ResetGame();
            IsRunning = false;
            System.Reset();
            lastInterruptTime = TimeSpan.Zero;
            lastInterrupt = 1;
            state.UpdatePorts(1, 0x01);
            state.UpdatePorts(2, 0x00);
            IsRunning = true;
            StartMainThread();
        }
    }
}