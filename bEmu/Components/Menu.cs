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
        protected readonly BaseGame game;
        protected readonly SpriteBatch spriteBatch;
        protected readonly Fonts fonts;
        protected int width;
        protected int height;
        protected readonly Texture2D black;
        protected readonly Texture2D white;
        protected int selectedOption = 0;
        public abstract string Title { get; }

        public Menu(BaseGame game, SpriteBatch spriteBatch, Fonts fonts)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.fonts = fonts;
            black = new Texture2D(game.GraphicsDevice, 1, 1);
            black.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0xE0) });
            white = new Texture2D(game.GraphicsDevice, 1, 1);
            white.SetData(new[] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xE0) });
        }

        protected abstract IEnumerable<MenuOption> GetMenuOptions();
        public virtual void Draw()
        {
            Rectangle border = new Rectangle(10, 10, width - 20, height - 20);

            spriteBatch.Draw(black, new Rectangle(0, 0, width, height), Color.Black);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, 1, border.Height), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Right, border.Top, 1, border.Height), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, border.Width, 1), Color.White);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Bottom, border.Width, 1), Color.White);

            var titleSize = fonts.Title.MeasureString(Title);
            float y = 20.0f;

            spriteBatch.DrawString(fonts.Title, Title, new Vector2((float)(width * 0.5 - (titleSize.X / 2)), y), Color.Red);

            y += 10;

            for (int i = 0; i < GetMenuOptions().Count(); i++)
            {
                var text = GetMenuOptions().ElementAt(i).Description;
                var textSize = fonts.Regular.MeasureString(text);
                y += textSize.Y + 2;

                Color color = i == selectedOption ? Color.Green : Color.White;
                spriteBatch.DrawString(fonts.Regular, text, new Vector2((float)(width * 0.5 - (textSize.X / 2)), y), color);
            }
        }

        protected void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public virtual void Update()
        {
            SetSize(game.Width * game.Options.Size, game.Height * game.Options.Size);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Down))
                selectedOption = (selectedOption + 1) % GetMenuOptions().Count();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Up))
                selectedOption = selectedOption == 0 ? GetMenuOptions().Count() - 1 : selectedOption - 1;

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