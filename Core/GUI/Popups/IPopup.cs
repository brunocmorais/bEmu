using System.Collections.Generic;

namespace bEmu.Core.GUI.Popups
{
    public interface IPopup : IDrawable
    {
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }
        string Text { get; }
        string Title { get; }
        IList<IButton> Buttons { get; }
        IButton SelectedButton { get; }
        bool Closed { get; set; }

        void SelectLeftButton();
        void SelectRightButton();
    }
}