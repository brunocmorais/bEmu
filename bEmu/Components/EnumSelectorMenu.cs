using System;
using System.Collections.Generic;
using bEmu.Classes;
using bEmu.Systems;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using bEmu.Core.Extensions;

namespace bEmu.Components
{
    public class EnumSelectorMenu<T> : Menu where T : Enum
    {
        private readonly Action<T> action;
        public EnumSelectorMenu(IMainGame game, Action<T> action) : base(game)
        {
            this.action = action;
        }

        public override string Title => "Selecione uma opção";

        protected override IEnumerable<MenuOption> GetMenuOptions()
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Select(x => new MenuOption(x.GetEnumDescription(), null, typeof(void), (_) => action(x)));
        }
    }
}