using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Extensions;

namespace bEmu.Core.GUI.Menus
{
    public class EnumSelectorMenu<T> : Menu where T : Enum
    {
        private readonly Action<T> action;
        public EnumSelectorMenu(IMain game, Action<T> action) : base(game)
        {
            this.action = action;
            IsSelectable = true;
        }

        public override string Title => "Selecione uma opção";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            return from item in Enum.GetValues(typeof(T)).Cast<T>()
                    where !item.IsIgnore()
                    select new MenuOption(item.GetEnumDescription(), (_) => action(item));
        }
    }
}