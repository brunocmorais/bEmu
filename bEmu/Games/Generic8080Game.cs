using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Core;
using bEmu.Core.Systems.Generic8080;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Generic8080 = bEmu.Core.Systems.Generic8080;
using State = bEmu.Core.Systems.Generic8080.State;

namespace bEmu
{
    public class Generic8080Game : BaseGame<Generic8080.System, Generic8080.State, MMU, Generic8080.PPU, APU>
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

        public Generic8080Game(string rom) : base(new Generic8080.System(), rom, 224, 256, 2)
        {
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, Delay);
            lastInterruptTime = TimeSpan.Zero;
            lastInterrupt = 1;
        }

        public void InitializeRomMetadata()
        {
            string gameToRun = Path.GetFileNameWithoutExtension(Rom);
            var gameInfos = JsonConvert.DeserializeObject<IList<GameInfo>>(File.ReadAllText("gamesIntel8080.json"));
            var gameInfo = gameInfos.FirstOrDefault(x => x.ZipName == gameToRun);
            
            fileNames = gameInfo.FileNames;
            memoryPositions = gameInfo.MemoryPositions;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeRomMetadata();
            LoadZipFile();
        }

        private void LoadZipFile()
        {
            var entries = new Dictionary<string, byte[]>();

            using (var zipFile = ZipFile.OpenRead($"{Rom}"))
            {
                foreach (var fileName in fileNames)
                {
                    var stream = zipFile.GetEntry(fileName).Open();

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        entries.Add(fileName, memoryStream.ToArray());
                    }
                }
            }

            for (int i = 0; i < fileNames.Length; i++)
                System.MMU.LoadProgram(entries[fileNames[i]], Convert.ToInt32(memoryPositions[i], 16));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            State.UpdatePorts(1, 0x01);
            State.UpdatePorts(2, 0x00);
            IsRunning = true;
            StartMainThread();
        }

        protected override void Update(GameTime gameTime)
        {
            lock (this)
            {
                cycle = CycleCount;
                
                if (State.EnableInterrupts)
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

            State.UpdatePorts(1, read1);
        }

        protected override void Draw (GameTime gameTime)
		{
            if (this is SpaceInvadersGame)
            {
                Gpu.UpdateFrameBuffer();
                base.Draw (gameTime);
            }
            else
            {
                GraphicsDevice.Clear (Color.Black);
                Gpu.UpdateFrameBuffer();
                SpriteBatch.Begin();
                base.Draw (gameTime);
                SpriteBatch.End();
            }
		}

        protected virtual void GenerateInterrupt(int interruptNumber)
        {
            (System.Runner as CPU).GenerateInterrupt(interruptNumber);
        }

        private void In()
        {
            
        }

        private void Out()
        {
            
        }

        protected virtual void UpdateSounds()
        {            
            byte write3 = State.Ports.Write3;
            byte write5 = State.Ports.Write5;            
            
            lastWrite3 = write3;
            lastWrite5 = write5;
        }

        public override void UpdateGame()
        {
            lock (this)
            {
                while (cycle-- >= 0)
                {
                    System.Runner.StepCycle();
                }   
            }
        }
    }
}