using bEmu.Core;

namespace bEmu.Scalers
{
    public abstract class BaseScaler : IScaler
    {
        public int ScaleFactor { get; set; }
        public virtual Framebuffer Original { get; protected set; }
        public virtual Framebuffer Scaled { get; protected set; }

        public BaseScaler(Framebuffer framebuffer, int size)
        {
            this.Original = framebuffer;
            ScaleFactor = size;
            Scaled = new Framebuffer(framebuffer.Width * ScaleFactor, framebuffer.Height * ScaleFactor);
        }

        public uint this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Original.Width || y >= Original.Height)
                    return 0;

                return Original[x, y];
            }
        }

        public virtual void Update()
        {
            if (Scaled.Width != Original.Width * ScaleFactor || Scaled.Height != Original.Height * ScaleFactor)
                Scaled = new Framebuffer(Original.Width, Original.Height);
        }
    }
}