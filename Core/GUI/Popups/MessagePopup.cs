namespace bEmu.Core.GUI.Popups
{
    public class MessagePopup : Popup
    {
        public MessagePopup(IMain game, PopupSize size, string text, string title, params IButton[] buttons) : 
            base(game, size, text, title, buttons)
        {
        }
    }
}