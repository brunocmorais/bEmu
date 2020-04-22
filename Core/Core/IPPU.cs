namespace bEmu.Core
{
    public interface IPPU
    {
        ISystem System { get; }
        int Width { get; }
        int Height { get; }
        byte[] FrameBuffer { get; }
        uint GetPixel(int x, int y);
        void SetPixel(int x, int y, uint color);
    }
}