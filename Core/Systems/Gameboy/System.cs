using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Core.Systems.Gameboy.Sound;

namespace bEmu.Core.Systems.Gameboy
{
    public class System : Core.System
    {
        public override IState GetInitialState()
        {
            var state = new State(this);
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
            MMU = new MMU(State as State);
            PPU = new GPU.GPU(this);
            APU = new bEmu.Core.Systems.Gameboy.Sound.APU(this);
            Runner = new CPU(this);
        }

        public bool GBCMode { get; set; }
    }
}