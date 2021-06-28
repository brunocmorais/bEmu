using System;
using System.ComponentModel;
using bEmu.Core.Components;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy
{
    public class Options : Core.Components.Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; protected set; }

        public Options(EventHandler<OnOptionChangedEventArgs> eventHandler, IOptions options, int size) : base(eventHandler, options, size)
        {
            foreach (var property in options.GetType().GetProperties())
            {
                if (property.Name == nameof(Options.Size))
                    continue;

                property.SetValue(this, property.GetValue(options));
            }
        }
    }
}