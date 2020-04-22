using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using bEmu.Core;
using System.Diagnostics;
using System;
using bEmu.Core.Systems.Gameboy;
using bEmu.Core.Util;
using bEmu.Core.CPUs.LR35902;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using bEmu.Core.Systems.Gameboy.MBCs;

namespace bEmu
{
    public class GameboyGame : Game
    {
        private const int TamanhoPixel = 2;
        private const int Width = 160;
        private const int Height = 144;
        private const int CycleCount = 70224;
        private readonly Core.Systems.Gameboy.System system;
        private readonly Core.Systems.Gameboy.State state;
        private readonly Core.Systems.Gameboy.MMU mmu;
        private readonly GPU gpu;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private string rom;
        private bool running;
        private Keys[] frameskipKeys;
        private int lastRenderedFrame;
        private Thread thread;
        private SpriteFont font;
        private Texture2D backBuffer;
        private Rectangle destinationRectangle;
        private bool showFPS;
        private int drawCounter;

        public GameboyGame(string rom)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width * TamanhoPixel;
            graphics.PreferredBackBufferHeight = Height * TamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.rom = rom;
            running = false;
            frameskipKeys = new Keys[]
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
            };
            showFPS = false;
            drawCounter = 0;
            system = new Core.Systems.Gameboy.System();
            state = system.State as bEmu.Core.Systems.Gameboy.State;
            mmu = system.MMU as bEmu.Core.Systems.Gameboy.MMU;
            gpu = system.PPU as GPU;
        }

        protected override void Initialize()
        {
            mmu.LoadProgram(rom);
            Window.Title = $"bEmu - {mmu.CartridgeHeader.Title}";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Common/Font");
            backBuffer = new Texture2D(GraphicsDevice, 160, 144);
            destinationRectangle = new Rectangle(0, 0, Width * TamanhoPixel, Height * TamanhoPixel);
            gpu.Frameskip = 1;

            running = true;

            thread = new Thread(() => 
            {
                while (running)
                {
                    int lastCycleCount = UpdateGame();

                    lock (this)
                    {
                        if (gpu.Frame <= drawCounter)
                        {
                            gpu.Cycles += lastCycleCount;
                            gpu.StepCycle();
                        }
                    }
                }
            });

            thread.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
                mmu.MBC.Shutdown();
            }

            if (keyboardState.IsKeyDown(Keys.F1))
            {
                running = false;
                Initialize();
            }

            if (keyboardState.IsKeyDown(Keys.F2))
                showFPS = !showFPS;

            if (frameskipKeys.Any(x => keyboardState.IsKeyDown(x)))
                gpu.Frameskip = (int) (frameskipKeys.First(x => keyboardState.IsKeyDown(x))) - 48;

            UpdateKeys(keyboardState);
        }

        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (gpu.Frame > lastRenderedFrame)
                backBuffer.SetData(gpu.FrameBuffer);

            lastRenderedFrame = gpu.Frame;
            spriteBatch.Begin();
            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

            if (showFPS)
            {
                double fps = Math.Round(gpu.Frame / gameTime.TotalGameTime.TotalSeconds, 1);
                spriteBatch.DrawString(font, $"{gpu.Frame}@{fps} fps\ninst={state.Instructions}", new Vector2(0, 0), Color.Red);
            }
            
            spriteBatch.End();
            drawCounter++;
        }

        private int UpdateGame()
        {
            if (mmu.Bios.Running && state.PC >= 0x100)
                mmu.Bios.Running = false;

            int prevCycles = state.Cycles;
            var opcode = system.Runner.StepCycle();
            int afterCycles = state.Cycles;

            int lastCycleCount = (afterCycles - prevCycles);
            state.Timer.UpdateTimers(lastCycleCount);

            if (mmu.MBC is IHasRTC)
                (mmu.MBC as IHasRTC).Tick(lastCycleCount);

            return lastCycleCount;
        }

        public void UpdateKeys(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Z))
                state.Joypad.Column1 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.X))
                state.Joypad.Column1 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.RightShift))
                state.Joypad.Column1 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Enter))
                state.Joypad.Column1 &= 0x7;
            if (keyboardState.IsKeyDown(Keys.Right))
                state.Joypad.Column2 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.Left))
                state.Joypad.Column2 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.Up))
                state.Joypad.Column2 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Down))
                state.Joypad.Column2 &= 0x7;

            if (keyboardState.IsKeyUp(Keys.Z))
                state.Joypad.Column1 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.X))
                state.Joypad.Column1 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.RightShift))
                state.Joypad.Column1 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Enter))
                state.Joypad.Column1 |= 0x8;
            if (keyboardState.IsKeyUp(Keys.Right))
                state.Joypad.Column2 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.Left))
                state.Joypad.Column2 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.Up))
                state.Joypad.Column2 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Down))
                state.Joypad.Column2 |= 0x8;

            if (state.Joypad.Column1 != 0xF || state.Joypad.Column2 != 0xF)
                state.RequestInterrupt(InterruptType.Joypad);
        }

        protected override void UnloadContent()
        {
            running = false;
        }
    }
}