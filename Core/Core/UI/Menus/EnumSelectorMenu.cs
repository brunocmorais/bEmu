using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Components;
using bEmu.Core.Extensions;
using bEmu.Core.UI;

namespace bEmu.Core.UI.Menus
{
    public class EnumSelectorMenu<T> : Menu where T : Enum
    {
        private readonly Action<T> action;
        public EnumSelectorMenu(IMainGame game, Action<T> action) : base(game)
        {
            this.action = action;
            IsSelectable = true;
        }

        public override string Title => "Selecione uma opção";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Select(x => new MenuOption(x.GetEnumDescription(), (_) => action(x)));
        }
    }
}