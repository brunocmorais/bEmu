using System.IO;
using bEmu.Core.IO;
using bEmu.Core.Memory;
using bEmu.Core.Util;

namespace bEmu.Systems.GBS
{
    public class ROMReader : Singleton<ROMReader>, IReader<IROM>
    {
        public IROM Read(FileStream fileStream)
        {
            var header = GBSHeaderReader.Instance.Read(fileStream);
            return new ROM(fileStream, header);
        }
    }
}