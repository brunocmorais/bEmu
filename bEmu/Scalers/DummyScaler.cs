using bEmu.Core;

namespace bEmu.Scalers
{
    public class DummyScaler : BaseScaler
    {
        public override Framebuffer ScaledFramebuffer => Framebuffer;

        public DummyScaler() : base(1)  { }

        public override void Update() { }
    }
}