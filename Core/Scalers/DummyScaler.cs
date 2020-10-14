using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class DummyScaler : BaseScaler
    {
        public override Framebuffer ScaledFramebuffer => Framebuffer;

        public DummyScaler() : base(1)  { }

        public override void Update() 
        { 
            // for (int i = 0; i < Framebuffer.Width; i++)
            //     for (int j = 0; j < Framebuffer.Height; j++)
            //         ScaledFramebuffer[i, j] = Framebuffer[i, j];
        }
    }
}