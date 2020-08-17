using System.ComponentModel;
using bEmu.Components;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Classes
{
    public class GameboyOptions : Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; set; }
    }
}