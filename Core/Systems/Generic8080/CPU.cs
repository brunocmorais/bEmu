using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Util;

namespace bEmu.Core.Systems.Generic8080
{
    public class CPU : Intel8080<State, MMU>
    {
        public CPU(System system) : base(system) { }

        public override void In()
        {
            byte port = GetNextByte();

            switch (port)
			{
                case 1:
                    State.A = State.Ports.Read1;
                    break;
                case 2:
                    State.A = State.Ports.Read2;
                    break;
                case 3:
                    ushort value = BitUtils.GetWordFrom2Bytes(State.Ports.Shift0, State.Ports.Shift1);
                    State.A = (byte)((value >> (8 - State.Ports.Write2)) & 0xFF);
                    break;
                default:
                    break;
			}

            IncreaseCycles(10);
        }

        public override void Out()
        {
            byte port = GetNextByte();

            switch (port)
			{
                case 2:
                    State.Ports.Write2 = (byte)(State.A & 0x7);
                    break;
                case 3:
                    State.Ports.Write3 = State.A;
                    break;
                case 4:
                    State.Ports.Shift0 = State.Ports.Shift1;
                    State.Ports.Shift1 = State.A;
                    break;
                case 5:
                    State.Ports.Write5 = State.A;
                    break;
                default:
                    break;
			}

            IncreaseCycles(10);
        }
    }
}