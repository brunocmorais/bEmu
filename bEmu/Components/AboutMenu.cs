using System.Collections.Generic;
using System.Linq;

namespace bEmu.Components
{
    public class AboutMenu : Menu
    {
        public AboutMenu(IMainGame game) : base(game)
        {
            IsSelectable = false;
        }

        public override string Title => "Sobre o bEmu";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            const string space = " ";
            yield return new MenuOption(space);
            yield return new MenuOption("Desenvolvido por");
            yield return new MenuOption(space);
            yield return new MenuOption("Bruno Costa de Morais");
            yield return new MenuOption(space);
            yield return new MenuOption("v1.0");
        }
    }
}