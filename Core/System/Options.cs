using System;
using System.ComponentModel;
using bEmu.Core.Attributes;
using bEmu.Core.Enums;
using bEmu.Core.GUI;
using bEmu.Core.Util;

namespace bEmu.Core.System
{

    public class Options : IOptions
    {

        [Description("Pular quadros")]
        [Range(0, 9)]
        public int Frameskip { get; protected set; }

        [Description("Exibir FPS")]
        public bool ShowFPS { get; protected set; }

        [Description("Scaler")]
        public ScalerType Scaler { get; protected set; }

        [Description("Tamanho")]
        [Range(1, 10)]
        public int Size { get; protected set; }

        [Description("Ativar Som")]
        public bool EnableSound { get; protected set; }
        protected readonly IMain Game;
        protected event EventHandler<OnOptionChangedEventArgs> OptionChanged;

        public Options(IMain game, int size)
        {
            Game = game;
            EnableSound = true;
            Init(size);
        }

        private void Init(int size)
        {
            if (Game.Options != null)
                foreach (var property in typeof(Options).GetProperties())
                    property.SetValue(this, property.GetValue(Game.Options));

            Size = size;
            OptionChanged += OptionChangedEvent;
        }

        public void SetOption(string optionName, bool increment)
        {
            var property = this.GetType().GetProperty(optionName);

            if (property == null)
                return;

            var propType = property.PropertyType;

            if (propType == typeof(int))
            {
                var attr = property.GetCustomAttributes(typeof(RangeAttribute), true)[0] as RangeAttribute;
                int currentValue = (int)property.GetValue(this);

                if (attr == null || (!increment && currentValue > (int)attr.Minimum) || (increment && currentValue < (int)attr.Maximum))
                    property.SetValue(this, currentValue + (increment ? 1 : -1));
            }
            else if (propType == typeof(bool))
                property.SetValue(this, !(bool)property.GetValue(this));
            else if (propType.IsEnum)
            {
                int currentValue = (int)property.GetValue(this);
                var array = Enum.GetValues(propType);

                if (currentValue == 0 && !increment)
                    property.SetValue(this, array.Length - 1);
                else if (currentValue == array.Length - 1 && increment)
                    property.SetValue(this, 0);
                else
                    property.SetValue(this, currentValue + (increment ? 1 : -1));
            }

            OptionChanged(this, new OnOptionChangedEventArgs() { Property = property.Name });
        }

        public virtual void OptionChangedEvent(object sender, OnOptionChangedEventArgs e)
        {
            var options = sender as Core.System.Options;

            switch (e.Property)
            {
                case nameof(options.ShowFPS):
                    Game.Osd.RemoveMessage(MessageType.FPS);

                    if (options.ShowFPS)
                        Game.Osd.InsertMessage(MessageType.FPS, string.Empty);

                    break;
                case nameof(options.Frameskip):
                    Game.System.Frameskip = options.Frameskip;
                    break;
                case nameof(options.Scaler):
                    Game.SetScaler();
                    break;
                case nameof(options.Size):
                    Game.SetScreenSize();
                    break;
                case nameof(options.EnableSound):
                    Game.SetSound(options.EnableSound);
                    break;
            }
        }
    }

    public class OnOptionChangedEventArgs : EventArgs
    {
        public string Property { get; set; }
    }
}