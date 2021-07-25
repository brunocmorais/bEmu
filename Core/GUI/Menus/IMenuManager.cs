namespace bEmu.Core.GUI.Menus
{
    public interface IMenuManager : IManager<IMenu>
    {
        void OpenDebugger();
        void OpenMainMenu();
        void ShowAbout();
    }
}