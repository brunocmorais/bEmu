using System.Collections.Generic;
using bEmu.Components;
using Microsoft.Xna.Framework;

namespace bEmu.Menus
{
    public interface IMenu : Components.IDrawable
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void Update(GameTime gameTime);
        void UpdateMenuOptions();

        bool IsSelectable { get; set; }
    }
}