using System.Collections.Generic;

namespace bEmu.Core.GUI.Menus
{
    public interface IMenu
    {
        string Title { get; }
        IEnumerable<MenuOption> GetMenuOptions();
        void Update(double totalMilliseconds);
        void UpdateMenuOptions();
        void UpdateSize();

        bool IsSelectable { get; set; }
        int Width { get; }
        int Height { get; }
        MenuOption[] MenuOptions { get; }
        int SelectedOption { get; }
    }
}