using System;
using System.IO;
using bEmu.Core.CPUs.LR35902;
using bEmu.Core;
using bEmu.Core.Util;
using bEmu.Systems.Gameboy.GPU;

namespace bEmu.Systems.Gameboy
{
    public class State : Core.CPUs.LR35902.State
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
    }
}