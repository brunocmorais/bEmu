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
using System.Threading.Tasks;

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
        const int CycleCount = 70224 / 2;
        string rom;
        bEmu.Core.CPUs.LR35902.Disassembler disassembler;
        private bool debug = false;

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
            this.Window.Title = $"bEmu - {(system.MMU as Core.Systems.Gameboy.MMU).Title}";

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

            UpdateKeys();

            State.Cycles = 0;
            State.Instructions = 0;
        }

        private void UpdateTimers(int lastCycleCount)
        {
            State.Timer.DIV += (byte) (lastCycleCount / 4);

            if (State.Timer.Enabled)
            {
                if (State.Timer.TIMA + State.Timer.Step > 0xFF)
                {
                    State.Timer.TIMA = State.Timer.TMA;
                    State.RequestInterrupt(InterruptType.Timer);
                }

                State.Timer.TIMA += State.Timer.Step;
            }
        }

        public void UpdateKeys()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Z))
                State.Joypad.Column1 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.X))
                State.Joypad.Column1 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.RightShift))
                State.Joypad.Column1 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Enter))
                State.Joypad.Column1 &= 0x7;
            if (keyboardState.IsKeyDown(Keys.Right))
                State.Joypad.Column2 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.Left))
                State.Joypad.Column2 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.Up))
                State.Joypad.Column2 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Down))
                State.Joypad.Column2 &= 0x7;


            if (keyboardState.IsKeyUp(Keys.Z))
                State.Joypad.Column1 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.X))
                State.Joypad.Column1 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.RightShift))
                State.Joypad.Column1 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Enter))
                State.Joypad.Column1 |= 0x8;
            if (keyboardState.IsKeyUp(Keys.Right))
                State.Joypad.Column2 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.Left))
                State.Joypad.Column2 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.Up))
                State.Joypad.Column2 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Down))
                State.Joypad.Column2 |= 0x8;

            if (State.Joypad.Column1 != 0xF || State.Joypad.Column2 != 0xF)
                State.RequestInterrupt(InterruptType.Joypad);
        }

        protected override void Draw (GameTime gameTime)
		{
            while (State.Cycles < CycleCount)
            {
                // char? debug = (system.MMU as bEmu.Core.Systems.Gameboy.MMU).Debug;
                // Debug.WriteIf(debug.HasValue, debug);

                // if (State.PC == 0xc45c)
                // {
                //     debug = true;
                //     Debugger.Break();
                // }

                if (debug)
                    Debug.WriteLine(State);

                int prevCycles = State.Cycles;
                var opcode = system.Runner.StepCycle();
                int afterCycles = State.Cycles;

                int lastCycleCount = (afterCycles - prevCycles);
                Gpu.StepCycle(lastCycleCount * 2);
                UpdateTimers(lastCycleCount);
            }

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
		}
    }
}