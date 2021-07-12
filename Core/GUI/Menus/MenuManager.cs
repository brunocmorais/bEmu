using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.System;

namespace bEmu.Core.GUI.Menus
{
    public class MenuManager : IMenuManager
    {
        private readonly IMain game;
        private readonly MainMenu mainMenu;
        public Stack<IMenu> Menus { get; }
        public IMenu Current => Menus.Any() ? Menus.Peek() : default;
        public bool IsOpen => Menus.Any();

        public MenuManager(IMain game)
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

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Escape)) // sair
            {
                menuRelatedKeyPressed = true;

                if (!game.MenuManager.IsOpen)
                    OpenMainMenu();
                else
                    game.MenuManager.CloseCurrentMenu();
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F3)) // reiniciar jogo
                game.ResetGame();


            if (menuRelatedKeyPressed && ((game.MenuManager.IsOpen && game.IsRunning) || (!game.MenuManager.IsOpen && !game.IsRunning)))
                game.Pause();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.P)) // pausar
            {
                if (!game.MenuManager.IsOpen)
                {
                    if (game.IsRunning)
                        game.Osd.InsertMessage(MessageType.Default, "Pausado");
                    else
                        game.Osd.InsertMessage(MessageType.Default, "Em andamento");

                    game.Pause();
                }
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F2)) // mostrar informações
                game.Options.SetOption(nameof(Options.ShowFPS), false);
        }
    }
}