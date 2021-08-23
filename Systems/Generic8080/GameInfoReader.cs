using System;
using System.Collections.Generic;
using System.IO;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.Util;

namespace bEmu.Systems.Generic8080
{
    public class GameInfoReader : Singleton<GameInfoReader>, IReader<GameInfo>
    {
        
        private IEnumerable<GameInfoItem> Read(string[] items)
        {
            for (int i = 0; i < items.Length; i += 3)
            {
                yield return new GameInfoItem()
                {
                    ZipName = items[i],
                    FileNames = items[i + 1].Split('|'),
                    MemoryPositions = items[i + 2].Split('|')
                };
            }
        }

        public GameInfo Read(FileStream stream)
        {
            var items = stream.ToUTF8String().Split(Environment.NewLine);
            
            return new GameInfo(Read(items));
        }
    }
}