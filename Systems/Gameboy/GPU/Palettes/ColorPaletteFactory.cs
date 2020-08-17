using System;
using System.Collections.Generic;

namespace bEmu.Systems.Gameboy.GPU.Palettes
{
    public static class ColorPaletteFactory
    {
        private static readonly Dictionary<MonochromePaletteType, IColorPalette> palettes;

        static ColorPaletteFactory()
        {
            Array array = Enum.GetValues(typeof(MonochromePaletteType));
            palettes = new Dictionary<MonochromePaletteType, IColorPalette>(array.Length);

            foreach (var item in array)
            {
                IColorPalette palette;
                var paletteType = (MonochromePaletteType)item;

                switch (paletteType)
                {
                    case MonochromePaletteType.Gray:       
                        palette = new GrayColorPalette(); break;
                    case MonochromePaletteType.LightGreen: 
                        palette = new LightGreenColorPalette(); break;
                    default:
                        continue;
                }

                palettes.Add(paletteType, palette);
            }
        }

        public static IColorPalette Get(MonochromePaletteType paletteType)
        {
            return palettes[paletteType];
        }
    }
}