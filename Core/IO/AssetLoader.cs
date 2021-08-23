using System.IO;
using bEmu.Core.Enums;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.IO
{
    public static class AssetLoader
    {
        public const string AssetFolder = "Assets";
        public static FileStream Load(ISystem system, string assetName)
        {
            return Load(system.Type, assetName);
        }

        public static FileStream Load(SystemType type, string assetName)
        {
            return Load(type.ToString(), assetName);
        }

        public static FileStream Load(string assetName)
        {
            return Load("Common", assetName);
        }

        private static FileStream Load(string folderName, string assetName)
        {
            var path = Path.Combine(FileManager.ProgramLocation, AssetFolder, folderName, assetName);
            return File.OpenRead(path);
        }
    }
}