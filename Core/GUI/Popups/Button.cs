using System;

namespace bEmu.Core.GUI.Popups
{
    public class Button : IButton
    {
        public string Text { get; }
        public Action Action { get; }
        public bool Close { get; }

        public Button(string text, bool close, Action action) : this(text, close)
        {
            Action = action;
        }

        public Button(string text, bool close)
        {
            Text = text;
            Close = close;
        }
    }
}