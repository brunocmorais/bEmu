using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class DummyScaler : BaseScaler
    {
        public override Framebuffer ScaledFramebuffer => Framebuffer;

        public DummyScaler() : base(1)  { }

        public override void Update() { }
    }
}