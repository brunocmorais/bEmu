namespace bEmu.Core.Video.Scalers
{
    public abstract class Scaler : IScaler
    {
        public int ScaleFactor { get; }
        public virtual IFrameBuffer Framebuffer { get; }
        public virtual IFrameBuffer ScaledFramebuffer { get; }
        public int Frame { get; set; }

        public Scaler(IFrameBuffer framebuffer, int scaleFactor)
        {
            ScaleFactor = scaleFactor;
            Framebuffer = framebuffer;
            ScaledFramebuffer = new FrameBuffer(Framebuffer.Width * ScaleFactor, Framebuffer.Height * ScaleFactor);
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