using System.IO;
using bEmu.Core.Extensions;

namespace bEmu.Core.Memory
{

    public class ROM : IROM
    {
        public byte[] Bytes { get; }
        public string FileName { get; }
        public string FileNameWithoutExtension => FileExtensions.GetFileNameWithoutExtension(FileName);
        public string FilePath => Path.GetDirectoryName(FileName);
        public string SaveFileName => FileNameWithoutExtension + ".sav";

        public ROM(FileStream fileStream) : this(fileStream.ToBytes(), fileStream.Name)
        {
        }

        public ROM(byte[] bytes, string fileName)
        {
            Bytes = bytes;
            FileName = fileName;
        }
    }
}