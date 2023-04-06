using System.Drawing;
using System.Numerics;
using bEmu.Core.Enums;
using bEmu.Core.GUI.Menus;
using Blazor.Extensions.Canvas.Canvas2D;

namespace bEmu.Web.Drawers
{
    public class MenuDrawer : Drawer<IMenu>
    {
        public MenuDrawer(Canvas2DContext context) : base(context)
        {
        }

        public override async void Draw(IMenu menu)
        {
            Rectangle border = new Rectangle(10, 10, menu.Width - 20, menu.Height - 20);
            int screenHeight = border.Height - border.Y;

            await Context.SetFillStyleAsync(Black);
            await Context.FillRectAsync(0, 0, menu.Width, menu.Height);

            await Context.SetFillStyleAsync(White);
            await Context.FillRectAsync(border.Left, border.Top, 1, border.Height + 1);
            await Context.FillRectAsync(border.Right, border.Top, 1, border.Height + 1);
            await Context.FillRectAsync(border.Left, border.Top, border.Width + 1, 1);
            await Context.FillRectAsync(border.Left, border.Bottom, border.Width + 1, 1);

            await Context.SetFontAsync(Title);
            var titleSize = await Context.MeasureTextAsync(menu.Title);
            float y = TitleSize + 20;
            
            var titlePosition = new Vector2((float)(menu.Width * 0.5 - (titleSize.Width / 2)), y);

            await Context.SetFillStyleAsync(LightGreen);
            await Context.FillTextAsync(menu.Title, titlePosition.X, titlePosition.Y);
            y += 10;

            int showableItens = 0;
            float auxY = y;

            for (int i = 0; i < menu.MenuOptions.Length; i++)
            {
                auxY += RegularSize + 2;

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

            await Context.SetFontAsync(Regular);

            for (int i = startItem; i < endItem; i++)
            {
                var menuOption = menu.MenuOptions.ElementAt(i);
                var textSize = await Context.MeasureTextAsync(menuOption.Description);
                y += RegularSize + 2;

                string textColor = White;

                if (i == menu.SelectedOption && menu.IsSelectable)
                {
                    await Context.SetFillStyleAsync(White);
                    await Context.FillRectAsync(10, (int)y - RegularSize + 2, menu.Width - 20, RegularSize);
                    textColor = Black;
                }

                float x;

                switch (menuOption.MenuOptionAlignment)
                {
                    case MenuOptionAlignment.Center:
                        x = (float)(menu.Width * 0.5 - (textSize.Width / 2));
                        break;
                    case MenuOptionAlignment.Left:
                    default:
                        x = 20;
                        break;
                }

                await Context.SetFillStyleAsync(textColor);
                await Context.FillTextAsync(menuOption.Description, x, y);
            }
        }
    }
}