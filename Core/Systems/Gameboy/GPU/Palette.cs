using System;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class Palette
    {
        public const int Size = 8;
        private readonly uint[] palette;
        public PaletteType Type { get; set; }
        public uint this[int index]
        {
            get => palette[index];
            set => palette[index] = value;
        }
        public int Length => Size;
        public int Address { get; set; }
        public static readonly PaletteType[] BackgroundPalettes = new[] 
        {
            PaletteType.BG0, PaletteType.BG1, PaletteType.BG2, PaletteType.BG3, 
            PaletteType.BG4, PaletteType.BG5, PaletteType.BG6, PaletteType.BG7
        };

        public Palette()
        {
            palette = new uint[Size];
        }

        public static uint ColorToRGB(ushort color)
        {
            int r = ((((color & 0x001F) >>  0)) * 8);
            int g = ((((color & 0x03E0) >>  5)) * 8);
            int b = ((((color & 0x7C00) >> 10)) * 8);

            return (uint)((r << 24) | (g << 16) | (b << 8) | 0xFF);
        }

        public static uint ShadeToRGB(byte paletteBytes, int colorNumber)
        {
            int bitOffset = (colorNumber * 2);
            var shadeNumber = ((paletteBytes & (3 << bitOffset)) >> bitOffset);
            int color;
            
            switch (shadeNumber)
            {
                case 0: color = 0xFF; break;
                case 1: color = 0xC0; break;
                case 2: color = 0x60; break;
                case 3: color = 0x00; break;
                default: throw new Exception();
            }

            return (uint)((color << 24) | (color << 16) | (color << 8) | 0xFF);
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

        public bool IsColorPalette
        {
            get
            {
                switch (Type)
                {
                    case PaletteType.BGP:
                    case PaletteType.OBP0:
                    case PaletteType.OPB1:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public bool IsBackgroundPalette
        {
            get
            {
                switch (Type)
                {
                    case PaletteType.BG0:
                    case PaletteType.BG1:
                    case PaletteType.BG2:
                    case PaletteType.BG3:
                    case PaletteType.BG4:
                    case PaletteType.BG5:
                    case PaletteType.BG6:
                    case PaletteType.BG7:
                    case PaletteType.BGP:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}