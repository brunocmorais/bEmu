using System;
using System.Collections.Generic;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy.GPU
{
    public static class Util
    {
        public static readonly PaletteType[] BackgroundPalettes = new[] 
        {
            PaletteType.BG0, PaletteType.BG1, PaletteType.BG2, PaletteType.BG3, 
            PaletteType.BG4, PaletteType.BG5, PaletteType.BG6, PaletteType.BG7
        };

        public static uint ColorToRGB(ushort color)
        {
            int r = ((((color & 0x001F) >>  0)) * 8);
            int g = ((((color & 0x03E0) >>  5)) * 8);
            int b = ((((color & 0x7C00) >> 10)) * 8);

            return (uint)((r << 24) | (g << 16) | (b << 8) | 0xFF);
        }

        public static uint ShadeToRGB(IColorPalette colorPalette, byte paletteBytes, int colorNumber)
        {
            int bitOffset = (colorNumber * 2);
            var shadeNumber = ((paletteBytes & (3 << bitOffset)) >> bitOffset);

            switch (shadeNumber)
            {
                case 0: return colorPalette.Shade0;
                case 1: return colorPalette.Shade1;
                case 2: return colorPalette.Shade2;
                case 3: return colorPalette.Shade3;
                default: throw new Exception();
            }
        }

        public static int GetIndexFromPalette(PaletteType type)
        {
            switch (type)
            {
                case PaletteType.BG0:
                case PaletteType.OBJ0:
                    return 0;
                case PaletteType.BG1:
                case PaletteType.OBJ1:
                    return 8;
                case PaletteType.BG2:
                case PaletteType.OBJ2:
                    return 16;
                case PaletteType.BG3:
                case PaletteType.OBJ3:
                    return 24;
                case PaletteType.BG4:
                case PaletteType.OBJ4:
                    return 32;
                case PaletteType.BG5:
                case PaletteType.OBJ5:
                    return 40;
                case PaletteType.BG6:
                case PaletteType.OBJ6:
                    return 48;
                case PaletteType.BG7:
                case PaletteType.OBJ7:
                    return 56;
                default:
                    throw new Exception();
            }
        }

        public static IEnumerable<uint> GetBGColors(ColorPaletteData colorPaletteData)
        {
            foreach (var paletteType in BackgroundPalettes)
            {
                int pos = GetIndexFromPalette(paletteType);
                var firstByte = colorPaletteData.BackgroundPalettes[pos];
                var secondByte = colorPaletteData.BackgroundPalettes[pos + 1];
                yield return ColorToRGB((ushort)((secondByte << 8) | firstByte));
            }
        }
    }
}