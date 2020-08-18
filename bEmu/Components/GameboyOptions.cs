using System.ComponentModel;
using bEmu.Components;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Classes
{
    public class GameboyOptions : Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; set; }

        public GameboyOptions(Options options)
        {
            foreach (var property in options.GetType().GetProperties())
                property.SetValue(this, property.GetValue(options));
        }
    }
}