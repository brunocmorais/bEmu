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
        protected const int Width = 224;
        protected const int Height = 256;
        protected const int Delay = 8;
        protected const int CycleCount = 3000;
        protected int Alpha = 255;
        protected const int TamanhoPixel = 2;
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected Texture2D whiteRect;
        protected Texture2D backBuffer;
        protected Core.Systems.Generic8080.System system;
        protected TimeSpan lastInterruptTime;
        protected int lastInterrupt;
        protected byte lastWrite3;
        protected byte lastWrite5;
        protected string[] fileNames;
        protected string[] memoryPositions;
        protected string zipName;
        protected Rectangle destinationRectangle;
        protected State State => system.State as State;
        protected Core.Systems.Generic8080.PPU PPU => system.PPU as Core.Systems.Generic8080.PPU;
        protected Intel8080<State, MMU> CPU => system.Runner as Intel8080<State, MMU>;

        public Generic8080Game(string zipName, string[] fileNames, string[] memoryPositions)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width * TamanhoPixel;
            graphics.PreferredBackBufferHeight = Height * TamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, Delay);
            this.fileNames = fileNames;
            this.memoryPositions = memoryPositions; 
            this.zipName = zipName;
            lastInterruptTime = TimeSpan.Zero;
            lastInterrupt = 1;
            destinationRectangle = new Rectangle(0, 0, Width * TamanhoPixel, Height * TamanhoPixel);
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
            whiteRect = new Texture2D(GraphicsDevice, TamanhoPixel, TamanhoPixel);
            backBuffer = new Texture2D(GraphicsDevice, Width, Height);

            Color[] whiteColor = new Color[TamanhoPixel * TamanhoPixel];
            
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

            if (totalMilliseconds >= Delay)
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
            PPU.UpdateFrameBuffer();
            backBuffer.SetData(PPU.FrameBuffer);
            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);
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