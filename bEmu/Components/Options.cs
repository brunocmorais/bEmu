using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using bEmu.Core.Scalers;

namespace bEmu.Components
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

        public event EventHandler<OnOptionChangedEventArgs> OptionChanged;

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

            OnOptionChanged(new OnOptionChangedEventArgs() { Property = property.Name });
        }

        protected virtual void OnOptionChanged(OnOptionChangedEventArgs e)
        {
            EventHandler<OnOptionChangedEventArgs> handler = OptionChanged;

            if (handler != null)
                handler(this, e);
        }
    }

    public class OnOptionChangedEventArgs : EventArgs
    {
        public string Property { get; set; }
    }
}