using System;

namespace bEmu.Core.Video
{

    public class FrameBuffer : IFrameBuffer
    {
        public virtual byte[] Data { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public FrameBuffer(int width, int height)
        {
            SetSize(width, height);
        }

        public void Reset()
        {
            Data = new byte[Width * Height * 4];
            
            for (int i = 3; i < Data.Length; i += 4)
                Data[i] = 0xFF;
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
                return (uint)((Data[start] << 24) | (Data[start + 1] << 16) | (Data[start + 2] << 8) | (Data[start + 3]));
            }
            set
            {
                int start = GetIndex(x, y);
                Data[start] = (byte)((value & 0xFF000000) >> 24);
                Data[start + 1] = (byte)((value & 0x00FF0000) >> 16);
                Data[start + 2] = (byte)((value & 0x0000FF00) >> 8);
                Data[start + 3] = (byte)((value & 0x000000FF));
            }
        }

        public void SetScaledPixel(ScaledPixel pixel, int x, int y)
        {
            int scale = pixel.Scale;

            for (int i = 0; i < scale; i++)
                for (int j = 0; j < scale; j++)
                    this[i + (x * scale), j + (y * scale)] = pixel[i, j];
        }

        public void SetData(byte[] data)
        {
            for (int i = 0; i < Data.Length; i++)
                Data[i] = data[i];
        }
    }
}