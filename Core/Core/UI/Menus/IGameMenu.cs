using System.Collections.Generic;

namespace bEmu.Core.UI.Menus
{
    public interface IGameMenu
    {
        Stack<IMenu> Menus { get; }
        IMenu Current { get; }
        bool IsOpen { get; }

        void CloseAll();
        void CloseCurrentMenu();
        void OpenDebugger();
        void OpenMainMenu();
        void OpenMenu(IMenu menu);
        void ShowAbout();
        void Update(double totalMilliseconds);
    }
}