namespace bEmu.Core
{
    public interface IPPU
    {
        ISystem System { get; }
        Pixel this[int x, int y] { get; }
        int Width { get; }
        int Height { get; }
    }
}