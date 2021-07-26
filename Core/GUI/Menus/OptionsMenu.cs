using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Extensions;

namespace bEmu.Core.GUI.Menus
{
    public abstract class OptionsMenu : Menu
    {
        public OptionsMenu(IMain game) : base(game)
        {
            IsSelectable = true;
        }

        private string GetTextFromOptionValue(object value)
        {
            if (value == null)
                return string.Empty;

            Type type = value.GetType();

            if (type == typeof(int))
                return ((int)value).ToString();
            else if (type == typeof(bool))
                return ((bool)value) ? "Sim" : "Não";
            else if (type.IsEnum)
                return (value as Enum).GetEnumDescription();

            return string.Empty;
        }

        protected IEnumerable<MenuOption> GetOptions(PropertyInfo[] properties)
        {
            foreach (var prop in properties.OrderBy(x => x.Name))
            {
                var attrs = prop.GetCustomAttributes(typeof(DescriptionAttribute), true);

                if (!attrs.Any())
                    continue;
                    
                var attr = attrs[0] as DescriptionAttribute;
                var text = attr.Description;
                var propType = prop.PropertyType;
                var name = prop.Name;
                var value = GetTextFromOptionValue(prop.GetValue(game.Options));

                if (!string.IsNullOrWhiteSpace(value))
                    text = $"{text}: {value}";

                yield return new MenuOption(text, name, propType, (inc) => game.Options.SetOption(name, inc.Value));
            }
        }
    }
}