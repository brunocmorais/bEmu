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
        public bool GBCMode => (MMU as MMU).Bios.IsGBC;
        public IColorPalette ColorPalette { get; set; }
        public bool DoubleSpeedMode => (MMU[0xFF4D] & 0x80) == 0x80;
        public override int Width => 160;
        public override int Height => 144;
        public override int RefreshRate => 16;
        public override int CycleCount => 69905;
        public override int StartAddress => 0;

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
        }

        public override bool Update()
        {            
            while (Cycles >= 0)
            {
                if (!base.Update())
                    return false;
                    
                var opcode = Runner.StepCycle();
                //global::System.Console.Write(State.PC.ToString("x") + " ");

                PPU.Cycles += opcode.CyclesTaken;
                PPU.StepCycle();

                Cycles -= opcode.CyclesTaken;
                APU.Update(opcode.CyclesTaken);
            }

            return true;
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