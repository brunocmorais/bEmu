using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public class EmptyScaler : Scaler
    {
        public override IFrameBuffer ScaledFramebuffer => Framebuffer;

        public EmptyScaler() : base(1)  { }

        public override void Update(int x, int y)
        {
            
        }
    }
}