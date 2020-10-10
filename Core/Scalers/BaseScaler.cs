using bEmu.Core;

namespace bEmu.Core.Scalers
{
    public abstract class BaseScaler : IScaler
    {
        private Framebuffer original;
        public int ScaleFactor { get; set; }
        public virtual Framebuffer Framebuffer 
        { 
            get => original;
            set
            {
                original = value;
                ScaledFramebuffer = new Framebuffer(value.Width * ScaleFactor, value.Height * ScaleFactor);
            } 
        }
        public virtual Framebuffer ScaledFramebuffer { get; protected set; }

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

        public abstract void Update();
    }
}