using bEmu.Core.Enums;

namespace bEmu.Core.System
{
    public interface ISystem
    {
        string FileName { get; }
        IDebugger Debugger { get; }
        SystemType Type { get; }
        string FileNameWithoutExtension { get; }
        string FilePath { get; }
        string SaveFileName { get; }

        void Reset();
        bool Update();
        void Stop();
        void AttachDebugger();
    }
}