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

        public static IFrameBuffer From(Bitmap bitmap)
        {
            var frameBuffer = new FrameBuffer(bitmap.Width, bitmap.Height);

            for (int i = 0; i < frameBuffer.Width; i++)
                for (int j = 0; j < frameBuffer.Height; j++)
                    frameBuffer[i, j] = bitmap[i, j];

            return frameBuffer;
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

        public virtual Pixel this[int x, int y]
        {
            get
            {
                int start = GetIndex(x, y);
                return ((uint)((Data[start] << 24) | (Data[start + 1] << 16) | (Data[start + 2] << 8) | (Data[start + 3])));
            }
            set
            {
                var num = value.ToUInt();
                int start = GetIndex(x, y);
                Data[start] = (byte)((num & 0xFF000000) >> 24);
                Data[start + 1] = (byte)((num & 0x00FF0000) >> 16);
                Data[start + 2] = (byte)((num & 0x0000FF00) >> 8);
                Data[start + 3] = (byte)((num & 0x000000FF));
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