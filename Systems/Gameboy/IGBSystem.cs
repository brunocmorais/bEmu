using bEmu.Core.System;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy
{
    public interface IGBSystem : IRunnableSystem
    {
        bool GBCMode { get; }
        IColorPalette ColorPalette { get; }
        bool DoubleSpeedMode { get; }
        void SetColorPalette(MonochromePaletteType type);
    }
}