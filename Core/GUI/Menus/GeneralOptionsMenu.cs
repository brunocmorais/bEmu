using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Extensions;
using bEmu.Core.System;

namespace bEmu.Core.GUI.Menus
{
    public class GeneralOptionsMenu : OptionsMenu
    {
        public GeneralOptionsMenu(IMain game) : base(game) 
        { 
        }

        public override string Title => "Opções Gerais";

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            return GetOptions(typeof(Options).GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }
    }
}