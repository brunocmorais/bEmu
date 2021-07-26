using System.Linq;
using System.Text;
using bEmu.Core.GUI.Popups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.MonoGame.Drawers
{
    public class PopupDrawer : Drawer<IPopup>
    {
        public PopupDrawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont regular, SpriteFont title) : base(spriteBatch, graphicsDevice, regular, title)
        {
        }

        public override void Draw(IPopup popup)
        {
            Rectangle border = new Rectangle(popup.X, popup.Y, popup.Width, popup.Height);
            int screenHeight = border.Height - border.Y;

            SpriteBatch.Draw(Black, new Rectangle(popup.X, popup.Y, popup.Width, popup.Height), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Top, 1, border.Height + 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Right, border.Top, 1, border.Height + 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Top, border.Width + 1, 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Bottom, border.Width + 1, 1), Color.White);

            var titleSize = Title.MeasureString(popup.Title);
            float y = popup.Y + 10;
            float x = (float)((popup.Width / 2) + popup.X - (titleSize.X / 2));
            SpriteBatch.DrawString(Title, popup.Title, new Vector2(x, y), Color.LightGreen);

            int buttonCount = 0;

            if (popup.Buttons.Any())
            {
                buttonCount = popup.Buttons.Count();
                y = popup.Height + popup.Y - 40;

                for (int i = 0; i < buttonCount; i++)
                {
                    var button = popup.Buttons[i];
                    var textButtonSize = Regular.MeasureString(button.Text);
                    x = ((popup.Width / (buttonCount + 1)) * (i + 1)) + popup.X - (textButtonSize.X / 2);
                    var borderButton = new Rectangle((int) x - 5, (int) y - 5, (int)(textButtonSize.X) + 10, (int)(textButtonSize.Y) + 10);

                    SpriteBatch.Draw(White, new Rectangle(borderButton.Left, borderButton.Top, 1, borderButton.Height + 1), Color.White);
                    SpriteBatch.Draw(White, new Rectangle(borderButton.Right, borderButton.Top, 1, borderButton.Height + 1), Color.White);
                    SpriteBatch.Draw(White, new Rectangle(borderButton.Left, borderButton.Top, borderButton.Width + 1, 1), Color.White);
                    SpriteBatch.Draw(White, new Rectangle(borderButton.Left, borderButton.Bottom, borderButton.Width + 1, 1), Color.White);

                    var color = Color.White;

                    if (popup.SelectedButton == button)
                    {
                        SpriteBatch.Draw(White, borderButton, color);
                        color = Color.Black;
                    }
                    
                    SpriteBatch.DrawString(Regular, button.Text, new Vector2(x, y), color);
                }
            }

            float lineSize = 0;
            StringBuilder sb = new StringBuilder();

            foreach (char c in popup.Text)
            {
                lineSize += Regular.MeasureString(c.ToString()).X;

                if (lineSize >= popup.Width - 30)
                {
                    sb.AppendLine();
                    lineSize = 0;
                }

                sb.Append(c);
            }

            var textSize = Regular.MeasureString(sb);
            x = (float)((popup.Width / 2) + popup.X - (textSize.X / 2));
            y = popup.Y + (popup.Height / 2) - (textSize.Y / 2);

            SpriteBatch.DrawString(Regular, sb, new Vector2(x, y), Color.White);
        }
    }
}