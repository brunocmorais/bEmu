using bEmu.Core;

namespace bEmu.Scalers
{
    public class DummyScaler : BaseScaler
    {
        public DummyScaler(Framebuffer framebuffer) : base(framebuffer, 1)
        {
            Scaled = framebuffer;
        }

        public override void Update() 
        { 
            base.Update();
        }
    }
}