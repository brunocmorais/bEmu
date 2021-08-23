using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.IO;
using bEmu.Core.Memory;
using bEmu.Core.Util;

namespace bEmu.Systems.Generic8080
{
    public class ROMReader : Singleton<ROMReader>, IReader<IROM>
    {
        public IROM Read(string filePath)
        {
            var bytes = new byte[0x10000];
            var gameInfo = GameInfoReader.Instance.Read(AssetLoader.Load(SystemType.Generic8080, "games.dat"));

            var entries = new Dictionary<string, byte[]>();
            var gameInfoItem = gameInfo.Items.FirstOrDefault(x => x.ZipName == Path.GetFileNameWithoutExtension(filePath));

            using (var zipFile = ZipFile.OpenRead(filePath))
            {
                foreach (var fileName in gameInfoItem.FileNames)
                {
                    var stream = zipFile.GetEntry(fileName).Open();

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        entries.Add(fileName, memoryStream.ToArray());
                    }
                }
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var entryBytes = entries[gameInfoItem.FileNames[i]];
                var startAddress = Convert.ToInt32(gameInfoItem.MemoryPositions[i], 16);

                for (int j = 0; j < entryBytes.Length; j++)
                    bytes[j + startAddress] = entryBytes[j];
            }
                
            return new ROM(bytes, filePath);
        }

        public IROM Read(FileStream stream)
        {
            return Read(stream.Name);
        }
    }
}