using System.IO;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.Memory
{
    public class ROMReader : Singleton<ROMReader>, IReader<IROM>
    {
        public virtual IROM Read(FileStream stream) => new ROM(stream);

        public virtual IROM Read(string filePath) => Read(FileManager.Read(filePath));
    }
}