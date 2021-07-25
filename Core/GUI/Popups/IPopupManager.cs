using System;

namespace bEmu.Core.GUI.Popups
{
    public interface IPopupManager : IManager<IPopup>
    {
        IPopup BuildPopup(string title, string text, PopupSize popupSize);
        void ShowErrorDialog(string title, string text, Exception ex);
    }
}