using System.IO;
using System.Reflection;

namespace bEmu.Core.Util
{
    public static class Infrastructure
    {
        public static string GetProgramLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}