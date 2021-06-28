using System.Collections.Generic;
using bEmu.Components;
using Microsoft.Xna.Framework;

namespace bEmu.Menus
{
    public interface IMenu
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void Update(GameTime gameTime);
        void UpdateMenuOptions();
        void Draw();
        bool IsSelectable { get; set; }
    }
}