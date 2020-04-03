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
using bEmu.Core.CPUs.LR35902;

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
        const int CycleCount = 70224;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                Initialize();

            int cycle = CycleCount;

            UpdateKeys();

            while (cycle >= 0)
            {
                // char? debug = (system.MMU as bEmu.Core.Systems.Gameboy.MMU).Debug;
                // Debug.WriteIf(debug.HasValue, debug);

                int prevCycles = State.Cycles;
                system.Runner.StepCycle();
                int afterCycles = State.Cycles;

                int cyclesLastOperation = afterCycles - prevCycles;
                cycle -= cyclesLastOperation;
                Gpu.StepCycle(cyclesLastOperation / 2);
            }

            //base.Update(gameTime);
        }

        public void UpdateKeys()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Right))
                State.Joypad.Column2 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.Left))
                State.Joypad.Column2 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.Up))
                State.Joypad.Column2 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Down))
                State.Joypad.Column2 &= 0x7;
            if (keyboardState.IsKeyDown(Keys.Z))
                State.Joypad.Column1 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.X))
                State.Joypad.Column1 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.RightShift))
                State.Joypad.Column1 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Enter))
                State.Joypad.Column1 &= 0x7;

            if (keyboardState.IsKeyUp(Keys.Right))
                State.Joypad.Column2 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.Left))
                State.Joypad.Column2 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.Up))
                State.Joypad.Column2 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Down))
                State.Joypad.Column2 |= 0x8;
            if (keyboardState.IsKeyUp(Keys.Z))
                State.Joypad.Column1 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.X))
                State.Joypad.Column1 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.RightShift))
                State.Joypad.Column1 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Enter))
                State.Joypad.Column1 |= 0x8;

            if (State.Joypad.Column1 != 0xF || State.Joypad.Column2 != 0xF)
                State.RequestInterrupt(InterruptType.Joypad);
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