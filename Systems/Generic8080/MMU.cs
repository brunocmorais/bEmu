using System.Collections.Generic;
using System.IO;
using System.Reflection;
using bEmu.Core;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.System;
using bEmu.Core.Util;
using Newtonsoft.Json;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.Memory.MMU
    {
        private readonly IList<GameInfo> games;

        public MMU(ISystem system) : base(system, 0x10000) 
        { 
            string json = AssetLoader.Load(system, "games.json").ToUTF8String();
            games = JsonConvert.DeserializeObject<IList<GameInfo>>(json);
        }

        public override void LoadProgram()
        {
            (System as Systems.Generic8080.System).LoadZipFile(games);
        }
    }
}