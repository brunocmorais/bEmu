using bEmu.Core;

namespace bEmu.Systems.Dummy
{
    public class System : Core.System
    {
        public System(string fileName) : base(fileName)
        {
        }

        public override int Width => 640;

        public override int Height => 480;

        public override int RefreshRate => 16;

        public override int CycleCount => 0;

        public override IState GetInitialState()
        {
            return null;
        }

        public override void Stop()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void UpdateGamePad(IGamePad gamePad)
        {
            
        }

        public override void Initialize()
        {
            PPU = new PPU(this, Width, Height);
        }
    }
}