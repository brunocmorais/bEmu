namespace bEmu.Core.Video
{
    public interface IFrameBuffer
    {
        uint this[int x, int y] { get; set; }

        byte[] Data { get; }
        int Width { get; }
        int Height { get; }

        void Reset();
        void SetData(byte[] data);
        void SetScaledPixel(ScaledPixel pixel, int x, int y);
        void SetSize(int width, int height);
    }
}