using bEmu.Core;
using bEmu.Core.CPUs.LR35902;

namespace bEmu.Systems.Gameboy
{
    public class CPU : LR35902<State, MMU>
    {
        public CPU(ISystem system) : base(system) { }

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
    }
}