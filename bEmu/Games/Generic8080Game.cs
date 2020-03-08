using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using bEmu.Core;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Systems.Generic8080;
using bEmu.Core.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using State = bEmu.Core.Systems.Generic8080.State;

namespace bEmu
{
    public class Generic8080Game : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected Texture2D whiteRect;
        protected Core.Systems.Generic8080.System system;
        protected TimeSpan lastInterruptTime = TimeSpan.Zero;
        protected int lastInterrupt = 1;
        protected const int tamanhoPixel = 2;
        protected byte lastWrite3;
        protected byte lastWrite5;
        protected string[] fileNames;
        protected string[] memoryPositions;
        protected string zipName;
        protected const int width = 224 * tamanhoPixel;
        protected const int height = 256 * tamanhoPixel;
        protected const int delay = 8;
        protected const int CycleCount = 3000;
        protected int alpha = 255;

        protected State State => system.State as State;
        protected Intel8080<State> CPU => system.Runner as Intel8080<State>;

        public Generic8080Game(string zipName, string[] fileNames, string[] memoryPositions)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, delay);
            this.fileNames = fileNames;
            this.memoryPositions = memoryPositions; 
            this.zipName = zipName;
        }

        protected override void Initialize()
        {
            var entries = new Dictionary<string, byte[]>();
            
            using (var zipFile = ZipFile.OpenRead($"{zipName}"))
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

            system = new Core.Systems.Generic8080.System();

            for (int i = 0; i < fileNames.Length; i++)
                system.MMU.LoadProgram(entries[fileNames[i]], Convert.ToInt32(memoryPositions[i], 16));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whiteRect = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);

            Color[] whiteColor = new Color[tamanhoPixel * tamanhoPixel];
            
            for(int i = 0; i < whiteColor.Length; i++) 
                whiteColor[i] = Color.White;
            
            whiteRect.SetData(whiteColor);

            State.UpdatePorts(1, 0x01);
            State.UpdatePorts(2, 0x00);
        }

        protected override void Update(GameTime gameTime)
        {
            double totalMilliseconds = (gameTime.TotalGameTime - lastInterruptTime).TotalMilliseconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            int cycle = CycleCount;

            while (cycle-- >= 0)
            {
                var opcode = CPU.StepCycle();

                if (opcode.Byte == 0xDB) //IN
                    In();
                else if (opcode.Byte == 0xD3) //OUT
                    Out();
            }

            if (totalMilliseconds >= delay)
            {
                if (State.EnableInterrupts)
                {
                    lastInterruptTime = gameTime.TotalGameTime;
                    lastInterrupt = lastInterrupt == 1 ? 2 : 1;
                    GenerateInterrupt(lastInterrupt);
                }
            }

            UpdateButtons();
            UpdateSounds();

            base.Update(gameTime);
        }

        protected virtual void UpdateButtons()
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
            if (!(this is SpaceInvadersGame))        
                GraphicsDevice.Clear (Color.Black);

            spriteBatch.Begin ();
            Color color;
			
            for (int i = 0; i < system.PPU.Width; i++) 
			{
				for (int j = 0; j < system.PPU.Height; j++)
				{
                    Pixel pixel = system.PPU[i, j];
                    
                    Vector2 coor = new Vector2(j * tamanhoPixel, (system.PPU.Width * tamanhoPixel) - (i * tamanhoPixel));
                        
                    if (coor.Y < (50 * tamanhoPixel))
                        color = Color.FromNonPremultiplied(255 & pixel.R, 0, 0, alpha & pixel.A);
                    else if (coor.Y > (180 * tamanhoPixel))
                        color = Color.FromNonPremultiplied(0, 255 & pixel.G, 0, alpha & pixel.A);
                    else
                        color = Color.FromNonPremultiplied(255 & pixel.R, 255 & pixel.G, 255 & pixel.B, alpha & pixel.A);
                    
                    spriteBatch.Draw (whiteRect, coor, color);
				}
			}

			spriteBatch.End ();

            base.Draw (gameTime);	
		}

        protected virtual void GenerateInterrupt(int interruptNumber)
        {
            CPU.GenerateInterrupt(interruptNumber);
        }

        protected virtual void In()
        {
            byte port = CPU.GetNextByte();

            switch (port)
			{
                case 1:
                    State.A = State.Ports.Read1;
                    break;
                case 2:
                    State.A = State.Ports.Read2;
                    break;
                case 3:
                    ushort value = BitUtils.GetWordFrom2Bytes(State.Ports.Shift0, State.Ports.Shift1);
                    State.A = (byte)((value >> (8 - State.Ports.Write2)) & 0xFF);
                    break;
                default:
                    break;
			}
        }

        protected virtual void Out()
        {
            byte port = CPU.GetNextByte();

            switch (port)
			{
                case 2:
                    State.Ports.Write2 = (byte)(State.A & 0x7);
                    break;
                case 3:
                    State.Ports.Write3 = State.A;
                    break;
                case 4:
                    State.Ports.Shift0 = State.Ports.Shift1;
                    State.Ports.Shift1 = State.A;
                    break;
                case 5:
                    State.Ports.Write5 = State.A;
                    break;
                default:
                    break;
			}
        }

        protected virtual void UpdateSounds()
        {            
            byte write3 = State.Ports.Write3;
            byte write5 = State.Ports.Write5;            
            
            lastWrite3 = write3;
            lastWrite5 = write5;
        }
    }
}