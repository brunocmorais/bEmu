using System.Drawing;
using System.Text;
using bEmu.Core.GUI.Popups;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Drawers
{
    public class PopupDrawer : Drawer<IPopup>
    {
        public PopupDrawer(Canvas2DContext context) : base(context)
        {
        }

        public override async void Draw(IPopup popup)
        {
            Rectangle border = new Rectangle(popup.X, popup.Y, popup.Width, popup.Height);
            int screenHeight = border.Height - border.Y;

            await Context.SetFillStyleAsync(Black);
            await Context.FillRectAsync(popup.X, popup.Y, popup.Width, popup.Height);

            await Context.SetFillStyleAsync(White);
            await Context.FillRectAsync(border.Left, border.Top, 1, border.Height + 1);
            await Context.FillRectAsync(border.Right, border.Top, 1, border.Height + 1);
            await Context.FillRectAsync(border.Left, border.Top, border.Width + 1, 1);
            await Context.FillRectAsync(border.Left, border.Bottom, border.Width + 1, 1);

            await Context.SetFontAsync(Title);
            var titleSize = (await Context.MeasureTextAsync(popup.Title)).Width;
            float y = popup.Y + 10;
            float x = (float)((popup.Width / 2) + popup.X - (titleSize / 2));

            await Context.SetFillStyleAsync(LightGreen);
            await Context.FillTextAsync(popup.Title, x, y);

            int buttonCount = 0;

            if (popup.Buttons.Any())
            {
                buttonCount = popup.Buttons.Count();
                y = popup.Height + popup.Y - 40;

                for (int i = 0; i < buttonCount; i++)
                {
                    var button = popup.Buttons[i];

                    await Context.SetFontAsync(Regular);
                    var textButtonSize = (float)(await Context.MeasureTextAsync(button.Text)).Width;
                    x = ((popup.Width / (buttonCount + 1)) * (i + 1)) + popup.X - (textButtonSize / 2);
                    var borderButton = new Rectangle((int) x - 5, (int) y - 5, (int)(textButtonSize) + 10, (RegularSize) + 10);

                    await Context.SetFillStyleAsync(White);
                    await Context.FillRectAsync(borderButton.Left, borderButton.Top, 1, borderButton.Height + 1);
                    await Context.FillRectAsync(borderButton.Right, borderButton.Top, 1, borderButton.Height + 1);
                    await Context.FillRectAsync(borderButton.Left, borderButton.Top, borderButton.Width + 1, 1);
                    await Context.FillRectAsync(borderButton.Left, borderButton.Bottom, borderButton.Width + 1, 1);

                    var color = White;

                    if (popup.SelectedButton == button)
                    {
                        await Context.FillRectAsync(borderButton.Left, borderButton.Top, borderButton.Width, borderButton.Height);
                        color = Black;
                    }
                    
                    await Context.SetFontAsync(Regular);
                    await Context.SetFillStyleAsync(color);
                    await Context.FillTextAsync(button.Text, x, y);
                }

                float lineSize = 0;
                StringBuilder sb = new StringBuilder();

                foreach (char c in popup.Text)
                {
                    await Context.SetFontAsync(Regular);
                    lineSize += (float)(await Context.MeasureTextAsync(c.ToString())).Width;

                    if (lineSize >= popup.Width - 30)
                    {
                        sb.AppendLine();
                        lineSize = 0;
                    }

                    sb.Append(c);
                }

                await Context.SetFontAsync(Regular);
                var textSize = (await Context.MeasureTextAsync(sb.ToString())).Width;
                x = (float)((popup.Width / 2) + popup.X - (textSize / 2));
                y = popup.Y + (popup.Height / 2) - (RegularSize / 2);

                await Context.SetFillStyleAsync(White);
                await Context.FillTextAsync(sb.ToString(), x, y);
            }
        }
    }
}