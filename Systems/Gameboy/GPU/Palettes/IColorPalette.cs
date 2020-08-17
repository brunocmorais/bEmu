namespace bEmu.Systems.Gameboy.GPU.Palettes
{
    public interface IColorPalette
    {
        uint Shade0 { get; }
        uint Shade1 { get; }
        uint Shade2 { get; }
        uint Shade3 { get; }
    }
}