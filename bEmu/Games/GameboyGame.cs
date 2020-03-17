using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core;
using System.Diagnostics;
using System;
using bEmu.Core.Systems.Gameboy;
using State = bEmu.Core.Systems.Gameboy.State;
using bEmu.Core.Util;

namespace bEmu
{
    public class GameboyGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        Core.Systems.Gameboy.System system;
        const int tamanhoPixel = 2;
        const int width = 160 * tamanhoPixel;
        const int height = 144 * tamanhoPixel;
        private const int CycleCount = 70224;
        string rom;
        bEmu.Core.CPUs.LR35902.Disassembler disassembler;
        
        public GameboyGame(string rom)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.rom = rom;
        }

        bEmu.Core.Systems.Gameboy.State State => system.State as bEmu.Core.Systems.Gameboy.State;
        GPU Gpu => system.PPU as GPU;

        protected override void Initialize()
        {
            system = new Core.Systems.Gameboy.System();
            system.MMU.LoadProgram(rom);
            disassembler = new Core.CPUs.LR35902.Disassembler(system);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);

            const int pixels = tamanhoPixel * tamanhoPixel;

            var whiteColor = new Microsoft.Xna.Framework.Color[pixels];
            
            for (int i = 0; i < pixels; i++) 
                whiteColor[i] = Microsoft.Xna.Framework.Color.White;
            
            pixel.SetData(whiteColor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || State.Halted)
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                Initialize();

            UpdateKeys();

            int cycle = CycleCount;

            while (cycle >= 0)
            {
                int prevCycles = State.Cycles;
                system.Runner.StepCycle();
                int afterCycles = State.Cycles;

                int cyclesLastOperation = afterCycles - prevCycles;
                cycle -= cyclesLastOperation;
                Gpu.StepCycle(cyclesLastOperation);
            }

            base.Update(gameTime);
        }

        public void UpdateKeys()
        {
            if (State.Joypad == 16)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    State.Joypad &= 0b11110;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    State.Joypad &= 0b11101;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    State.Joypad &= 0b11011;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    State.Joypad &= 0b10111;

                if (Keyboard.GetState().IsKeyUp(Keys.Right))
                    State.Joypad |= 0b10001;
                if (Keyboard.GetState().IsKeyUp(Keys.Left))
                    State.Joypad |= 0b10010;
                if (Keyboard.GetState().IsKeyUp(Keys.Up))
                    State.Joypad |= 0b10100;
                if (Keyboard.GetState().IsKeyUp(Keys.Down))
                    State.Joypad |= 0b11000;
            }



            // if (Keyboard.GetState().IsKeyDown(Keys.Z))
            //     State.Joypad &= 0xE;
            // if (Keyboard.GetState().IsKeyDown(Keys.X))
            //     State.Joypad &= 0xE;
            // if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //     State.Joypad &= 0xE;
            // if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            //     State.Joypad &= 0xE;
        }

        protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (Microsoft.Xna.Framework.Color.White);
            spriteBatch.Begin ();

			for (int i = 0; i < system.PPU.Width; i++) 
				for (int j = 0; j < system.PPU.Height; j++)
                {
                    Pixel pixel = system.PPU[i, j];
                    spriteBatch.Draw(this.pixel, new Vector2(i * tamanhoPixel, j * tamanhoPixel),
                        Microsoft.Xna.Framework.Color.FromNonPremultiplied(pixel.R, pixel.G, pixel.B, pixel.A));
                }

            spriteBatch.End ();

            base.Draw (gameTime);
		}
    }
}