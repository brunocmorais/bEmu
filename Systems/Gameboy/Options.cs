using System;
using System.ComponentModel;
using bEmu.Core.Components;
using bEmu.Systems.Gameboy.GPU.Palettes;

namespace bEmu.Systems.Gameboy
{
    public class Options : Core.Components.Options
    {
        [Description("Paleta de cores")]
        public MonochromePaletteType PaletteType { get; set; }

        public Options(EventHandler<OnOptionChangedEventArgs> eventHandler, Core.Components.Options options) : base(eventHandler, options)
        {
            foreach (var property in options.GetType().GetProperties())
                property.SetValue(this, property.GetValue(options));
        }
    }
}