using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Chip8
{
    public class Chip8Game : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        CPU cpu;
        const int tamanhoPixel = 10;
        const int width = 64 * tamanhoPixel;
        const int height = 32 * tamanhoPixel;
        
        public Chip8Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            cpu = new CPU();
            cpu.State.LoadProgram(File.ReadAllBytes("Chip8/roms/test_opcode.ch8"));

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (cpu.State.Delay > 0)
                cpu.State.Delay--;
            
            if (cpu.State.Sound > 0)
                cpu.State.Sound--;

            int cycle = 15;

            while (cycle-- >= 0)
                cpu.StepCycle();

            UpdateKeys(Keyboard.GetState());

            base.Update(gameTime);
        }

        private void UpdateKeys(KeyboardState keyboardState)
        {
            var keys = new Keys[] 
            {
                Keys.D1, Keys.D2, Keys.D3, Keys.D4,
                Keys.Q, Keys.W, Keys.E, Keys.R,
                Keys.A, Keys.S, Keys.D, Keys.F,
                Keys.Z, Keys.X, Keys.C, Keys.V
            };

            var keyboard = new int[] {
                0x1, 0x2, 0x3, 0xC,
                0x4, 0x5, 0x6, 0xD,
                0x7, 0x8, 0x9, 0xE,
                0xA, 0x0, 0xB, 0xF
            };

            for (int i = 0; i < keys.Length; i++)
                cpu.State.Keys[keyboard[i]] = keyboardState.IsKeyDown(keys[i]);
        }

        protected override void Draw (GameTime gameTime)
		{
            base.Draw (gameTime);	

            if (!cpu.State.Draw)
                return;

			GraphicsDevice.Clear (Color.Black);
            spriteBatch.Begin ();
            
			for (int i = 0; i < 64; i++) 
				for (int j = 0; j < 32; j++)
                    if (cpu.State.Gfx[i, j]) 
                        spriteBatch.Draw (whiteRect, new Vector2(i * tamanhoPixel, j * tamanhoPixel), Color.White);

			spriteBatch.End ();
            cpu.State.Draw = false;
		}
    }
}