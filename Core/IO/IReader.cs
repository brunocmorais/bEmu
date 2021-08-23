using System.IO;

namespace bEmu.Core.IO
{
    public interface IReader<T> where T : class
    {
        T Read(FileStream stream);
    }
}