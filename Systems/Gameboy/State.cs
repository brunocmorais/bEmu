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

        public State(System system) : base(system)
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

        private static State GetState(System system)
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

        public static State GetDMGState(System system)
        {
            var state = GetState(system);

            state.A = 0x1;
            state.BC = 0x0013;
            state.DE = 0x00D8;
            state.HL = 0x014D;

            return state;
        }

        public static State GetCGBState(System system)
        {
            var state = GetState(system);

            state.A = 0x11;
            state.BC = 0x0000;
            state.DE = 0xFF56;
            state.HL = 0x000D;

            return state;
        }
    }
}