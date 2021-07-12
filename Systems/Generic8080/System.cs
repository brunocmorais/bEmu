using bEmu.Core.CPU.Intel8080;
using bEmu.Core;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;
using bEmu.Core.Enums;
using bEmu.Core.Audio;
using bEmu.Core.Input;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;
using bEmu.Core.System;

namespace bEmu.Systems.Generic8080
{
    public class System : Core.System.System
    {
        private int lastInterrupt = 1;
        public override int Width => 224;
        public override int Height => 256;
        public override int RefreshRate => 8;
        public override int CycleCount => 34952;
        public override int StartAddress => 0;
        public override SystemType Type => SystemType.Generic8080;
        public override IRunner Runner { get; }
        public override IState State { get; }
        public override IMMU MMU { get; }
        public override IPPU PPU { get; }
        public override IAPU APU { get; }
        public const string AssetFolder = "Assets/Generic8080";

        public System(string fileName) : base(fileName)
        {
            State = GetInitialState();
            MMU = new MMU(this);
            PPU = new PPU(this, 224, 256);
            Runner = new CPU(this);
            APU = new APU(this);

            ((Systems.Generic8080.State) State).UpdatePorts(1, 0x01);
            ((Systems.Generic8080.State) State).UpdatePorts(2, 0x00);
        }

        public void LoadZipFile(IList<GameInfo> gameInfos)
        {
            var entries = new Dictionary<string, byte[]>();
            var gameInfo = gameInfos.FirstOrDefault(x => x.ZipName == Path.GetFileNameWithoutExtension(FileName));

            using (var zipFile = ZipFile.OpenRead($"{FileName}"))
            {
                foreach (var fileName in gameInfo.FileNames)
                {
                    var stream = zipFile.GetEntry(fileName).Open();

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        entries.Add(fileName, memoryStream.ToArray());
                    }
                }
            }

            for (int i = 0; i < gameInfo.FileNames.Length; i++)
                MMU.LoadProgram(entries[gameInfo.FileNames[i]], Convert.ToInt32(gameInfo.MemoryPositions[i], 16));
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
            State.PC = pc;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override bool Update()
        {
            if (!base.Update())
                return false;
                
            var state = (Systems.Generic8080.State) State;

            if (state.EnableInterrupts)
            {
                lastInterrupt = lastInterrupt == 1 ? 2 : 1;

                if (lastInterrupt == 1)
                    PPU.Frame++;

                (Runner as CPU).GenerateInterrupt(lastInterrupt);
            }

            while (Cycles >= 0)
            {
                if (!base.Update())
                    return false;

                var opcode = Runner.StepCycle();
                Cycles -= opcode.CyclesTaken;
                APU.Update(Cycles);
            }

            return true;
        }

        public override void Stop()
        {
            
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
    }
}