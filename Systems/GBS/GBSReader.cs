using System.IO;
using bEmu.Core.IO;
using bEmu.Core.Util;

namespace bEmu.Systems.GBS
{
    public class GBSReader : Singleton<GBSReader>, IReader<GBS>
    {
        public GBS Read(FileStream stream)
        {
            return null;
        }
    }
}