using Microsoft.Xna.Framework.Input;
using bEmu.Core.CPUs.LR35902;
using bEmu.Systems.Gameboy.MBCs;
using bEmu.Systems.Gameboy;
using State = bEmu.Systems.Gameboy.State;
using APU = bEmu.Systems.Gameboy.Sound.APU;
using GPU = bEmu.Systems.Gameboy.GPU.GPU;
using Gameboy = bEmu.Systems.Gameboy;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using bEmu.Systems.Gameboy.GPU;
using bEmu.Factory;
using bEmu.Systems;
using bEmu.Classes;
using bEmu.Components;
using bEmu.Scalers;

namespace bEmu
{
    public class GameboyGame : BaseGame
    {
        private readonly Gameboy.System system;
        private readonly GPU gpu;
        private readonly State state;
        private readonly MMU mmu;
        private Timer timer = new Timer();

        public GameboyGame(string rom) : base(SystemFactory.Get(SupportedSystems.GameBoy, rom), 160, 144, 2)
        {
            system = System as Gameboy.System;
            gpu = Gpu as GPU;
            state = State as State;
            mmu = Mmu as MMU;
            Options = new GameboyOptions(Options);
            Options.OptionChanged += OnOptionChanged;
        }

        protected override void Initialize()
        {
            mmu.LoadProgram();
            state.PC = 0x00;
            Window.Title = $"bEmu - {mmu.CartridgeHeader.Title}";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            IsRunning = true;
            StartMainThread();
        }

        public override void UpdateGame()
        {
            if (mmu.Bios.Running && state.PC == 0x100)
            {
                mmu.Bios.Running = false;

                if ((mmu.CartridgeHeader.GBCFlag & 0x80) == 0x80) // set gameboy color mode
                {
                    state.A = 0x11;
                    system.GBCMode = true;
                }
            }

            int prevCycles = state.Cycles;
            var opcode = System.Runner.StepCycle();
            int afterCycles = state.Cycles;

            int lastCycleCount = (afterCycles - prevCycles);
            state.Timer.UpdateTimers(lastCycleCount);

            if (mmu.MBC is IHasRTC)
                (mmu.MBC as IHasRTC).Tick(lastCycleCount);
            
            lock (this)
            {
                if (gpu.Frame <= DrawCounter)
                {
                    gpu.Cycles += lastCycleCount;

                    if (state.Instructions % 2 == 0)
                        gpu.StepCycle();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            SpriteBatch.End();
        }

        public override void StopGame()
        {
            base.StopGame();
            mmu.MBC.Shutdown();
        }

        public override void UpdateGamePad(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Z))
                mmu.Joypad.Column1 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.X))
                mmu.Joypad.Column1 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.RightShift))
                mmu.Joypad.Column1 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Enter))
                mmu.Joypad.Column1 &= 0x7;
            if (keyboardState.IsKeyDown(Keys.Right))
                mmu.Joypad.Column2 &= 0xE;
            if (keyboardState.IsKeyDown(Keys.Left))
                mmu.Joypad.Column2 &= 0xD;
            if (keyboardState.IsKeyDown(Keys.Up))
                mmu.Joypad.Column2 &= 0xB;
            if (keyboardState.IsKeyDown(Keys.Down))
                mmu.Joypad.Column2 &= 0x7;

            if (keyboardState.IsKeyUp(Keys.Z))
                mmu.Joypad.Column1 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.X))
                mmu.Joypad.Column1 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.RightShift))
                mmu.Joypad.Column1 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Enter))
                mmu.Joypad.Column1 |= 0x8;
            if (keyboardState.IsKeyUp(Keys.Right))
                mmu.Joypad.Column2 |= 0x1;
            if (keyboardState.IsKeyUp(Keys.Left))
                mmu.Joypad.Column2 |= 0x2;
            if (keyboardState.IsKeyUp(Keys.Up))
                mmu.Joypad.Column2 |= 0x4;
            if (keyboardState.IsKeyUp(Keys.Down))
                mmu.Joypad.Column2 |= 0x8;

            if (mmu.Joypad.Column1 != 0xF || mmu.Joypad.Column2 != 0xF)
                state.RequestInterrupt(InterruptType.Joypad);
        }

        protected override void OnOptionChanged(object sender, OnOptionChangedEventArgs e)
        {
            base.OnOptionChanged(sender, e);

            switch (e.Property)
            {
                case "PaletteType":
                    gpu.SetShadeColorPalette((Options as GameboyOptions).PaletteType);
                    break;
            }
        }

        public override void ResetGame()
        {
            base.ResetGame();
            IsRunning = false;
            System.Reset();
            mmu.Bios.Running = true;
            IsRunning = true;
            StartMainThread();
        }
    }
}