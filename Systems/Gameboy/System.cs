using APU = bEmu.Systems.Gameboy.Sound.APU;
using bEmu.Core.Enums;
using bEmu.Systems.Gameboy.GPU.Palettes;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.System;

namespace bEmu.Systems.Gameboy
{
    public class System : VideoGameSystem
    {
        public bool GBCMode => ((ROM as ROM).CartridgeHeader.GBCFlag & 0x80) == 0x80;
        public IColorPalette ColorPalette { get; private set; }
        public bool DoubleSpeedMode => (MMU[0xFF4D] & 0x80) == 0x80;
        public override int Width => 160;
        public override int Height => 144;
        public override int StartAddress => 0;
        public override SystemType Type => SystemType.GameBoy;

        public System(IROM rom) : base(rom)
        {
            MMU = new MMU(this);
            State = GetInitialState();
            PPU = new GPU.GPU(this);
            APU = new APU(this);
            Runner = new CPU(this, 4194304);
            ColorPalette = ColorPaletteFactory.Get(MonochromePaletteType.Gray);
        }

        public override IState GetInitialState()
        {
            if (GBCMode)
                return Gameboy.State.GetCGBState(this);
            else
                return Gameboy.State.GetCGBState(this);
        }

        public override bool Update()
        {    
            if (!base.Update())
                return false;

            while (Cycles >= 0)
            {                    
                var opcode = Runner.StepCycle();

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

        public void SetColorPalette(MonochromePaletteType type)
        {
            ColorPalette = ColorPaletteFactory.Get(type);
        }
    }
}