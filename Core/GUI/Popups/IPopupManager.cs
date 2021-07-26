using System;

namespace bEmu.Core.GUI.Popups
{
    public interface IPopupManager : IManager<IPopup>
    {
        void ShowErrorDialog(string title, string text, Exception ex);
    }
}