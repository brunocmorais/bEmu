using bEmu.Core;

namespace bEmu.Systems.Empty
{
    public class System : Core.System
    {
        public System() : base(string.Empty)
        {
        }

        public override int Width => 640;

        public override int Height => 480;

        public override int RefreshRate => 16;

        public override int CycleCount => 0;

        public override int StartAddress => 0;

        public override IState GetInitialState()
        {
            return default;
        }

        public override void Stop() {}

        public override void UpdateGamePad(IGamePad gamePad) {}
    }
}