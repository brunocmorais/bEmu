using bEmu.Core;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;

namespace bEmu.Core.System
{
    public class EmptySystem : BaseSystem
    {
        public override SystemType Type => SystemType.None;
        
        public EmptySystem() : base(string.Empty)
        {
        }

        public override void Reset() { }

        public override void Stop() { }

        public override bool Update()
        {
            return true;
        }

        public override void AttachDebugger() { }
    }
}