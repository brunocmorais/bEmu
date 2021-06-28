using System;
using System.ComponentModel;
using bEmu.Core.Scalers;

namespace bEmu.Core.Components
{
    public class Options
    {

        [Description("Pular quadros")]
        [Range(0, 9)]
        public int Frameskip { get; set; }
        
        [Description("Exibir FPS")]
        public bool ShowFPS { get; set; }

        [Description("Scaler")]
        public Scaler Scaler { get; set; }

        [Description("Tamanho")]
        [Range(1, 10)]
        public int Size { get; set; }

        [Description("Ativar Som")]
        public bool EnableSound { get; set; }

        public event EventHandler<OnOptionChangedEventArgs> OptionChanged;

        public Options(EventHandler<OnOptionChangedEventArgs> eventHandler, Options options)
        {
            if (options != null)
                foreach (var property in typeof(Options).GetProperties())
                    property.SetValue(this, property.GetValue(options));

            EnableSound = true;
            OptionChanged += eventHandler;
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

                if (attr == null || (!increment && currentValue > (int) attr.Minimum) || (increment && currentValue < (int) attr.Maximum))
                    property.SetValue(this, currentValue + (increment ? 1 : -1));
            }
            else if (propType == typeof(bool))
                property.SetValue(this, !(bool) property.GetValue(this));
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
    }

    public class OnOptionChangedEventArgs : EventArgs
    {
        public string Property { get; set; }
    }
}