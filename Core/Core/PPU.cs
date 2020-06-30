namespace bEmu.Core
{

    public abstract class PPU : IPPU
    {
        public ISystem System { get; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public virtual byte[] FrameBuffer { get; private set; }
        public int Frameskip { get; set; }
        public int Frame { get; set; }
        public int Cycles { get; set; }

        public PPU(ISystem system, int width, int height)
        {
            System = system;
            DefineSize(width, height);
        }

        public void DefineSize(int width, int height)
        {
            Width = width;
            Height = height;
            FrameBuffer = new byte[width * height * 4];
        }

        public void ResetFrameBuffer()
        {
            FrameBuffer = new byte[Width * Height * 4];
        }

        public virtual void SetPixel(int x, int y, uint color)
        {
            int start = (y * Width * 4) + (x * 4);
            FrameBuffer[start]     = (byte) ((color & 0xFF000000) >> 24);
            FrameBuffer[start + 1] = (byte) ((color & 0x00FF0000) >> 16);
            FrameBuffer[start + 2] = (byte) ((color & 0x0000FF00) >>  8);
            FrameBuffer[start + 3] = (byte) ((color & 0x000000FF));
        }

        public virtual uint GetPixel(int x, int y)
        {
            int start = (y * Width * 4) + (x * 4);
            return (uint) ((FrameBuffer[start] << 24) | (FrameBuffer[start + 1] << 16) | (FrameBuffer[start + 2] << 8) | (FrameBuffer[start + 3])); 
        }

        public abstract void StepCycle();
    }
}