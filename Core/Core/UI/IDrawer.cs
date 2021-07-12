using bEmu.Core.Components;
using bEmu.Core.UI.Menus;

namespace bEmu.Core.UI
{
    public interface IDrawer
    {
        void Draw(IMenu menu);
        void Draw(IOSD osd);
    }
}