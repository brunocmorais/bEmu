using System;
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
            foreach (var property in game.Options.GetType().GetProperties())
            {
                if (property.Name == nameof(Options.Size))
                    continue;

                property.SetValue(this, property.GetValue(game.Options));
            }

            OptionChanged += OptionChangedEvent;
        }

        public override void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            base.OptionChangedEvent(sender, e);

            var gameBoyOptions = sender as Systems.Gameboy.Options;

            switch (e.Property)
            {
                case nameof(gameBoyOptions.PaletteType):
                    (Game.System as Systems.Gameboy.System).SetColorPalette(gameBoyOptions.PaletteType);
                    break;
            }
        }
    }
}