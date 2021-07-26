using System.Collections.Generic;

namespace bEmu.Core.GUI.Menus
{
    public interface IMenu : IDrawable
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void UpdateControls(double totalMilliseconds);
        void UpdateMenuOptions();

        bool IsSelectable { get; set; }
        int Width { get; }
        int Height { get; }
        MenuOption[] MenuOptions { get; }
        int SelectedOption { get; }
    }
}