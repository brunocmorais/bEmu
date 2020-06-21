using System;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class Palette
    {
        private readonly uint[] palette;
        public PaletteType Type { get; set; }
        private readonly MMU mmu;

        public Palette(MMU mmu)
        {
            palette = new uint[8];
            this.mmu = mmu;
        }

        public uint GetShadeFrom(int colorNumber)
        {
            byte val = 0;

            switch (Type)
            {
                case PaletteType.BGP: val = mmu.State.LCD.BGP; break;
                case PaletteType.OBP0: val = mmu.State.LCD.OBP0; break;
                case PaletteType.OPB1: val = mmu.State.LCD.OBP1; break;
            }

            int bitOffset = (colorNumber * 2);
            var shadeNumber = ((val & (3 << bitOffset)) >> bitOffset);
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

        public uint GetShadeFrom(PaletteType type, int colorNumber)
        {
            byte val = 0;

            switch (type)
            {
                case PaletteType.BGP: val = mmu.State.LCD.BGP; break;
                case PaletteType.OBP0: val = mmu.State.LCD.OBP0; break;
                case PaletteType.OPB1: val = mmu.State.LCD.OBP1; break;
            }

            int bitOffset = (colorNumber * 2);
            var shadeNumber = ((val & (3 << bitOffset)) >> bitOffset);
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

        public uint GetColorFrom(int colorNumber)
        {
            int index = GetIndexFromPalette(Type);
            byte firstByte, secondByte;
            int pos = index + (colorNumber * 2);

            if (IsBackgroundPalette)
            {
                firstByte = mmu.ColorPaletteData.BackgroundPalettes[pos];
                secondByte = mmu.ColorPaletteData.BackgroundPalettes[pos + 1];
            }
            else
            {
                firstByte = mmu.ColorPaletteData.SpritePalettes[pos];
                secondByte = mmu.ColorPaletteData.SpritePalettes[pos + 1];
            }

            return ColorToRGB((ushort)((secondByte << 8) | firstByte));
        }

        private static uint ColorToRGB(ushort color)
        {
            int r = ((((color & 0x001F) >>  0)) * 8);
            int g = ((((color & 0x03E0) >>  5)) * 8);
            int b = ((((color & 0x7C00) >> 10)) * 8);

            return (uint)((r << 24) | (g << 16) | (b << 8) | 0xFF);
        }

        private static int GetIndexFromPalette(PaletteType type)
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

        public void UpdatePaletteFromBytes(byte byte1, byte byte2)
        {
            for (int i = 0; i < 8; i++)
            {
                byte colorNumber = 0;
                int v1 = (1 << (7 - i));
                
                if ((byte2 & v1) == v1)
                    colorNumber += 2;

                if ((byte1 & v1) == v1)
                    colorNumber += 1;

                if (IsBackgroundPalette || colorNumber > 0)
                    palette[i] = IsColorPalette ? GetColorFrom(colorNumber) : GetShadeFrom(colorNumber);
                else
                    palette[i] = 0;
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

        public uint this[int index] => palette[index];
        public int Length => palette.Length;
    }
}