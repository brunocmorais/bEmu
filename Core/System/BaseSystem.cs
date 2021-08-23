using System.IO;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;
using bEmu.Core.Memory;

namespace bEmu.Core.System
{
    public abstract class BaseSystem : ISystem
    {
        public IDebugger Debugger { get; protected set; }
        public abstract SystemType Type { get; }
        public IROM ROM { get; }

        public BaseSystem(IROM rom)
        {
            ROM = rom;
        }

        protected BaseSystem() { }
        
        public abstract void Reset();
        public abstract void Stop();
        public abstract bool Update();
        public abstract void AttachDebugger();
    }
}