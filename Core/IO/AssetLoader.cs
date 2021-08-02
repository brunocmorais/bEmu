using System.IO;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.IO
{
    public static class AssetLoader
    {
        public const string AssetFolder = "Assets";
        public static FileStream Load(ISystem system, string assetName)
        {
            var path = Path.Combine(FileManager.ProgramLocation, AssetFolder, system.Type.ToString(), assetName);
            return File.OpenRead(path);
        }
    }
}