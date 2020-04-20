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
using System.Linq;
using System.Threading;
using bEmu.Core.Systems.Gameboy.MBCs;

namespace bEmu
{
    public class GameboyGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Core.Systems.Gameboy.System system;
        const int tamanhoPixel = 2;
        const int width = 160;
        const int height = 144;
        const int CycleCount = 70224;
        string rom;
        bEmu.Core.CPUs.LR35902.Disassembler disassembler;
        bool running = false;
        Keys[] frameskipKeys = new Keys[]
        {
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
        };
        int lastRenderedFrame;
        Thread thread;
        bool debug;
        SpriteFont font;
        Texture2D backBuffer;
        Rectangle destinationRectangle;
        bool showFPS = false;
        int drawCounter = 0;

        public GameboyGame(string rom)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width * tamanhoPixel;
            graphics.PreferredBackBufferHeight = height * tamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.rom = rom;
        }

        bEmu.Core.Systems.Gameboy.State State => system.State as bEmu.Core.Systems.Gameboy.State;
        bEmu.Core.Systems.Gameboy.MMU Mmu => (system.MMU as bEmu.Core.Systems.Gameboy.MMU);
        GPU Gpu => system.PPU as GPU;

        protected override void Initialize()
        {
            system = new Core.Systems.Gameboy.System();
            Mmu.LoadProgram(rom);
            disassembler = new Core.CPUs.LR35902.Disassembler(system);
            this.Window.Title = $"bEmu - {Mmu.CartridgeHeader.Title}";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Common/Font");
            backBuffer = new Texture2D(GraphicsDevice, 160, 144);
            destinationRectangle = new Rectangle(0, 0, width * tamanhoPixel, height * tamanhoPixel);
            Gpu.Frameskip = 1;

            running = true;

            thread = new Thread(() => 
            {
                while (running)
                {
                    int lastCycleCount = UpdateGame();

                    lock (this)
                    {
                        if (Gpu.Frame <= drawCounter)
                        {
                            Gpu.Cycles += lastCycleCount;
                            Gpu.StepCycle();
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
                Mmu.MBC.Shutdown();
            }

            if (keyboardState.IsKeyDown(Keys.F1))
            {
                running = false;
                Initialize();
            }

            if (keyboardState.IsKeyDown(Keys.F2))
                showFPS = !showFPS;

            if (frameskipKeys.Any(x => keyboardState.IsKeyDown(x)))
                Gpu.Frameskip = (int) (frameskipKeys.First(x => keyboardState.IsKeyDown(x))) - 48;

            UpdateKeys(keyboardState);
        }

        protected override void Draw (GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (Gpu.Frame > lastRenderedFrame)
                backBuffer.SetData(Gpu.FrameBuffer);

            lastRenderedFrame = Gpu.Frame;
            spriteBatch.Begin();
            spriteBatch.Draw(backBuffer, destinationRectangle, Color.White);

            if (showFPS)
            {
                double fps = Math.Round(Gpu.Frame / gameTime.TotalGameTime.TotalSeconds, 1);
                spriteBatch.DrawString(font, $"{Gpu.Frame}@{fps} fps\ninst={State.Instructions}", new Vector2(0, 0), Color.Red);
            }
            
            spriteBatch.End();
            drawCounter++;
        }

        private int UpdateGame()
        {
            if (Mmu.Bios.Running && State.PC >= 0x100)
                Mmu.Bios.Running = false;

            int prevCycles = State.Cycles;
            var opcode = system.Runner.StepCycle();
            int afterCycles = State.Cycles;

            int lastCycleCount = (afterCycles - prevCycles);
            State.Timer.UpdateTimers(lastCycleCount);

            if (Mmu.MBC is IHasRTC)
                (Mmu.MBC as IHasRTC).Tick(lastCycleCount);

            return lastCycleCount;
        }

        public void UpdateKeys(KeyboardState keyboardState)
        {
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

        protected override void UnloadContent()
        {
            running = false;
        }
    }
}