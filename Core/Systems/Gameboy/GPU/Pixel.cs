namespace bEmu.Core.Systems.Gameboy.GPU
{
    public struct Pixel
    {
        public int Color { get; set; }
        public int Palette { get; set; }
        public bool SpritePriority { get; set; }
        public bool BackgroundPriority { get; set; }

        public Pixel(int color, int palette, bool spritePriority, bool backgroundPriority)
        {
            Color = color;
            Palette = palette;
            SpritePriority = spritePriority;
            BackgroundPriority = backgroundPriority;
        }
    }
}