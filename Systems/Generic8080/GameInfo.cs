using System;
using System.Collections.Generic;
using System.IO;
using bEmu.Core.Extensions;

namespace bEmu.Systems.Generic8080
{
    public class GameInfo
    {
        public string ZipName { get; set; }
        public string[] FileNames { get; set; }
        public string[] MemoryPositions { get; set; }
    }

    public static class GameInfoReader
    {
        public static IEnumerable<GameInfo> Read(FileStream fileStream)
        {
            var str = fileStream.ToUTF8String();
            var split = str.Split(Environment.NewLine);

            for (int i = 0; i < split.Length; i += 3)
            {
                yield return new GameInfo()
                {
                    ZipName = split[i],
                    FileNames = split[i + 1].Split('|'),
                    MemoryPositions = split[i + 2].Split('|')
                };
            }
        }
    }
}