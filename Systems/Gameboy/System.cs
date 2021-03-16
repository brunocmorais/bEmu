using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Systems.Gameboy.Sound;
using bEmu.Systems.Gameboy.GPU.Palettes;
using bEmu.Systems.Gameboy.MBCs;
using System.Diagnostics;

namespace bEmu.Systems.Gameboy
{
    public class System : Core.System
    {
        public bool GBCMode { get; set; }
        public IColorPalette ColorPalette { get; set; }
        public bool DoubleSpeedMode => (MMU[0xFF4D] & 0x80) == 0x80;
        public override int Width => 160;
        public override int Height => 144;
        public override int RefreshRate => 16;
        public override int CycleCount => 69905;

        public System(string fileName) : base(fileName)
        {
        }

        public override IState GetInitialState()
        {
            var state = new bEmu.Systems.Gameboy.State(this);
            state.Flags = new Flags();

            state.EnableInterrupts = false;
            state.Cycles = 0;
            state.Halted = false;
            state.Instructions = 0;
            state.PC = 0x0000;

            return state;
        }

        public override void Initialize()
        {
            base.Initialize();
            MMU = new MMU(this);
            PPU = new GPU.GPU(this);
            APU = new bEmu.Systems.Gameboy.Sound.APU(this);
            Runner = new CPU(this);
            ColorPalette = ColorPaletteFactory.Get(MonochromePaletteType.Gray);
        }

        public override void Reset()
        {
            base.Reset();
            GBCMode = false;
        }

        public override void Update()
        {
            MMU mmu = (MMU) MMU;
            State state = (State) State;
            GPU.GPU gpu = (GPU.GPU) PPU;

            while (Cycles >= 0)
            {
                if (mmu.Bios.Running && state.PC == 0x100)
                {
                    mmu.Bios.Running = false;

                    if ((mmu.CartridgeHeader.GBCFlag & 0x80) == 0x80) // set gameboy color mode
                    {
                        state.A = 0x11;
                        GBCMode = true;
                    }
                }

                var opcode = Runner.StepCycle();

                int cyclesTaken = opcode.CyclesTaken;

                if (DoubleSpeedMode)
                    cyclesTaken /= 2;

                state.Timer.UpdateTimers(cyclesTaken);

                if (mmu.MBC is IHasRTC)
                    (mmu.MBC as IHasRTC).Tick(cyclesTaken);
                
                gpu.Cycles += cyclesTaken;
                gpu.StepCycle();

                Cycles -= cyclesTaken;
            }
        }

        public override void Stop()
        {
            ((MMU) MMU).MBC.Shutdown();
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            var joypad = ((Systems.Gameboy.MMU) MMU).Joypad;

            if (gamePad.IsKeyDown(GamePadKey.Z))
                joypad.Column1 &= 0xE;
            if (gamePad.IsKeyDown(GamePadKey.X))
                joypad.Column1 &= 0xD;
            if (gamePad.IsKeyDown(GamePadKey.RightShift))
                joypad.Column1 &= 0xB;
            if (gamePad.IsKeyDown(GamePadKey.Enter))
                joypad.Column1 &= 0x7;
            if (gamePad.IsKeyDown(GamePadKey.Right))
                joypad.Column2 &= 0xE;
            if (gamePad.IsKeyDown(GamePadKey.Left))
                joypad.Column2 &= 0xD;
            if (gamePad.IsKeyDown(GamePadKey.Up))
                joypad.Column2 &= 0xB;
            if (gamePad.IsKeyDown(GamePadKey.Down))
                joypad.Column2 &= 0x7;

            if (gamePad.IsKeyUp(GamePadKey.Z))
                joypad.Column1 |= 0x1;
            if (gamePad.IsKeyUp(GamePadKey.X))
                joypad.Column1 |= 0x2;
            if (gamePad.IsKeyUp(GamePadKey.RightShift))
                joypad.Column1 |= 0x4;
            if (gamePad.IsKeyUp(GamePadKey.Enter))
                joypad.Column1 |= 0x8;
            if (gamePad.IsKeyUp(GamePadKey.Right))
                joypad.Column2 |= 0x1;
            if (gamePad.IsKeyUp(GamePadKey.Left))
                joypad.Column2 |= 0x2;
            if (gamePad.IsKeyUp(GamePadKey.Up))
                joypad.Column2 |= 0x4;
            if (gamePad.IsKeyUp(GamePadKey.Down))
                joypad.Column2 |= 0x8;

            if (joypad.Column1 != 0xF || joypad.Column2 != 0xF)
                ((State)State).RequestInterrupt(InterruptType.Joypad);
        }
    }
}