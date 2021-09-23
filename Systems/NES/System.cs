using bEmu.Core.Audio;
using bEmu.Core.CPU;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Video;

namespace bEmu.Systems.NES
{
    public class System : VideoGameSystem
    {

        public override int Width { get; }
        public override int Height { get; }
        public override int StartAddress { get; }

        public override SystemType Type { get; }

        public System(IROM rom) : base(rom)
        {
            Runner = new CPU(this, 2144196);
            MMU = new MMU(this);
            State = GetInitialState();
        }

        public override IState GetInitialState()
        {
            var state = new State(this);
            state.A = 0x00;
            state.Y = 0x00;
            state.X = 0x00;
            state.PC = (ushort)((MMU[CPU.RstVectorL] << 8) | MMU[CPU.RstVectorH]);
            state.SP = 0xFD;

            return state;
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            throw new global::System.NotImplementedException();
        }

        public override bool Update()
        {
            if (!base.Update())
                return false;

            while (Cycles >= 0)
            {
                var opcode = Runner.StepCycle();
                Cycles -= opcode.CyclesTaken;
            }

            return true;
        }

        public override void Stop()
        {
            throw new global::System.NotImplementedException();
        }
    }
}