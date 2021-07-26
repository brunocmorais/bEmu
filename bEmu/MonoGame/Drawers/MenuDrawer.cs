using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.GUI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEmu.MonoGame.Drawers
{
    public class MenuDrawer : Drawer<IMenu>
    {
        public MenuDrawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont regular, SpriteFont title) : base(spriteBatch, graphicsDevice, regular, title)
        {
        }

        public override void Draw(IMenu menu)
        {
            Rectangle border = new Rectangle(10, 10, menu.Width - 20, menu.Height - 20);
            int screenHeight = border.Height - border.Y;

            SpriteBatch.Draw(Transparent, new Rectangle(0, 0, menu.Width, menu.Height), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Top, 1, border.Height + 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Right, border.Top, 1, border.Height + 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Top, border.Width + 1, 1), Color.White);
            SpriteBatch.Draw(White, new Rectangle(border.Left, border.Bottom, border.Width + 1, 1), Color.White);

            var titleSize = Title.MeasureString(menu.Title);
            float y = titleSize.Y;

            var titlePosition = new Vector2((float)(menu.Width * 0.5 - (titleSize.X / 2)), titleSize.Y);
            SpriteBatch.DrawString(Title, menu.Title, titlePosition, Color.LightGreen);
            y += 10;

            int showableItens = 0;
            float auxY = y;

            for (int i = 0; i < menu.MenuOptions.Length; i++)
            {
                auxY += Regular.MeasureString(menu.MenuOptions[i].Description).Y + 2;

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
                var textSize = Regular.MeasureString(menuOption.Description);
                y += textSize.Y + 2;

                Color textColor = Color.White;

                if (i == menu.SelectedOption && menu.IsSelectable)
                {
                    SpriteBatch.Draw(White, new Rectangle(10, (int)y + 2, menu.Width - 20, (int)textSize.Y), Color.White);
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

                SpriteBatch.DrawString(Regular, menuOption.Description, new Vector2(x, y), textColor);
            }
        }
    }
}