using bEmu.Core.Audio;
using bEmu.Core.CPU;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Video;

namespace bEmu.Systems.NES
{
    public class System : Core.System.System
    {

        public override int Width { get; }

        public override int Height { get; }

        public override int StartAddress { get; }

        public override IRunner Runner { get; }

        public override IState State { get; }

        public override IMMU MMU { get; }

        public override IPPU PPU { get; }

        public override IAPU APU { get; }

        public override SystemType Type { get; }

        public System(string fileName) : base(fileName)
        {
            MMU = new MMU(this);
            State = GetInitialState();
            Runner = new CPU(this, 2144196);
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
    }
}