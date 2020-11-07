using System.ComponentModel;

namespace bEmu.Systems.Gameboy.GPU.Palettes
{
    public enum MonochromePaletteType
    {
        [Description("Cinza")]
        Gray = 0,
        [Description("Verde claro")]
        LightGreen = 1,
        [Description("Marrom")]
        Brown = 2,
        [Description("Light")]
        Light = 3,
        [Description("Kiosk")]
        Kiosk = 4,
        [Description("Super Gameboy")]
        SuperGameboy = 5,
        [Description("Verde")]
        Green = 6,
        [Description("Amarelo")]
        Yellow = 7,
        [Description("Vermelho")]
        Red = 8,
        [Description("Azul")]
        Blue = 9
    }
}