using System;
using System.Collections.Generic;
using System.Linq;
using bEmu.Components;

namespace bEmu.Menus
{
    public class AboutMenu : Menu
    {
        const string Space = " ";
        
        public AboutMenu(IMainGame game) : base(game)
        {
            IsSelectable = false;
        }

        public override string Title => "Sobre o bEmu";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption(Space);
            yield return new MenuOption("Desenvolvido por");
            yield return new MenuOption(Space);
            yield return new MenuOption("Bruno Costa de Morais");
            yield return new MenuOption(Space);
            yield return new MenuOption("v1.0");
        }
    }
}