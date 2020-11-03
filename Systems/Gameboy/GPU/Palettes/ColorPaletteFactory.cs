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
                        palette = new Gray(); break;
                    case MonochromePaletteType.LightGreen: 
                        palette = new LightGreen(); break;
                    case MonochromePaletteType.Brown:
                        palette = new Brown(); break;
                    case MonochromePaletteType.Light:
                        palette = new Light(); break;
                    case MonochromePaletteType.Kiosk:
                        palette = new Kiosk(); break;
                    case MonochromePaletteType.SuperGameboy:
                        palette = new SuperGameboy(); break;
                    case MonochromePaletteType.Green:
                        palette = new Green(); break;
                    case MonochromePaletteType.Yellow:
                        palette = new Yellow(); break;
                    case MonochromePaletteType.Red:
                        palette = new Red(); break;
                    case MonochromePaletteType.Blue:
                        palette = new Blue(); break;
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

    class Gray : IColorPalette
    {
        public uint Shade0 => 0xFFFFFFFF;
        public uint Shade1 => 0xC0C0C0FF;
        public uint Shade2 => 0x606060FF;
        public uint Shade3 => 0x000000FF;
    }

    class LightGreen : IColorPalette
    {
        public uint Shade0 => 0x9BBC0FFF;
        public uint Shade1 => 0x8BAC0FFF;
        public uint Shade2 => 0x306230FF;
        public uint Shade3 => 0x0F380FFF;
    }

    class Brown : IColorPalette
    {
        public uint Shade0 => 0xF8E088FF;
        public uint Shade1 => 0xD8B058FF;
        public uint Shade2 => 0x987838FF;
        public uint Shade3 => 0x483818FF;
    }

    class Light : IColorPalette
    {
        public uint Shade0 => 0x00B581FF;
        public uint Shade1 => 0x009A71FF;
        public uint Shade2 => 0x00694AFF;
        public uint Shade3 => 0x004F3BFF;
    }

    class Kiosk : IColorPalette
    {
        public uint Shade0 => 0xECEDB0FF;
        public uint Shade1 => 0xBBBB18FF;
        public uint Shade2 => 0x6B6E00FF;
        public uint Shade3 => 0x103700FF;
    }

    class SuperGameboy : IColorPalette
    {
        public uint Shade0 => 0xFFEFCEFF;
        public uint Shade1 => 0xDE944AFF;
        public uint Shade2 => 0xAD2921FF;
        public uint Shade3 => 0x311852FF;
    }

    class Green : IColorPalette
    {
        public uint Shade0 => 0x50D050FF;
        public uint Shade1 => 0x40A040FF;
        public uint Shade2 => 0x307030FF;
        public uint Shade3 => 0x204020FF;
    }

    class Yellow : IColorPalette
    {
        public uint Shade0 => 0xF8F078FF;
        public uint Shade1 => 0xB0A848FF;
        public uint Shade2 => 0x686830FF;
        public uint Shade3 => 0x202010FF;
    }

    class Red : IColorPalette
    {
        public uint Shade0 => 0xFFC0C0FF;
        public uint Shade1 => 0xFF6060FF;
        public uint Shade2 => 0xC00000FF;
        public uint Shade3 => 0x600000FF;
    }

    class Blue : IColorPalette
    {
        public uint Shade0 => 0xC0C0FFFF;
        public uint Shade1 => 0x5F60FFFF;
        public uint Shade2 => 0x0000C0FF;
        public uint Shade3 => 0x000060FF;
    }
}