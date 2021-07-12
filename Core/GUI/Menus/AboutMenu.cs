using System.Collections.Generic;

namespace bEmu.Core.GUI.Menus
{
    public class AboutMenu : Menu
    {
        const string Space = " ";
        
        public AboutMenu(IMain game) : base(game)
        {
            IsSelectable = false;
        }

        public override string Title => "Sobre o bEmu";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption(Space);
            yield return new MenuOption("Desenvolvido com <3 por");
            yield return new MenuOption(Space);
            yield return new MenuOption("Bruno Costa de Morais");
            yield return new MenuOption(Space);
            yield return new MenuOption("v0.1a");
        }
    }
}