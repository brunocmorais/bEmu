using bEmu.Core;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.Memory;
using bEmu.Core.CPU;
using bEmu.Core.Video;

namespace bEmu.Core
{
    public class EmptySystem : Core.System
    {
        public override int Width => 640;
        public override int Height => 480;
        public override int RefreshRate => 16;
        public override int CycleCount => 0;
        public override int StartAddress => 0;
        public override SystemType Type => 0;
        public override IRunner Runner { get; }
        public override IState State { get; }
        public override IMMU MMU { get; }
        public override IPPU PPU { get; }
        public override IAPU APU { get; }

        public EmptySystem() : base(string.Empty)
        {
        }

        public override IState GetInitialState() => default;

        public override void Stop() {}

        public override void UpdateGamePad(IGamePad gamePad) {}
    }
}