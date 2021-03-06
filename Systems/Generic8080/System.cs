using bEmu.Core.CPU.Intel8080;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System;
using bEmu.Core.Enums;
using bEmu.Core.Audio;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;
using bEmu.Core.System;
using bEmu.Core.IO;

namespace bEmu.Systems.Generic8080
{
    public class System : VideoGameSystem
    {
        public override int Width => 224;
        public override int Height => 256;
        public override int StartAddress => 0;
        public override SystemType Type => SystemType.Generic8080;

        public System(IROM rom) : base(rom)
        {
            State = GetInitialState();
            MMU = new MMU(this);
            PPU = new PPU(this, 224, 256);
            Runner = new CPU(this, 4194304);
            APU = new APU(this);

            ((Systems.Generic8080.State) State).UpdatePorts(1, 0x01);
            ((Systems.Generic8080.State) State).UpdatePorts(2, 0x00);
        }

        public override IState GetInitialState()
        {
            var state = new State(this);

            state.A = 0;
            state.B = 0;
            state.C = 0;
            state.D = 0;
            state.E = 0;
            state.H = 0;
            state.SP = 0xf000;
            state.PC = 0;
            state.EnableInterrupts = false;
            state.Flags = new Flags();
            state.Ports = new Ports();
            state.Cycles = 0;
            state.Halted = false;
            state.Instructions = 0;

            return state;
        }

        public void SetStartPoint(ushort pc)
        {
            (State as State).PC = pc;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override bool Update()
        {
            if (!base.Update())
                return false;

            int halfCycles = (CycleCount / 2);
            int lastInterrupt = GenerateInterrupt(2);

            while (Cycles >= 0)
            {
                var opcode = Runner.StepCycle();
                Cycles -= opcode.CyclesTaken;

                if (lastInterrupt == 2 && Cycles <= halfCycles)
                    lastInterrupt = GenerateInterrupt(1);

                APU.Update(Cycles);
            }

            return true;
        }

        private int GenerateInterrupt(int interrupt)
        {
            if (((Systems.Generic8080.State)State).EnableInterrupts)
            {
                if (interrupt == 1)
                    PPU.IncreaseFrame();

                (Runner as CPU).GenerateInterrupt(interrupt);
            }

            return interrupt;
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            byte read1 = 0;
            
            if (gamePad.IsKeyDown(GamePadKey.D5))
                read1 = (byte) (read1 | (1 << 0));
            
            if (gamePad.IsKeyDown(GamePadKey.D1))
                read1 = (byte) (read1 | (1 << 2));
            
            if (gamePad.IsKeyDown(GamePadKey.Space))
                read1 = (byte) (read1 | (1 << 4));

            if (gamePad.IsKeyDown(GamePadKey.Left))
                read1 = (byte) (read1 | (1 << 5));

            if (gamePad.IsKeyDown(GamePadKey.Right))
                read1 = (byte) (read1 | (1 << 6));

            ((State)State).UpdatePorts(1, read1);
        }

        public void SetCustomColors(bool value)
        {
            (PPU as PPU).SetCustomColors(value);
        }

        public void SetBackdrop(bool value)
        {
            (PPU as PPU).SetBackdrop(value);
        }

        public override void Stop() { }
    }
}