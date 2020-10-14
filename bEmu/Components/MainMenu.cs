using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Classes;
using bEmu.Core.Extensions;
using bEmu.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Components
{
    public class MainMenu : Menu
    {
        public override string Title => "bEmu";

        public MainMenu(IMainGame game) : base(game) { }

        protected override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("Carregar jogo", null, typeof(void), 
                (_) => game.Menu.OpenMenu(new FileSelectorMenu(game, (file) => SelectSystem(file))));
            yield return new MenuOption("Carregar estado", null, typeof(void), (_) => game.LoadState());
            yield return new MenuOption("Salvar estado", null, typeof(void), (_) => game.SaveState());
            yield return new MenuOption("Reiniciar", null, typeof(void), (_) => game.ResetGame());
            yield return new MenuOption("Fechar jogo", null, typeof(void), (_) => game.CloseGame());

            var type = game.Options.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = prop.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                var text = attr.Description;
                var propType = prop.PropertyType;
                var name = prop.Name;
                var value = GetTextFromOptionValue(prop.GetValue(game.Options));

                if (!string.IsNullOrWhiteSpace(value))
                    text = $"{text}: {value}";

                yield return new MenuOption(text, name, propType, (inc) => game.Options.SetOption(name, inc.Value));
            }

            yield return new MenuOption("Sair", null, typeof(void), (_) => game.StopGame());
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

        private void SelectSystem(string file)
        {
            game.Menu.OpenMenu(new EnumSelectorMenu<SupportedSystems>(game, (system) => game.LoadGame(system, file)));
        }
    }
}