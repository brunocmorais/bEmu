using System.Linq;
using System.Text;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.UI.Menus;
using bEmu.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.Components
{

    public class Drawer : IDrawer
    {
        private readonly SpriteBatch spriteBatch;
        private readonly GraphicsDevice graphicsDevice;
        private readonly Fonts fonts;
        private readonly Texture2D transparent;
        private readonly Texture2D black;
        private readonly Texture2D white;

        public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Fonts fonts)
        {
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;
            this.fonts = fonts;
            this.transparent = new Texture2D(graphicsDevice, 1, 1);
            this.transparent.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0x80) });
            this.black = new Texture2D(graphicsDevice, 1, 1);
            this.black.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0xB0) });
            this.white = new Texture2D(graphicsDevice, 1, 1);
            this.white.SetData(new[] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xB0) });
        }

        public void Draw(IMenu menu)
        {
            Rectangle border = new Rectangle(10, 10, menu.Width - 20, menu.Height - 20);
            int screenHeight = border.Height - border.Y;

            spriteBatch.Draw(black, new Rectangle(0, 0, menu.Width, menu.Height), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, 1, border.Height), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Right, border.Top, 1, border.Height), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, border.Width, 1), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Bottom, border.Width, 1), Color.White);

            var titleSize = fonts.Title.MeasureString(menu.Title);
            float y = titleSize.Y;

            var titlePosition = new Vector2((float)(menu.Width * 0.5 - (titleSize.X / 2)), titleSize.Y);
            spriteBatch.DrawString(fonts.Title, menu.Title, titlePosition, Color.LightGreen);
            y += 10;

            int showableItens = 0;
            float auxY = y;

            for (int i = 0; i < menu.MenuOptions.Length; i++)
            {
                auxY += fonts.Regular.MeasureString(menu.MenuOptions[i].Description).Y + 2;

                if (auxY >= screenHeight - 10)
                    break;

                showableItens++;
            }

            int startItem = menu.SelectedOption - (showableItens / 2);
            int endItem = startItem + showableItens;

            if (startItem < 0)
            {
                startItem = 0;
                endItem = showableItens;
            }

            if (endItem >= menu.MenuOptions.Length)
            {
                endItem = menu.MenuOptions.Length;
                startItem = endItem - showableItens;
            }

            for (int i = startItem; i < endItem; i++)
            {
                var menuOption = menu.MenuOptions.ElementAt(i);
                var textSize = fonts.Regular.MeasureString(menuOption.Description);
                y += textSize.Y + 2;

                Color textColor = Color.White;

                if (i == menu.SelectedOption && menu.IsSelectable)
                {
                    spriteBatch.Draw(white, new Rectangle(10, (int)y + 2, menu.Width - 20, (int)textSize.Y), Color.White);
                    textColor = Color.Black;
                }

                float x;

                switch (menuOption.MenuOptionAlignment)
                {
                    case MenuOptionAlignment.Center:
                        x = (float)(menu.Width * 0.5 - (textSize.X / 2));
                        break;
                    case MenuOptionAlignment.Left:
                    default:
                        x = 20;
                        break;
                }

                spriteBatch.DrawString(fonts.Regular, menuOption.Description, new Vector2(x, y), textColor);
            }
        }

        public void Draw(IOSD osd)
        {
            var messages = osd.Messages;

            if (!messages.Any())
                return;

            var sb = new StringBuilder();

            foreach (var message in messages)
                if (!string.IsNullOrWhiteSpace(message.Text))
                    sb.AppendLine(message.Text);

            if (sb.Length == 0)
                return;

            string text = sb.ToString().Substring(0, sb.Length - 1);

            var size = fonts.Regular.MeasureString(text);
            Vector2 position = new Vector2(0, 0);

            spriteBatch.Draw(transparent, position, new Rectangle(0, 0, (int)(size.X + 10), (int)(size.Y + 10)), Color.White);
            spriteBatch.DrawString(fonts.Regular, text, new Vector2(position.X + 5, position.Y + 5), Color.YellowGreen);
        }
    }
}