using bEmu.Core;
using bEmu.Core.Video;

namespace bEmu.Core.Video.Scalers
{
    public abstract class BaseScaler : IScaler
    {
        private IFrameBuffer original;
        public int ScaleFactor { get; }
        public virtual IFrameBuffer Framebuffer 
        { 
            get => original;
            set
            {
                original = value;
                ScaledFramebuffer = new FrameBuffer(value.Width * ScaleFactor, value.Height * ScaleFactor);
            } 
        }
        public virtual IFrameBuffer ScaledFramebuffer { get; protected set; }
        public int Frame { get; set; }

        public BaseScaler(int scaleFactor)
        {
            ScaleFactor = scaleFactor;
        }

        public uint this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Framebuffer.Width || y >= Framebuffer.Height)
                    return 0xFFFFFFFF;

                return Framebuffer[x, y];
            }
        }

        public void Update(int frame)
        {
            for (int x = 0; x < Framebuffer.Width; x++) 
                for (int y = 0; y < Framebuffer.Height; y++) 
                    Update(x, y);

            Frame = frame;
        }
        
        public abstract void Update(int x, int y);
    }
}