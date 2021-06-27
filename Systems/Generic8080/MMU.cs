using System.Collections.Generic;
using System.IO;
using System.Reflection;
using bEmu.Core;
using bEmu.Core.Util;
using Newtonsoft.Json;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.MMU
    {
        private readonly IList<GameInfo> games;

        public MMU(ISystem system) : base(system, 0x10000) 
        { 
            string assetFolder = Path.Combine(Infrastructure.GetProgramLocation(), Generic8080.System.AssetFolder);
            string json = File.ReadAllText(Path.Combine(assetFolder, "games.json"));
            games = JsonConvert.DeserializeObject<IList<GameInfo>>(json);
        }

        public override void LoadProgram()
        {
            (System as Systems.Generic8080.System).LoadZipFile(games);
        }
    }
}