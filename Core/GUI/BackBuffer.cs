using bEmu.Core.GUI;
using bEmu.Core.Video;

namespace bEmu.Core.GUI
{
    public class BackBuffer : IDrawable
    {
        public IFrameBuffer FrameBuffer { get; }

        public BackBuffer(IFrameBuffer frameBuffer)
        {
            FrameBuffer = frameBuffer;
        }

        public void Update()
        {
            
        }
    }
}