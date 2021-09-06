using System;
using System.IO;
using bEmu.Core.CPU.LR35902;
using bEmu.Core;
using bEmu.Core.Util;
using bEmu.Systems.Gameboy.GPU;
using bEmu.Core.System;

namespace bEmu.Systems.Gameboy
{
    public class State : Core.CPU.LR35902.State
    {
        public LCD LCD { get; set; }

        public State(IGBSystem system) : base(system)
        {
            Timer = new Timer(system);
        }

        public Timer Timer { get; set; }

        public byte IE
        {
            get { return System.MMU[0xFFFF]; }
            set { System.MMU[0xFFFF] = value; }
        }

        public byte IF
        {
            get { return System.MMU[0xFF0F]; }
            set { System.MMU[0xFF0F] = value; }
        }

        public void EnableInterrupt(InterruptType type)
        {
            IE |= (byte) (0x1 << (int) type);
        }

        public void RequestInterrupt(InterruptType type)
        {
            IF |= (byte) (0x1 << (int) type);
        }

        public override void Reset()
        {
            base.Reset();
            Timer = new Timer(System as Gameboy.System);
        }

        private static State GetState(IGBSystem system)
        {
            var state = new bEmu.Systems.Gameboy.State(system);
            state.Flags = new Flags();

            state.EnableInterrupts = false;
            state.Cycles = 0;
            state.Halted = false;
            state.Instructions = 0;
            state.PC = 0x0100;
            state.SP = 0xFFFE;

            state.Flags.Zero = true;
            state.Flags.Subtract = false;
            state.Flags.HalfCarry = false;
            state.Flags.Carry = false;

            return state;
        }

        public static State GetDMGState(IGBSystem system)
        {
            var state = GetState(system);

            state.A = 0x1;
            state.BC = 0x0013;
            state.DE = 0x00D8;
            state.HL = 0x014D;

            var io = (system.MMU as MMU).IO;

            io[0x00] = 0xCF;
            io[0x01] = 0x00;
            io[0x02] = 0x7E;
            io[0x04] = 0xAB;
            io[0x05] = 0x00;
            io[0x06] = 0x00;
            io[0x07] = 0xF8;
            io[0x0F] = 0xE1;
            io[0x10] = 0x80;
            io[0x11] = 0xBF;
            io[0x12] = 0xF3;
            io[0x13] = 0xFF;
            io[0x14] = 0xBF;
            io[0x16] = 0x3F;
            io[0x17] = 0x00;
            io[0x18] = 0xFF;
            io[0x19] = 0xBF;
            io[0x1A] = 0x7F;
            io[0x1B] = 0xFF;
            io[0x1C] = 0x9F;
            io[0x1D] = 0xFF;
            io[0x1E] = 0xBF;
            io[0x20] = 0xFF;
            io[0x21] = 0x00;
            io[0x22] = 0x00;
            io[0x23] = 0xBF;
            io[0x24] = 0x77;
            io[0x25] = 0xF3;
            io[0x26] = 0xF1;
            io[0x40] = 0x91;
            io[0x41] = 0x85;
            io[0x42] = 0x00;
            io[0x43] = 0x00;
            io[0x44] = 0x00;
            io[0x45] = 0x00;
            io[0x46] = 0xFF;
            io[0x47] = 0xFC;
            io[0x48] = 0xFF;
            io[0x49] = 0xFF;
            io[0x4A] = 0x00;
            io[0x4B] = 0x00;
            io[0x4D] = 0xFF;
            io[0x4F] = 0xFF;
            io[0x51] = 0xFF;
            io[0x52] = 0xFF;
            io[0x53] = 0xFF;
            io[0x54] = 0xFF;
            io[0x55] = 0xFF;
            io[0x56] = 0xFF;
            io[0x68] = 0xFF;
            io[0x69] = 0xFF;
            io[0x6A] = 0xFF;
            io[0x6B] = 0xFF;
            io[0x70] = 0xFF;

            return state;
        }

        public static State GetCGBState(IGBSystem system)
        {
            var state = GetState(system);

            state.A = 0x11;
            state.BC = 0x0000;
            state.DE = 0xFF56;
            state.HL = 0x000D;

            var io = (system.MMU as MMU).IO;

            io[0x00] = 0xCF;
            io[0x01] = 0x00;
            io[0x02] = 0x7F;
            io[0x04] = 0x00;
            io[0x05] = 0x00;
            io[0x06] = 0x00;
            io[0x07] = 0xF8;
            io[0x0F] = 0xE1;
            io[0x10] = 0x80;
            io[0x11] = 0xBF;
            io[0x12] = 0xF3;
            io[0x13] = 0xFF;
            io[0x14] = 0xBF;
            io[0x16] = 0x3F;
            io[0x17] = 0x00;
            io[0x18] = 0xFF;
            io[0x19] = 0xBF;
            io[0x1A] = 0x7F;
            io[0x1B] = 0xFF;
            io[0x1C] = 0x9F;
            io[0x1D] = 0xFF;
            io[0x1E] = 0xBF;
            io[0x20] = 0xFF;
            io[0x21] = 0x00;
            io[0x22] = 0x00;
            io[0x23] = 0xBF;
            io[0x24] = 0x77;
            io[0x25] = 0xF3;
            io[0x26] = 0xF1;
            io[0x40] = 0x91;
            io[0x41] = 0x00;
            io[0x42] = 0x00;
            io[0x43] = 0x00;
            io[0x44] = 0x00;
            io[0x45] = 0x00;
            io[0x46] = 0x00;
            io[0x47] = 0xFC;
            io[0x48] = 0x00;
            io[0x49] = 0x00;
            io[0x4A] = 0x00;
            io[0x4B] = 0x00;
            io[0x4D] = 0xFF;
            io[0x4F] = 0xFF;
            io[0x51] = 0xFF;
            io[0x52] = 0xFF;
            io[0x53] = 0xFF;
            io[0x54] = 0xFF;
            io[0x55] = 0xFF;
            io[0x56] = 0xFF;
            io[0x68] = 0x00;
            io[0x69] = 0x00;
            io[0x6A] = 0x00;
            io[0x6B] = 0x00;
            io[0x70] = 0xFF;

            return state;
        }
    }
}