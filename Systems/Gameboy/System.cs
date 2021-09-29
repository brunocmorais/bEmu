using APU = bEmu.Systems.Gameboy.Sound.APU;
using bEmu.Core.Enums;
using bEmu.Systems.Gameboy.GPU.Palettes;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Systems.Gameboy.MBCs;

namespace bEmu.Systems.Gameboy
{
    public class System : VideoGameSystem, IGBSystem
    {
        public bool GBCMode => ((ROM as ROM).CartridgeHeader.GBCFlag & 0x80) == 0x80;
        public IColorPalette ColorPalette { get; private set; }
        public bool DoubleSpeedMode => GBCMode && (MMU[0xFF4D] & 0x80) == 0x80;
        public override int Width => 160;
        public override int Height => 144;
        public override int StartAddress => 0;
        public override SystemType Type => SystemType.GameBoy;

        public System(IROM rom) : base(rom)
        {
            Runner = new CPU(this, 4194304);
            MMU = new MMU(this);
            State = GetInitialState();
            PPU = new GPU.GPU(this);
            APU = new APU(this);
            ColorPalette = ColorPaletteFactory.Get(MonochromePaletteType.Gray);
        }

        public override IState GetInitialState()
        {
            if (GBCMode)
                return Gameboy.State.GetCGBState(this);
            else
                return Gameboy.State.GetDMGState(this);
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
            var mbc = (MMU as MMU).MBC;

            if (mbc != null && mbc is IRTC)
                (mbc as IRTC).Shutdown();
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            var joypad = ((Systems.Gameboy.MMU) MMU).Joypad;

            joypad.Update(gamePad);

            if (joypad.Column1 != 0xF || joypad.Column2 != 0xF)
                ((State)State).RequestInterrupt(InterruptType.Joypad);
        }

        public void SetColorPalette(MonochromePaletteType type)
        {
            ColorPalette = ColorPaletteFactory.Get(type);
        }
    }
}