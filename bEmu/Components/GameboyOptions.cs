using System.ComponentModel;
using bEmu.Components;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Classes
{
    public class GameboyOptions : Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; set; }

        public GameboyOptions(IMainGame game, Options options) : base(game)
        {
            foreach (var property in options.GetType().GetProperties())
                property.SetValue(this, property.GetValue(options));
        }

        public override void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            base.OptionChangedEvent(sender, e);

            switch (e.Property)
            {
                case "PaletteType":
                    (Game.GameSystem.System.PPU as Systems.Gameboy.GPU.GPU).SetShadeColorPalette(PaletteType);
                    break;
            }
        }
    }
}