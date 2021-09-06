using System.Diagnostics;
using bEmu.Core;
using bEmu.Core.CPU.LR35902;
using bEmu.Core.CPU;
using bEmu.Systems.Gameboy.MBCs;
using bEmu.Core.System;

namespace bEmu.Systems.Gameboy
{
    public class CPU : LR35902<State, MMU>
    {
        public CPU(IGBSystem system, int clock) : base(system, clock) { }

        public override void HandleInterrupts()
        {
            if (State.IE == 0 || State.IF == 0)
                return;

            for (int i = 0; i < 5; i++)
            {
                int mask = (0x1 << i);

                if ((State.IE & State.IF & mask) == mask)
                {
                    State.Halted = false;

                    if (!State.EnableInterrupts)
                        return;

                    State.EnableInterrupts = false;
                    Rst((ushort) (0x40 + (0x8 * i)));
                    State.IF &= (byte) ~mask;
                    break;
                }
            }
        }

        protected override void Stop()
        {
            base.Stop();

            if ((System as System).GBCMode && (MMU[0xFF4D] & 0x1) == 0x1)
                MMU[0xFF4D] = 0xFE;
        }

        public override IOpcode StepCycle()
        {
            var opcode = base.StepCycle();

            if (((System) System).DoubleSpeedMode)
                opcode.CyclesTaken /= 2;

            State.Timer.UpdateTimers(opcode.CyclesTaken);

            if (MMU.MBC is IHasRTC)
                (MMU.MBC as IHasRTC).Tick(opcode.CyclesTaken);

            return opcode;
        }
    }
}