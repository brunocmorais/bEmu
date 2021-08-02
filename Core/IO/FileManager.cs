using System.IO;
using System.Reflection;

namespace bEmu.Core.IO
{
    public static class FileManager
    {
        public static string ProgramLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string GetPath(string fileName) => Path.Combine(ProgramLocation, fileName);
        public static FileStream Read(string fileName) => File.OpenRead(GetPath(fileName));
        public static void Write(string fileName, byte[] bytes) => File.WriteAllBytes(GetPath(fileName), bytes);
    }
}