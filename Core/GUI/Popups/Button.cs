using System;

namespace bEmu.Core.GUI.Popups
{
    public class Button : IButton
    {
        public string Text { get; }
        public Action Action { get; }

        public Button(string text, Action action) : this(text)
        {
            Action = action;
        }

        public Button(string text)
        {
            Text = text;
        }
    }
}