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

            state.AF = 0x01B0;
            state.BC = 0x0013;
            state.DE = 0x00D8;
            state.HL = 0x014D;
            state.SP = 0xFFFE;

            MMU[0xFF05] = 0x00;
            MMU[0xFF06] = 0x00;
            MMU[0xFF07] = 0x00;
            MMU[0xFF10] = 0x80;
            MMU[0xFF11] = 0xBF;
            MMU[0xFF12] = 0xF3;
            MMU[0xFF14] = 0xBF;
            MMU[0xFF16] = 0x3F;
            MMU[0xFF17] = 0x00;
            MMU[0xFF19] = 0xBF;
            MMU[0xFF1A] = 0x7F;
            MMU[0xFF1B] = 0xFF;
            MMU[0xFF1C] = 0x9F;
            MMU[0xFF1E] = 0xBF;
            MMU[0xFF20] = 0xFF;
            MMU[0xFF21] = 0x00;
            MMU[0xFF22] = 0x00;
            MMU[0xFF23] = 0xBF;
            MMU[0xFF24] = 0x77;
            MMU[0xFF25] = 0xF3;
            MMU[0xFF26] = 0xF1;
            MMU[0xFF40] = 0x91;
            MMU[0xFF42] = 0x00;
            MMU[0xFF43] = 0x00;
            MMU[0xFF45] = 0x00;
            MMU[0xFF47] = 0xFC;
            MMU[0xFF48] = 0xFF;
            MMU[0xFF49] = 0xFF;
            MMU[0xFF4A] = 0x00;
            MMU[0xFF4B] = 0x00;
            MMU[0xFFFF] = 0x00;

            state.PC = 0x100;

            state.EnableInterrupts = false;
            state.Cycles = 0;
            state.Halted = false;
            state.Instructions = 0;
            state.Joypad = new Joypad();

            return state;
        }

        public override void Initialize()
        {
            MMU = new MMU(this);
            PPU = new GPU(this, 160, 144);
            Runner = new LR35902<CPUs.LR35902.State>(this);
            base.Initialize();
        }
    }
}