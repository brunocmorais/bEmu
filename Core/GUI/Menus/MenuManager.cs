using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.System;

namespace bEmu.Core.GUI.Menus
{

    public class MenuManager : Manager<IMenu>, IMenuManager
    {
        private readonly MainMenu mainMenu;

        public MenuManager(IMain game) : base(game)
        {
            mainMenu = new MainMenu(game);
        }

        public void OpenMainMenu()
        {
            mainMenu.UpdateMenuOptions();
            Items.Push(mainMenu);
        }

        public void ShowAbout()
        {
            Items.Push(new AboutMenu(Game));
        }

        public void OpenDebugger()
        {
            Items.Push(new DebuggerMenu(Game));
        }

        public override void Update(double totalMilliseconds)
        {
            bool menuRelatedKeyPressed = false;

            if (IsOpen)
                Current.Update(totalMilliseconds);

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Escape)) // sair
            {
                menuRelatedKeyPressed = true;

                if (!Game.MenuManager.IsOpen)
                    OpenMainMenu();
                else
                    Game.MenuManager.CloseCurrent();
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F3)) // reiniciar jogo
                Game.ResetGame();


            if (menuRelatedKeyPressed && ((Game.MenuManager.IsOpen && Game.IsRunning) || (!Game.MenuManager.IsOpen && !Game.IsRunning)))
                Game.Pause();

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.P)) // pausar
            {
                if (!Game.MenuManager.IsOpen)
                {
                    if (Game.IsRunning)
                        Game.Osd.InsertMessage(MessageType.Default, "Pausado");
                    else
                        Game.Osd.InsertMessage(MessageType.Default, "Em andamento");

                    Game.Pause();
                }
            }

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.F2)) // mostrar informações
                Game.Options.SetOption(nameof(Options.ShowFPS), false);
        }
    }
}