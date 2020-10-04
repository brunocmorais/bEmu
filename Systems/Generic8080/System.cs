using bEmu.Core.CPUs.Intel8080;
using bEmu.Core;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace bEmu.Systems.Generic8080
{
    public class System : Core.System
    {
        public System(string fileName) : base(fileName)
        {
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

        public override void Initialize()
        {
            base.Initialize();
            MMU = new MMU(this);
            PPU = new PPU(this, 224, 256);
            Runner = new CPU(this);
        }

        public void SetStartPoint(ushort pc)
        {
            State.PC = pc;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}