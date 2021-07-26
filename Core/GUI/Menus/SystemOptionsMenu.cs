using System.Collections.Generic;
using System.Reflection;
using bEmu.Core.System;

namespace bEmu.Core.GUI.Menus
{
    public class SystemOptionsMenu : OptionsMenu
    {
        public SystemOptionsMenu(IMain game) : base(game)
        {
        }

        public override string Title => "Opções do Sistema";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            var type = game.Options.GetType();

            if (type == typeof(Options))
                return new List<MenuOption>() { new MenuOption("Nenhuma opção para este sistema!") };

            return GetOptions(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }
    }
}