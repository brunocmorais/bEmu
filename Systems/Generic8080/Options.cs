using System.ComponentModel;
using bEmu.Core.GUI;
using bEmu.Core.System;

namespace bEmu.Systems.Generic8080
{
    public class Options : Core.System.Options
    {
        [Description("Cores customizadas")]
        public bool CustomColors { get; protected set; }

        [Description("Tema de fundo")]
        public bool UseBackdrop { get; protected set; }

        public Options(IMain game, int size) : base(game, size)
        {
            SetOption(nameof(CustomColors), false);
            SetOption(nameof(UseBackdrop), false);
        }

        public override void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            base.OptionChangedEvent(sender, e);

            var options = sender as Options;

            switch (e.Property)
            {
                case nameof(options.CustomColors):
                    (Game.System as Systems.Generic8080.System).SetCustomColors(options.CustomColors);
                    break;
                case nameof(options.UseBackdrop):
                    (Game.System as Systems.Generic8080.System).SetBackdrop(options.UseBackdrop);
                    break;
            }
        }
    }
}