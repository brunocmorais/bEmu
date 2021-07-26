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

        public override void UpdateControls(double totalMilliseconds)
        {
            if (!Items.Any())
                return;

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Enter))
            {
                if (Current.Buttons.Any())
                {
                    if (Current.SelectedButton.Action != null)
                        Current.SelectedButton.Action();

                    if (Current.Closed || Current.SelectedButton.Close)
                        CloseCurrent();
                }
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Right))
                Current.SelectRightButton();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Left))
                Current.SelectLeftButton();
        }

        public void ShowErrorDialog(string title, string text, Exception ex)
        {
            var detailsPopup = new MessagePopup(Game, PopupSize.Large, ex.GetValidatedExceptionString(), "Detalhes");
            var errorPopup = new MessagePopup(Game, PopupSize.Medium, text, title);

            detailsPopup.Buttons.Add(new Button("OK", true));

            errorPopup.Buttons.Add(new Button("OK", true));
            errorPopup.Buttons.Add(new Button("Detalhes", false, () => Open(detailsPopup)));

            Open(errorPopup);
        }
    }
}