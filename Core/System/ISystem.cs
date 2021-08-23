using bEmu.Core.Enums;
using bEmu.Core.Memory;

namespace bEmu.Core.System
{
    public interface ISystem
    {
        IDebugger Debugger { get; }
        SystemType Type { get; }
        IROM ROM { get; }

        void Reset();
        bool Update();
        void Stop();
        void AttachDebugger();
    }
}