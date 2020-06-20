using Microsoft.Xna.Framework.Input;
using bEmu.Core.CPUs.LR35902;
using bEmu.Core.Systems.Gameboy.MBCs;
using bEmu.Core.Systems.Gameboy;
using State = bEmu.Core.Systems.Gameboy.State;
using APU = bEmu.Core.Systems.Gameboy.Sound.APU;
using GPU = bEmu.Core.Systems.Gameboy.GPU.GPU;
using Gameboy = bEmu.Core.Systems.Gameboy;

namespace bEmu
{
    public class GameboyGame : BaseGame<Gameboy.System, State, MMU, GPU, APU>
    {
        // private readonly DynamicSoundEffectInstance instance;
        // private readonly byte[] instanceBuffer;

        public GameboyGame(string rom) : base(new Gameboy.System(), rom, 160, 144, 2)
        {
            // instance = new DynamicSoundEffectInstance(APU.AudioSampleRate, AudioChannels.Stereo);
            // instanceBuffer = new byte[2 * APU.AudioBufferFrames * APU.BytesPerSample];
        }

        protected override void Initialize()
        {
            Mmu.LoadProgram(Rom);
            State.PC = 0x00;
            Window.Title = $"bEmu - {Mmu.CartridgeHeader.Title}";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            IsRunning = true;
            Gpu.Frameskip = 1;
            StartMainThread();
        }

        public override void UpdateGame()
        {
            if (Mmu.Bios.Running && State.PC >= 0x100)
                Mmu.Bios.Running = false;

            int prevCycles = State.Cycles;
            var opcode = System.Runner.StepCycle();
            int afterCycles = State.Cycles;

            int lastCycleCount = (afterCycles - prevCycles);
            State.Timer.UpdateTimers(lastCycleCount);

            if (Mmu.MBC is IHasRTC)
                (Mmu.MBC as IHasRTC).Tick(lastCycleCount);
            
            lock (this)
            {
                if (Gpu.Frame <= DrawCounter)
                {
                    Gpu.Cycles += lastCycleCount;

                    if (State.Instructions % 2 == 0)
                        Gpu.StepCycle();
                }
            }
        }

        public override void StopGame()
        {
            base.StopGame();
            Mmu.MBC.Shutdown();
        }

        public override void UpdateGamePad(KeyboardState keyboardState)
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
    }
}