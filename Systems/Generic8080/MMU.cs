using System.Collections.Generic;
using System.Linq;
using bEmu.Core.IO;
using bEmu.Core.System;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.Memory.MMU
    {
        private readonly IList<GameInfo> games;

        public MMU(ISystem system) : base(system, 0x10000) 
        {
            games = GameInfoReader.Read(AssetLoader.Load(system, "games.dat")).ToList();
        }

        public override void LoadProgram()
        {
            (System as Systems.Generic8080.System).LoadZipFile(games);
        }
    }
}