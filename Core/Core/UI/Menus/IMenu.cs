using System.Collections.Generic;

namespace bEmu.Core.UI.Menus
{
    public interface IMenu
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void Update(double totalMilliseconds);
        void UpdateMenuOptions();
        bool IsSelectable { get; set; }
        int Width { get; }
        int Height { get; }
        MenuOption[] MenuOptions { get; }
        int SelectedOption { get; }
    }
}