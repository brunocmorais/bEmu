using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;

namespace bEmu.Core.GUI.Menus
{
    public class MainMenu : Menu
    {
        private readonly FileSelectorMenu romSelectorMenu;

        public override string Title => "bEmu";

        public MainMenu(IMain game) : base(game) 
        { 
            romSelectorMenu = new FileSelectorMenu(game, (file) => SelectSystem(file));
            IsSelectable = true;
        }

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("Carregar jogo", (_) => game.MenuManager.Open(romSelectorMenu));
            yield return new MenuOption("Carregar estado", (_) => game.LoadState());
            yield return new MenuOption("Salvar estado", (_) => game.SaveState());
            yield return new MenuOption("Reiniciar", (_) => game.ResetGame());
            yield return new MenuOption("Fechar jogo", (_) => game.CloseGame());

            var type = game.Options.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(x => x.Name))
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

            yield return new MenuOption("Debugger", (_) => game.MenuManager.OpenDebugger());
            yield return new MenuOption("Sobre", (_) => game.MenuManager.ShowAbout());
            yield return new MenuOption("Sair", (_) => game.StopGame());
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
            game.MenuManager.Open(new EnumSelectorMenu<SystemType>(game, (system) => game.LoadSystem(system, file)));
        }
    }
}