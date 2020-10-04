using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Systems.Gameboy.Sound;

namespace bEmu.Systems.Gameboy
{
    public class System : Core.System
    {
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
        }

        public bool GBCMode { get; set; }

        public override void Reset()
        {
            base.Reset();
            GBCMode = false;
        }
    }
}