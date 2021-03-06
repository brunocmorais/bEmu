using System.ComponentModel;
using bEmu.Core.System;
using bEmu.Core.GUI;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy
{
    public class Options : Core.System.Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; protected set; }

        public Options(IMain game, int size) : base(game, size)
        {
        }

        public override void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            base.OptionChangedEvent(sender, e);

            var options = sender as Options;

            switch (e.Property)
            {
                case nameof(options.PaletteType):
                    (Game.System as Systems.Gameboy.System).SetColorPalette(options.PaletteType);
                    break;
            }
        }
    }
}