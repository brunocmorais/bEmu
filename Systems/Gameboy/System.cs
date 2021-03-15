using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Systems.Gameboy.Sound;
using bEmu.Systems.Gameboy.GPU.Palettes;
using bEmu.Systems.Gameboy.MBCs;

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

                state.Timer.UpdateTimers(opcode.CyclesTaken);

                if (mmu.MBC is IHasRTC)
                    (mmu.MBC as IHasRTC).Tick(opcode.CyclesTaken);
                
                gpu.Cycles += opcode.CyclesTaken;
                gpu.StepCycle();

                Cycles -= opcode.CyclesTaken;
            }
        }

        public override void Stop()
        {
            ((MMU) MMU).MBC.Shutdown();
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            var joypad = (Joypad) gamePad;
            var state = (State) State;
            var mmu = (MMU) MMU;
            mmu.Joypad = joypad;

            if (joypad.Column1 != 0xF || joypad.Column2 != 0xF)
                state.RequestInterrupt(InterruptType.Joypad);
        }
    }
}