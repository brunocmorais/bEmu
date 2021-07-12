using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.UI;
using bEmu.Core.Util;

namespace bEmu.Core.UI.Menus
{
    public class GameMenu : IGameMenu
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

        public void OpenDebugger()
        {
            Menus.Push(new DebuggerMenu(game));
        }

        public void Update(double totalMilliseconds)
        {
            bool menuRelatedKeyPressed = false;

            if (IsOpen)
                Current.Update(totalMilliseconds);

            if (GamePadUtils.HasBeenPressed(GamePadKey.Escape)) // sair
            {
                menuRelatedKeyPressed = true;

                if (!game.Menu.IsOpen)
                    OpenMainMenu();
                else
                    game.Menu.CloseCurrentMenu();
            }

            if (GamePadUtils.HasBeenPressed(GamePadKey.F3)) // reiniciar jogo
                game.ResetGame();


            if (menuRelatedKeyPressed && ((game.Menu.IsOpen && game.IsRunning) || (!game.Menu.IsOpen && !game.IsRunning)))
                game.Pause();

            if (GamePadUtils.HasBeenPressed(GamePadKey.P)) // pausar
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

            if (GamePadUtils.HasBeenPressed(GamePadKey.F2)) // mostrar informações
                game.Options.SetOption(nameof(Options.ShowFPS), false);
        }
    }
}