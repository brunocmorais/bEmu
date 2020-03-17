namespace bEmu.Core
{

    public abstract class PPU : IPPU
    {
        public ISystem System { get; }
        public int Width { get; }
        public int Height { get; }

        public PPU(ISystem system, int width, int height)
        {
            System = system;
            Width = width;
            Height = height;
        }

        public abstract Pixel this[int x, int y] { get; }
    }
}