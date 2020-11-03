using System.Collections.Generic;
using System.Linq;
using bEmu.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Components
{
    public class GameMenu
    {
        public Stack<IMenu> Menus { get; }

        private readonly IMainGame game;
        private readonly MainMenu mainMenu;

        public IMenu Current => Menus.Any() ? Menus.Peek() : null;
        public bool IsOpen => Menus.Any();

        public GameMenu(IMainGame game)
        {
            Menus = new Stack<IMenu>();
            this.game = game;
            mainMenu = new MainMenu(game);
        }

        public void CloseAll()
        {
            Menus.Clear();
        }

        public void CloseCurrentMenu()
        {
            Menus.TryPop(out IMenu result);
        }

        public void OpenMenu(IMenu menu)
        {
            Menus.Push(menu);
        }

        public void OpenMainMenu()
        {
            mainMenu.UpdateMenuOptions();
            Menus.Push(mainMenu);
        }

        public void ShowAbout()
        {
            Menus.Push(new AboutMenu(game));
        }

        public void Update(GameTime gameTime)
        {
            bool menuRelatedKeyPressed = false;

            if (IsOpen)
                Current.Update(gameTime);

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Escape)) // sair
                game.StopGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F3)) // reiniciar jogo
                game.ResetGame();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.Back)) // voltar
            {
                menuRelatedKeyPressed = true;
                game.Menu.CloseCurrentMenu();
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F1)) // abrir menu
            {
                menuRelatedKeyPressed = true;

                if (!game.Menu.IsOpen)
                    OpenMainMenu();
                else
                    game.Menu.CloseAll();
            }

            if (menuRelatedKeyPressed && ((game.Menu.IsOpen && game.IsRunning) || (!game.Menu.IsOpen && !game.IsRunning)))
                game.Pause();

            if (KeyboardStateExtensions.HasBeenPressed(Keys.P)) // pausar
            {
                if (!game.Menu.IsOpen)
                {
                    if (game.IsRunning)
                        game.Osd.InsertMessage(MessageType.Default, "Pausado");
                    else
                        game.Osd.InsertMessage(MessageType.Default, "Em andamento");

                    game.Pause();
                }
            }

            if (KeyboardStateExtensions.HasBeenPressed(Keys.F2)) // mostrar informações
            {
                game.Options.ShowFPS = !game.Options.ShowFPS;
                game.Options.OptionChangedEvent(this, new OnOptionChangedEventArgs() { Property = "ShowFPS" });
            }
        }
    }
}