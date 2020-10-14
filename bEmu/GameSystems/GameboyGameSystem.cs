using bEmu.Classes;
using bEmu.Components;
using bEmu.Core;
using bEmu.Systems;
using bEmu.Systems.Factory;
using bEmu.Systems.Gameboy;
using bEmu.Systems.Gameboy.MBCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.GameSystems
{

    public class GameboyGameSystem : IGameSystem
    {
        public SupportedSystems Type => SupportedSystems.GameBoy;
        public int Width => 160;
        public int Height => 144;
        public IMainGame MainGame { get; }
        public int Frame { get => gpu.Frame; set => gpu.Frame = value; }
        public int Frameskip { get => gpu.Frameskip; set => gpu.Frameskip = value; }
        public Framebuffer Framebuffer { get => gpu.Framebuffer; }
        public int RefreshRate => 16;
        public ISystem System { get; }
        private readonly Systems.Gameboy.GPU.GPU gpu;
        private readonly Systems.Gameboy.State state;
        private readonly Systems.Gameboy.MMU mmu;

        public GameboyGameSystem(IMainGame mainGame, string rom)
        {
            System = SystemFactory.Get(SupportedSystems.GameBoy, rom) as Systems.Gameboy.System;
            gpu = System.PPU as Systems.Gameboy.GPU.GPU;
            state = System.State as Systems.Gameboy.State;
            mmu = System.MMU as Systems.Gameboy.MMU;
            MainGame = mainGame;
            MainGame.Options = new GameboyOptions(mainGame, MainGame.Options);
            MainGame.Options.Size = 2;
        }

        public void Initialize() 
        { 
            mmu.LoadProgram();
            state.PC = 0x00;
        }
        public void LoadContent() { }
        
        public void Update(GameTime gameTime) { }
        
        public void UpdateGame() 
        { 
            if (mmu.Bios.Running && state.PC == 0x100)
            {
                mmu.Bios.Running = false;

                if ((mmu.CartridgeHeader.GBCFlag & 0x80) == 0x80) // set gameboy color mode
                {
                    state.A = 0x11;
                    (System as Systems.Gameboy.System).GBCMode = true;
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
                if (gpu.Frame <= MainGame.DrawCounter)
                {
                    gpu.Cycles += lastCycleCount;

                    if (state.Instructions % 2 == 0)
                        gpu.StepCycle();
                }
            }
        }
        
        public void UpdateGamePad(KeyboardState keyboardState) 
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

        public void Draw(GameTime gameTime) { }

        public void StopGame()
        {
            mmu.MBC.Shutdown();
        }
    }
}