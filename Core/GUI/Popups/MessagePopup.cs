namespace bEmu.Core.GUI.Popups
{
    public class MessagePopup : Popup
    {
        public MessagePopup(string title, string text, int width, int height, int screenX, int screenY, params IButton[] buttons) :
            base(width, height, GetCenterPosition(width, screenX), GetCenterPosition(height, screenY), text, title, buttons)
        {
        }
    }
}