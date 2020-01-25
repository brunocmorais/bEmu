using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;

namespace bEmu.Intel8080
{
    public class SpaceInvadersGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        Texture2D backdrop;
        CPU cpu;
        TimeSpan lastInterruptTime = TimeSpan.Zero;
        int lastInterrupt = 1;
        const int tamanhoPixel = 2;
        SoundEffect shot;
        SoundEffect invaderDie;
        SoundEffect explosion;
        SoundEffect ufoLowPitch;
        SoundEffectInstance ufoHighPitch;
        SoundEffect fastInvader1;
        SoundEffect fastInvader2;
        SoundEffect fastInvader3;
        SoundEffect fastInvader4;
        byte lastWrite3;
        byte lastWrite5;
        const int width = 224 * tamanhoPixel;
        const int height = 256 * tamanhoPixel;
        Color backdropColor = Color.FromNonPremultiplied(255, 255, 255, 128);
        string game;

        public SpaceInvadersGame(string gameToRun)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            game = gameToRun;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 8);
        }

        protected override void Initialize()
        {
            var gameInfos = JsonConvert.DeserializeObject<IList<GameInfo>>(File.ReadAllText("Intel8080/games.json"));
            var gameInfo = gameInfos.FirstOrDefault(x => x.zipName == game);

            var entries = new Dictionary<string, byte[]>();
            
            using (var zipFile = ZipFile.OpenRead($"Intel8080/Content/{gameInfo.zipName}.zip"))
            {
                foreach (var fileName in gameInfo.fileNames)
                {
                    var stream = zipFile.GetEntry(fileName).Open();
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        entries.Add(fileName, memoryStream.ToArray());
                    }
                }
            }

            cpu = new CPU();

            for (int i = 0; i < gameInfo.fileNames.Length; i++)
                cpu.State.LoadProgram(entries[gameInfo.fileNames[i]], Convert.ToInt32(gameInfo.memoryPositions[i], 16));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whiteRect = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);
            backdrop = Content.Load<Texture2D>("backdrop");

            shot = Content.Load<SoundEffect>("shot");
            invaderDie = Content.Load<SoundEffect>("invader_die");
            explosion = Content.Load<SoundEffect>("explosion");
            ufoLowPitch = Content.Load<SoundEffect>("ufo_lowpitch");
            fastInvader1 = Content.Load<SoundEffect>("fastinvader1");
            fastInvader2 = Content.Load<SoundEffect>("fastinvader2");
            fastInvader3 = Content.Load<SoundEffect>("fastinvader3");
            fastInvader4 = Content.Load<SoundEffect>("fastinvader4");
            ufoHighPitch = Content.Load<SoundEffect>("ufo_highpitch").CreateInstance();
            ufoHighPitch.IsLooped = false;

            Color[] whiteColor = new Color[tamanhoPixel * tamanhoPixel];
            
            for(int i = 0; i < whiteColor.Length; i++) 
                whiteColor[i] = Color.White;
            
            whiteRect.SetData(whiteColor);

            cpu.UpdatePorts(1, 0x01);
            cpu.UpdatePorts(2, 0x00);
        }

        protected override void Update(GameTime gameTime)
        {
            double totalMilliseconds = (gameTime.TotalGameTime - lastInterruptTime).TotalMilliseconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            int cycle = 3000;

            while (cycle-- >= 0)
            {
                byte opcode = cpu.EmularCiclo();

                if (opcode == 0xDB) //IN
                    In();
                else if (opcode == 0xD3) //OUT
                    Out();
            }

            if (totalMilliseconds >= 8)
            {
                if (cpu.State.EnableInterrupts)
                {
                    lastInterrupt = lastInterrupt == 1 ? 2 : 1;
                    GenerateInterrupt(lastInterrupt);
                    lastInterruptTime = gameTime.TotalGameTime;
                }
            }

            UpdateButtons();
            UpdateSounds();

            base.Update(gameTime);
        }

        private void UpdateButtons()
        {
            byte read1 = 0;
            // créditos
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

            cpu.UpdatePorts(1, read1);
        }

        protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin ();
            spriteBatch.Draw(backdrop, new Rectangle(0, 0, width, height), backdropColor);
            Color color;
			
			for (int i = 0; i < 224; i++) 
			{
				for (int j = 0; j < 256; j += 8)
				{
					byte sprite = cpu.State.Memory [0x2400 + ((i * 256 / 8) + j / 8)];

					for (int pixel = 0; pixel < 8; pixel++) 
                    {
                        int x = (j + pixel) * tamanhoPixel;
                        int y = i * tamanhoPixel;

                        Vector2 coor = new Vector2(y, (256 * tamanhoPixel) - x);

                        if (coor.Y < (50 * tamanhoPixel))
                            color = Color.Red;
                        else if (coor.Y > (180 * tamanhoPixel))
                            color = Color.Lime;
                        else
                            color = Color.White;

						if ((sprite & (1 << pixel)) > 0) 
                            spriteBatch.Draw (whiteRect, coor, color);
					}
				}
			}

			spriteBatch.End ();

            base.Draw (gameTime);	
		}

        public void GenerateInterrupt(int interruptNumber)
        {
            cpu.GenerateInterrupt(interruptNumber);
        }

        public void In()
        {
            byte port = cpu.GetNextByte();
            var state = cpu.State;

			switch(port)
			{
                case 1:
                    state.A = cpu.State.Ports.Read1;
                    break;
                case 2:
                    state.A = cpu.State.Ports.Read2;
                    break;
                case 3:
                    ushort value = Util.Get16BitNumber(cpu.State.Ports.Shift0, cpu.State.Ports.Shift1);
                    state.A = (byte)((value >> (8 - cpu.State.Ports.Write2)) & 0xFF);
                    break;
                default:
                    break;
			}

            cpu.State = state;
        }

        public void Out()
        {
            byte port = cpu.GetNextByte();
            var state = cpu.State;
            
            switch(port)
			{
                case 2:
                    state.Ports.Write2 = (byte)(state.A & 0x7);
                    break;
                case 3:
                    state.Ports.Write3 = state.A;
                    break;
                case 4:
                    state.Ports.Shift0 = state.Ports.Shift1;
                    state.Ports.Shift1 = state.A;
                    break;
                case 5:
                    state.Ports.Write5 = state.A;
                    break;
                default:
                    break;
			}

            cpu.State = state;
        }

        public void UpdateSounds()
        {
            byte write3 = cpu.State.Ports.Write3;
            byte write5 = cpu.State.Ports.Write5;

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