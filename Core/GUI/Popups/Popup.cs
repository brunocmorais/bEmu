using System.Collections.Generic;
using System.Linq;

namespace bEmu.Core.GUI.Popups
{
    public abstract class Popup : IPopup
    {
        public int Width { get; }
        public int Height { get; }
        public int X { get; }
        public int Y { get; }
        public string Text { get; }
        public string Title { get; }
        public IList<IButton> Buttons { get; }
        public IButton SelectedButton => Buttons[selectedButtonIndex];
        public bool Closed { get; set; }
        private int selectedButtonIndex = 0;

        public Popup(int width, int height, int x, int y, string text, string title, params IButton[] buttons)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Text = text;
            Title = title;
            Buttons = buttons.ToList();
        }

        public void SelectRightButton()
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % Buttons.Count;
        }

        public void SelectLeftButton()
        {
            int value = (selectedButtonIndex - 1);
            selectedButtonIndex = value < 0 ? Buttons.Count - 1 : value;
        }

        protected static int GetCenterPosition(int size, int screenSize)
        {
            return (screenSize / 2 - (size / 2));
        }
    }
}