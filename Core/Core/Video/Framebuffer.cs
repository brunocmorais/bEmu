namespace bEmu.Core.Video
{
    public class Framebuffer
    {
        public virtual byte[] Data { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Framebuffer(int width, int height)
        {
            SetSize(width, height);
        }

        public void Reset()
        {
            Data = new byte[Width * Height * 4];
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            Reset();
        }

        protected int GetIndex(int x, int y) => (y * Width * 4) + (x * 4);

        public virtual uint this[int x, int y]
        {
            get 
            {
                int start = GetIndex(x, y);
                return (uint) ((Data[start] << 24) | (Data[start + 1] << 16) | (Data[start + 2] << 8) | (Data[start + 3])); 
            }
            set
            {
                int start = GetIndex(x, y);
                Data[start]     = (byte)((value & 0xFF000000) >> 24);
                Data[start + 1] = (byte)((value & 0x00FF0000) >> 16);
                Data[start + 2] = (byte)((value & 0x0000FF00) >> 8);
                Data[start + 3] = (byte)((value & 0x000000FF));
            }
        }

        public void SetPixel2x(Pixel2x pixel2x, int x, int y)
        {
            this[y + 0, x + 0] = pixel2x.P1;
            this[y + 1, x + 0] = pixel2x.P2;
            this[y + 0, x + 1] = pixel2x.P3;
            this[y + 1, x + 1] = pixel2x.P4;
        }

        public void SetPixel3x(Pixel3x pixel3x, int x, int y)
        {
            this[y + 0, x + 0] = pixel3x.P1;
            this[y + 1, x + 0] = pixel3x.P2;
            this[y + 2, x + 0] = pixel3x.P3;
            this[y + 0, x + 1] = pixel3x.P4;
            this[y + 1, x + 1] = pixel3x.P5;
            this[y + 2, x + 1] = pixel3x.P6;
            this[y + 0, x + 2] = pixel3x.P7;
            this[y + 1, x + 2] = pixel3x.P8;
            this[y + 2, x + 2] = pixel3x.P9;
        }

        public void SetData(byte[] data)
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = data[i];
        }
    }
}