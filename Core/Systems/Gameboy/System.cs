using bEmu.Core.CPUs.LR35902;
using bEmu.Core;

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
            state.PC = 0x00;

            return state;
        }

        public override void Initialize()
        {
            MMU = new MMU(this);
            PPU = new GPU(this, 160, 144);
            Runner = new LR35902<State>(this);
            base.Initialize();
        }
    }
}