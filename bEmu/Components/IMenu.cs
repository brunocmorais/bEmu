using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace bEmu.Components
{
    public interface IMenu : IDrawable
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void Update(GameTime gameTime);
        bool IsSelectable { get; set; }
    }
}