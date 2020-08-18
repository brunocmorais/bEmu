using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using bEmu.Classes;
using bEmu.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Components
{
    public class MainMenu : IDrawable
    {
        private readonly BaseGame game;
        private readonly SpriteBatch spriteBatch;
        private readonly Fonts fonts;
        private int width;
        private int height;
        private readonly Texture2D black;
        private readonly Texture2D white;
        private IEnumerable<MenuOption> menuOptions;
        private int selectedOption = 0;
        public bool IsOpen { get; set; }

        public MainMenu(BaseGame game, SpriteBatch spriteBatch, Fonts fonts)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.fonts = fonts;
            black = new Texture2D(game.GraphicsDevice, 1, 1);
            black.SetData(new [] { Color.FromNonPremultiplied(0, 0, 0, 0xE0) });
            white = new Texture2D(game.GraphicsDevice, 1, 1);
            white.SetData(new [] { Color.FromNonPremultiplied(0xFF, 0xFF, 0xFF, 0xE0) });
            IsOpen = false;
            menuOptions = GetMenuOptions();
        }

        public void Draw()
        {
            if (!IsOpen)
                return;

            Rectangle border = new Rectangle(10, 10, width - 20, height - 20);

            spriteBatch.Draw(black, new Rectangle(0, 0, width, height), Color.Black);
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, 1, border.Height), Color.White); 
            spriteBatch.Draw(white, new Rectangle(border.Right, border.Top, 1, border.Height), Color.White); 
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Top, border.Width , 1), Color.White); 
            spriteBatch.Draw(white, new Rectangle(border.Left, border.Bottom, border.Width, 1), Color.White);

            const string title = "bEmu";
            var titleSize = fonts.Title.MeasureString(title);
            float y = 20.0f;

            spriteBatch.DrawString(fonts.Title, title, new Vector2((float)(width * 0.5 - (titleSize.X / 2)), y), Color.Red);

            y += 20;

            for (int i = 0; i < menuOptions.Count(); i++)
            {
                y += 25;
                var text = menuOptions.ElementAt(i).Description;
                var textSize = fonts.Regular.MeasureString(text);

                Color color = i == selectedOption ? Color.Green : Color.White;
                spriteBatch.DrawString(fonts.Regular, text, new Vector2((float)(width * 0.5 - (textSize.X / 2)), y), color);
            }
        }

        private IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("Carregar estado", null, typeof(void), null);
            yield return new MenuOption("Salvar estado", null, typeof(void), null);

            var type = game.Options.GetType();

            foreach (var prop in type.GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                var text = attr.Description;
                var propType = prop.PropertyType;
                var name = prop.Name;
                var value = GetTextFromOptionValue(prop.GetValue(game.Options));

                if (!string.IsNullOrWhiteSpace(value))
                    text = $"{text}: {value}";

                yield return new MenuOption(text, name, propType, (inc) => game.Options.SetOption(name, inc.Value));
            }

            yield return new MenuOption("Sair", null, typeof(void), (_) => game.StopGame());
        }

        private string GetTextFromOptionValue(object value)
        {
            if (value == null)
                return string.Empty;

            Type type = value.GetType();

            if (type == typeof(int))
                return ((int) value).ToString();
            else if (type == typeof(bool))
                return ((bool) value) ? "Sim" : "Não";
            else if (type.IsEnum)
                return (value as Enum).GetEnumDescription();
            
            return string.Empty;
        }

        public void Update()
        {
            if (!IsOpen)
                return;

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Down))
                selectedOption = (selectedOption + 1) % menuOptions.Count();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Up))
                selectedOption = selectedOption == 0 ? menuOptions.Count() - 1 : selectedOption - 1;

            var option = menuOptions.ElementAt(selectedOption);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Enter) && option.Type == typeof(void))
                option.Action(null);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Right) && option.Type != typeof(void))
                option.Action(true);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Left) && option.Type != typeof(void))
                option.Action(false);
        }

        public void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}