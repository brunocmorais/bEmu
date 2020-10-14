using System.Collections.Generic;
using System.Linq;
using bEmu.Classes;
using bEmu.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Components
{

    public abstract class Menu : IMenu
    {
        protected readonly IMainGame game;
        protected int width;
        protected int height;
        protected readonly Texture2D black;
        protected readonly Texture2D white;
        protected int selectedOption = 0;
        public abstract string Title { get; }
        private double lastSelectionUpdate = 0;

        public Menu(IMainGame game)
        {
            this.game = game;
            black = new Texture2D(game.GraphicsDevice, 1, 1);
            black.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0xE0) });
            white = new Texture2D(game.GraphicsDevice, 1, 1);
            white.SetData(new[] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xE0) });
        }

        protected abstract IEnumerable<MenuOption> GetMenuOptions();
        public virtual void Draw()
        {
            Rectangle border = new Rectangle(10, 10, width - 20, height - 20);
            int screenHeight = border.Height - border.Y;

            game.SpriteBatch.Draw(black, new Rectangle(0, 0, width, height), Color.Black);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Top, 1, border.Height), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Right, border.Top, 1, border.Height), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Top, border.Width, 1), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Bottom, border.Width, 1), Color.White);

            var titleSize = game.Fonts.Title.MeasureString(Title);
            float y = titleSize.Y;

            var titlePosition = new Vector2((float)(width * 0.5 - (titleSize.X / 2)), titleSize.Y);
            game.SpriteBatch.DrawString(game.Fonts.Title, Title, titlePosition, Color.LightGreen);
            y += 10;

            int menuItemCount = GetMenuOptions().Count();
            int showableItens = GetCountShowableItems(screenHeight, y);
            int startItem = selectedOption - (showableItens / 2);
            int endItem = startItem + showableItens;

            if (startItem < 0)
            {
                startItem = 0;
                endItem = showableItens;
            }
            
            if (endItem >= menuItemCount)
            {
                endItem = menuItemCount;
                startItem = endItem - showableItens;
            }

            for (int i = startItem; i < endItem; i++)
            {
                var text = GetMenuOptions().ElementAt(i).Description;
                var textSize = game.Fonts.Regular.MeasureString(text);
                y += textSize.Y + 2;

                if (y >= screenHeight - 10)
                    break;

                Color textColor = Color.White;

                if (i == selectedOption)
                {
                    game.SpriteBatch.Draw(white, new Rectangle(10, (int) y + 2, width - 20, (int)textSize.Y), Color.White);   
                    textColor = Color.Black;
                }

                float x = (float)(width * 0.5 - (textSize.X / 2));
                game.SpriteBatch.DrawString(game.Fonts.Regular, text, new Vector2(x, y), textColor);
            }
        }

        private int GetCountShowableItems(int screenHeight, float y)
        {
            int showableItens = 0;

            for (int i = 0; i < GetMenuOptions().Count(); i++)
            {
                y += game.Fonts.Regular.MeasureString(GetMenuOptions().ElementAt(i).Description).Y + 2;

                if (y >= screenHeight - 10)
                    break;

                showableItens++;
            }

            return showableItens;
        }

        protected void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public virtual void Update(GameTime gameTime)
        {
            SetSize(game.GameSystem.Width * game.Options.Size, game.GameSystem.Height * game.Options.Size);

            if (KeyboardStateExtensions.GetPressedKeys().Contains(Keys.Down) && 
                (gameTime.TotalGameTime.TotalMilliseconds - lastSelectionUpdate) > 150)
            {
                selectedOption = (selectedOption + 1) % GetMenuOptions().Count();
                lastSelectionUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (KeyboardStateExtensions.GetPressedKeys().Contains(Keys.Up) && 
                (gameTime.TotalGameTime.TotalMilliseconds - lastSelectionUpdate) > 150)
            {
                selectedOption = selectedOption == 0 ? GetMenuOptions().Count() - 1 : selectedOption - 1;
                lastSelectionUpdate = gameTime.TotalGameTime.TotalMilliseconds;
            }

            var option = GetMenuOptions().ElementAt(selectedOption);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Enter) && option.Type == typeof(void))
            {
                option.Action(null);
                selectedOption = 0;
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Right) && option.Type != typeof(void))
                option.Action(true);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Left) && option.Type != typeof(void))
                option.Action(false);
        }
    }
}