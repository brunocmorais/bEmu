using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Systems.Gameboy;

namespace bEmu.Systems.GBS
{
    public class System : AudioSystem, IGBSystem
    {
        public override int StartAddress { get; }
        public override SystemType Type => SystemType.GameBoySoundSystem;

        public System(IROM rom) : base(rom)
        { 
            MMU = new Gameboy.MMU(this);
            State = GetInitialState();
            APU = new Gameboy.Sound.APU(this);
            Runner = new CPU(this, 4194304);
        }

        public override IState GetInitialState()
        {
            var state = Gameboy.State.GetDMGState(this);

            state.SP = (ROM as ROM).GBSHeader.SP;
            state.PC = (ROM as ROM).GBSHeader.InitAddress;
            
            return state;
        }

        public override void Stop()
        {
        }

        public override bool Update()
        {
            if (!base.Update())
                return false;

            while (Cycles >= 0)
            {                    
                var opcode = Runner.StepCycle();

                Cycles -= opcode.CyclesTaken;
                APU.Update(opcode.CyclesTaken);
            }

            return true;
        }
    }
}