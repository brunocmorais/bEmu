using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Intel8080
{
    public class Generic8080Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        CPU cpu;
        TimeSpan lastInterruptTime = TimeSpan.Zero;
        int lastInterrupt = 1;
        const int tamanhoPixel = 2;
        byte lastWrite3;
        byte lastWrite5;
        string[] fileNames;
        string[] memoryPositions;
        string zipName;
        const int width = 224 * tamanhoPixel;
        const int height = 256 * tamanhoPixel;
        
        public Generic8080Game(string zipName, string[] fileNames, string[] memoryPositions)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 8);
            this.fileNames = fileNames;
            this.memoryPositions = memoryPositions; 
            this.zipName = zipName;
        }

        protected override void Initialize()
        {
            var entries = new Dictionary<string, byte[]>();
            
            using (var zipFile = ZipFile.OpenRead($"Content/{zipName}.zip"))
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

            cpu = new CPU();

            for (int i = 0; i < fileNames.Length; i++)
                cpu.State.LoadProgram(entries[fileNames[i]], Convert.ToInt32(memoryPositions[i], 16));

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

            cpu.UpdatePorts(1, 0x01);
            cpu.UpdatePorts(2, 0x00);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateButtons();
            UpdateSounds();

            if ((gameTime.TotalGameTime - lastInterruptTime).TotalMilliseconds >= 8)
            {
                if (cpu.State.EnableInterrupts)
                {
                    lastInterruptTime = gameTime.TotalGameTime;
                    lastInterrupt = lastInterrupt == 1 ? 2 : 1;
                    GenerateInterrupt(lastInterrupt);
                }
            }

            int cycle = 1000;

            while (cycle-- >= 0)
            {
                byte opcode = cpu.EmularCiclo();

                if (opcode == 0xDB) //IN
                    In();
                else if (opcode == 0xD3) //OUT
                    Out();
            }

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

						if ((sprite & (1 << pixel)) > 0) 
                            spriteBatch.Draw (whiteRect, coor, Color.White);
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
            
            lastWrite3 = write3;
            lastWrite5 = write5;
        }
    }
}