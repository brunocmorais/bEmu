using bEmu.Core.GUI.Menus;

namespace bEmu.Core.GUI
{
    public interface IDrawer
    {
        void Draw(IMenu menu);
        void Draw(IOSD osd);
    }
}