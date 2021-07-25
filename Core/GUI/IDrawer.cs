using bEmu.Core.GUI.Menus;
using bEmu.Core.GUI.Popups;

namespace bEmu.Core.GUI
{
    public interface IDrawer
    {
        void Draw(IMenu menu);
        void Draw(IOSD osd);
        void Draw(IPopup popup);
    }
}