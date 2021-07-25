using System;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;
using bEmu.Core.Input;

namespace bEmu.Core.GUI.Popups
{

    public class PopupManager : Manager<IPopup>, IPopupManager
    {
        public PopupManager(IMain game) : base(game)
        {
        }

        public override void Open(IPopup popup)
        {
            popup.Closed = false;
            base.Open(popup);
        }

        public override void Update(double totalMilliseconds)
        {
            if (!Items.Any())
                return;

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Enter))
            {
                if (Current.Buttons.Any())
                {
                    if (Current.SelectedButton.Action != null)
                        Current.SelectedButton.Action();

                    if (Current.Closed)
                        CloseCurrent();
                }
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Right))
                Current.SelectRightButton();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Left))
                Current.SelectLeftButton();
        }

        public IPopup BuildPopup(string title, string text, PopupSize popupSize)
        {
            int width = 0, height = 0;
            switch (popupSize)
            {
                case PopupSize.Small:
                    width = (int)(Game.System.Width / 2);
                    height = Game.System.Height / 4;
                    break;
                case PopupSize.Medium:
                    width = (int)(Game.System.Width / 1.25);
                    height = (int)(Game.System.Height / 2.5);
                    break;
                case PopupSize.Large:
                    width = (int)(Game.System.Width - 40);
                    height = (int)(Game.System.Height - 40);
                    break;
            }

            return new MessagePopup(title, text, width, height, Game.System.Width, Game.System.Height);
        }

        public void ShowErrorDialog(string title, string text, Exception ex)
        {
            var detailsPopup = BuildPopup("Detalhes", ex.GetValidatedExceptionString(), PopupSize.Large);
            var errorPopup = BuildPopup(title, text, PopupSize.Medium);

            detailsPopup.Buttons.Add(new Button("OK", () => detailsPopup.Closed = true));

            errorPopup.Buttons.Add(new Button("OK", () => errorPopup.Closed = true));
            errorPopup.Buttons.Add(new Button("Detalhes", () => Open(detailsPopup)));

            Open(errorPopup);
        }
    }
}