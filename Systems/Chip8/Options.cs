using System.ComponentModel;
using bEmu.Core.GUI;
using bEmu.Core.System;

namespace bEmu.Systems.Chip8
{
    public class Options : Core.System.Options
    {
        [Description("Velocidade de execução")]
        public ExecutionSpeed Speed { get; protected set; }
        public Options(IMain game, int size) : base(game, size)
        {
        }

        public override void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            base.OptionChangedEvent(sender, e);

            var options = sender as Options;

            switch (e.Property)
            {
                case nameof(options.Speed):
                    (Game.System as Systems.Chip8.System).Speed = options.Speed;
                    break;
            }
        }
    }
}