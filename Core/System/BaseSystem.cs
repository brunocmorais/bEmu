using System.IO;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;

namespace bEmu.Core.System
{
    public abstract class BaseSystem : ISystem
    {
        public string FileName { get; }
        public IDebugger Debugger { get; protected set; }
        public abstract SystemType Type { get; }
        public string FileNameWithoutExtension => FileExtensions.GetFileNameWithoutExtension(FileName);
        public string FilePath => Path.GetDirectoryName(FileName);
        public string SaveFileName => FileNameWithoutExtension + ".sav";

        public BaseSystem(string fileName)
        {
            FileName = fileName;
        }

        
        public abstract void Reset();
        public abstract void Stop();
        public abstract bool Update();
        public abstract void AttachDebugger();
    }
}