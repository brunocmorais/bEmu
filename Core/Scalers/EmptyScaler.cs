using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public class EmptyScaler : BaseScaler
    {
        public override Framebuffer ScaledFramebuffer => Framebuffer;

        public EmptyScaler() : base(1)  { }

        public override void Update(int frame) 
        { 
            base.Update(frame);
        }
    }
}