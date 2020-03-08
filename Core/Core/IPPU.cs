namespace bEmu.Core
{
    public interface IPPU
    {
        ISystem System { get; }
        Pixel this[int x, int y] { get; set; }
        int Width { get; }
        int Height { get; }
    }
}