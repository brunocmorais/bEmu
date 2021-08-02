using System.Collections.Generic;
using System.Linq;
using bEmu.Core.Enums;
using bEmu.Core.GamePad;
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
            if (IsOpen)
                Current.UpdateControls(totalMilliseconds);

            if (GamePadStateProvider.Instance.HasBeenPressed(GamePadKey.Escape)) // sair
            {                
                if (!IsOpen)
                    Open(new MainMenu(Game));
                else
                    CloseCurrent();

                if ((!IsOpen && !Game.IsRunning) || (IsOpen && Game.IsRunning))
                    Game.Pause();

            }
        }
    }
}