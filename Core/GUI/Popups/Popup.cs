using System.Collections.Generic;
using System.Linq;

namespace bEmu.Core.GUI.Popups
{
    public abstract class Popup : IPopup
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public string Text { get; }
        public string Title { get; }
        public IList<IButton> Buttons { get; }
        public IButton SelectedButton => Buttons[selectedButtonIndex];
        public bool Closed { get; set; }
        private int selectedButtonIndex = 0;
        private readonly IMain game;
        private readonly PopupSize size;

        public Popup(IMain game, PopupSize size, string text, string title, params IButton[] buttons)
        {
            this.game = game;
            this.size = size;
            Text = text;
            Title = title;
            Buttons = buttons.ToList();

            Update();
        }

        public virtual void SelectRightButton()
        {
            selectedButtonIndex = (selectedButtonIndex + 1) % Buttons.Count;
        }

        public virtual void SelectLeftButton()
        {
            int value = (selectedButtonIndex - 1);
            selectedButtonIndex = value < 0 ? Buttons.Count - 1 : value;
        }

        public virtual void Update()
        {
            float widthFactor = 0, heightFactor = 0;

            switch (size)
            {
                case PopupSize.Small:
                    widthFactor = 0.5f; heightFactor = 0.25f; break;
                case PopupSize.Medium:
                    widthFactor = 0.8f; heightFactor = 0.4f; break;
                case PopupSize.Large:
                    widthFactor = 0.9f; heightFactor = 0.9f; break;
            }

            int screenWidth = game.Width * game.Options.Size;
            int screenHeight = game.Height * game.Options.Size;

            Width = (int)(widthFactor * screenWidth);
            Height = (int)(heightFactor * screenHeight);
            X = screenWidth / 2 - (Width / 2);
            Y = screenHeight / 2 - (Height / 2);
        }
    }
}