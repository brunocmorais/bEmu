using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.Input;
using bEmu.Core.System;

namespace bEmu.Core.GUI.Menus
{

    public class MenuManager : Manager<IMenu>
    {
        public MenuManager(IMain game) : base(game)
        {
        }

        public override void UpdateControls(double totalMilliseconds)
        {
            bool menuRelatedKeyPressed = false;

            if (IsOpen)
                Current.UpdateControls(totalMilliseconds);

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Escape)) // sair
            {
                menuRelatedKeyPressed = true;

                if (!Game.MenuManager.IsOpen)
                    Open(new MainMenu(Game));
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