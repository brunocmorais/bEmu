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
        protected MenuOption[] menuOptions { get; private set; }
        public bool IsSelectable { get; set; }
        const int Delay = 150;

        public Menu(IMainGame game)
        {
            this.game = game;
            black = new Texture2D(game.GraphicsDevice, 1, 1);
            black.SetData(new[] { Color.FromNonPremultiplied(0, 0, 0, 0xB0) });
            white = new Texture2D(game.GraphicsDevice, 1, 1);
            white.SetData(new[] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xB0) });
            UpdateMenuOptions();
        }

        public void UpdateMenuOptions()
        {
            menuOptions = GetMenuOptions().ToArray();
        }

        public abstract IEnumerable<MenuOption> GetMenuOptions();

        public virtual void Draw()
        {
            Rectangle border = new Rectangle(10, 10, width - 20, height - 20);
            int screenHeight = border.Height - border.Y;

            game.SpriteBatch.Draw(black, new Rectangle(0, 0, width, height), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Top, 1, border.Height), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Right, border.Top, 1, border.Height), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Top, border.Width, 1), Color.White);
            game.SpriteBatch.Draw(white, new Rectangle(border.Left, border.Bottom, border.Width, 1), Color.White);

            var titleSize = game.Fonts.Title.MeasureString(Title);
            float y = titleSize.Y;

            var titlePosition = new Vector2((float)(width * 0.5 - (titleSize.X / 2)), titleSize.Y);
            game.SpriteBatch.DrawString(game.Fonts.Title, Title, titlePosition, Color.LightGreen);
            y += 10;

            int menuItemCount = menuOptions.Length;
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
                var menuOption = menuOptions.ElementAt(i);
                var textSize = game.Fonts.Regular.MeasureString(menuOption.Description);
                y += textSize.Y + 2;

                Color textColor = Color.White;

                if (i == selectedOption && IsSelectable)
                {
                    game.SpriteBatch.Draw(white, new Rectangle(10, (int) y + 2, width - 20, (int)textSize.Y), Color.White);   
                    textColor = Color.Black;
                }

                float x;

                switch (menuOption.MenuOptionAlignment)
                {
                    case MenuOptionAlignment.Center:
                        x = (float)(width * 0.5 - (textSize.X / 2));
                        break;
                    case MenuOptionAlignment.Left:
                    default:
                        x = 20;
                        break;
                }

                game.SpriteBatch.DrawString(game.Fonts.Regular, menuOption.Description, new Vector2(x, y), textColor);
            }
        }

        private int GetCountShowableItems(int screenHeight, float y)
        {
            int showableItens = 0;

            for (int i = 0; i < menuOptions.Count(); i++)
            {
                y += game.Fonts.Regular.MeasureString(menuOptions[i].Description).Y + 2;

                if (y >= screenHeight - 10)
                    break;

                showableItens++;
            }

            return showableItens;
        }

        protected void UpdateSize()
        {
            this.width = game.GameSystem.System.Width * game.Options.Size;
            this.height = game.GameSystem.System.Height * game.Options.Size;
        }

        public virtual void Update(GameTime gameTime)
        {
            var pressedKeys = KeyboardStateExtensions.GetPressedKeys();
            var option = menuOptions[selectedOption];
            bool updateSelection = (gameTime.TotalGameTime.TotalMilliseconds - lastSelectionUpdate) > Delay;
            bool simpleOption = option.Type == typeof(void);

            UpdateSize();

            if (pressedKeys.Contains(Keys.Down) && updateSelection)
                UpdateSelectedOption(1, gameTime);
            else if (pressedKeys.Contains(Keys.Up) && updateSelection)
                UpdateSelectedOption(-1, gameTime);
            else if (pressedKeys.Contains(Keys.Right) && simpleOption && updateSelection)
                UpdateSelectedOption(10, gameTime);
            else if (pressedKeys.Contains(Keys.Left) && simpleOption && updateSelection)
                UpdateSelectedOption(-10, gameTime);

            if (this.IsSelectable)
            {
                if (KeyboardStateExtensions.HasBeenPressed(Keys.Enter) && simpleOption)
                    ExecuteAction(option, true);
                else if (KeyboardStateExtensions.HasBeenPressed(Keys.Right) && !simpleOption)
                    ExecuteAction(option, false, true);
                else if (KeyboardStateExtensions.HasBeenPressed(Keys.Left) && !simpleOption)
                    ExecuteAction(option, false, false);
            }
        }

        private void ExecuteAction(MenuOption option, bool resetSelectedOption, bool? parameter = null)
        {
            option.Action(parameter);

            if (resetSelectedOption)
                selectedOption = 0;

            UpdateMenuOptions();
        }

        private void UpdateSelectedOption(int number, GameTime gameTime)
        {
            if (number >= 0)
                selectedOption = (selectedOption + number) % menuOptions.Length;
            else
                selectedOption = (selectedOption < -number ? menuOptions.Length + number - selectedOption : selectedOption + number) % menuOptions.Length;

            if (selectedOption < 0)
                selectedOption += menuOptions.Length;

            lastSelectionUpdate = gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}