using System;
using System.Collections.Generic;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy.GPU
{
    public class Palette
    {

        public const int PaletteSize = 8;
        public PaletteType Type { get; }
        public int Address { get; }
        public uint this[int index] => palette[index];
        public int Length => palette.Length;
        private readonly uint[] palette;

        public Palette(PaletteType type, int address)
        {
            palette = new uint[PaletteSize];
            Type = type;
            Address = address;
        }        

        private bool IsColorPalette()
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

        private bool IsBackgroundPalette()
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

        private uint GetColorFrom(int colorNumber, ColorPaletteData colorPaletteData)
        {
            byte firstByte, secondByte;
            int pos = Util.GetIndexFromPalette(Type) + (colorNumber * 2);

            if (IsBackgroundPalette())
            {
                firstByte = colorPaletteData.BackgroundPalettes[pos];
                secondByte = colorPaletteData.BackgroundPalettes[pos + 1];
            }
            else
            {
                firstByte = colorPaletteData.SpritePalettes[pos];
                secondByte = colorPaletteData.SpritePalettes[pos + 1];
            }

            return Util.ColorToRGB((ushort)((secondByte << 8) | firstByte));
        }

        private uint GetShadeFrom(IColorPalette colorPalette, int colorNumber, MonochromePaletteData data)
        {
            byte val = 0;

            switch (Type)
            {
                case PaletteType.BGP: val = data.BGP; break;
                case PaletteType.OBP0: val = data.OBP0; break;
                case PaletteType.OPB1: val = data.OBP1; break;
            }

            return Util.ShadeToRGB(colorPalette, val, colorNumber);
        }

        public void SetColors(IPaletteData data, ushort tileData, bool horizontalFlip, IColorPalette colorPalette)
        {
            for (int i = 0; i < Palette.PaletteSize; i++)
            {
                byte colorNumber = 0;
                int mask1 = (1 << (7 - i));
                int mask2 = (1 << (15 - i));
                int x = horizontalFlip ? (7 - i) : i;
                
                if ((tileData & mask2) == mask2)
                    colorNumber += 2;

                if ((tileData & mask1) == mask1)
                    colorNumber += 1;

                if (IsBackgroundPalette() || colorNumber > 0)
                {
                    if (IsColorPalette())
                        palette[x] = GetColorFrom(colorNumber, data as ColorPaletteData);
                    else
                        palette[x] = GetShadeFrom(colorPalette, colorNumber, data as MonochromePaletteData);
                }
                else
                    palette[x] = 0;
            }
        }
    }
}