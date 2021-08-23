using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Core.IO;
using bEmu.Core.System;

namespace bEmu.Systems.Generic8080
{
    public class MMU : Core.Memory.MMU
    {
        public MMU(IRunnableSystem system) : base(system, 0x10000) 
        {
        }

        public override void LoadProgram()
        {
            var gameInfos = GameInfoReader.Read(AssetLoader.Load(System, "games.dat")).ToList();

            var entries = new Dictionary<string, byte[]>();
            var gameInfo = gameInfos.FirstOrDefault(x => x.ZipName == Path.GetFileNameWithoutExtension(System.FileName));

            using (var zipFile = ZipFile.OpenRead($"{System.FileName}"))
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
                LoadProgram(entries[gameInfo.FileNames[i]], Convert.ToInt32(gameInfo.MemoryPositions[i], 16));
        }
    }
}