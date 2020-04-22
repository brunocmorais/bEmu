using bEmu.Core.CPUs.Intel8080;
using bEmu.Core;

namespace bEmu.Core.Systems.Generic8080
{
    public class System : Core.System
    {
        public override IState GetInitialState()
        {
            var state = new State(this);

            state.A = 0;
            state.B = 0;
            state.C = 0;
            state.D = 0;
            state.E = 0;
            state.H = 0;
            state.SP = 0xf000;
            state.PC = 0;
            state.EnableInterrupts = false;
            state.Flags = new Flags();
            state.Ports = new Ports();
            state.Cycles = 0;
            state.Halted = false;
            state.Instructions = 0;

            return state;
        }

        public override void Initialize()
        {
            base.Initialize();
            MMU = new MMU(0x10000);
            PPU = new PPU(this, 224, 256);
            Runner = new Intel8080<State, MMU>(this);
        }

        public void SetStartPoint(ushort pc)
        {
            State.PC = pc;
        }
    }
}