using bEmu.Core.Enums;
using bEmu.Core.Memory;
using bEmu.Core.System;

namespace bEmu.Systems.GBS
{
    public class System : AudioSystem
    {
        public System(IROM rom) : base(rom)
        {
        }

        public override int StartAddress { get; }

        public override SystemType Type => SystemType.GameBoySoundSystem;

        public override IState GetInitialState()
        {
            throw new global::System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new global::System.NotImplementedException();
        }

        public override bool Update()
        {
            return base.Update();
        }
    }
}