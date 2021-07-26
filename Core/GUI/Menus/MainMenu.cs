using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;
using bEmu.Core.GUI.Popups;

namespace bEmu.Core.GUI.Menus
{
    public class MainMenu : Menu
    {
        private readonly FileSelectorMenu romSelectorMenu;

        public override string Title => "bEmu";

        public MainMenu(IMain game) : base(game) 
        { 
            romSelectorMenu = new FileSelectorMenu(game, (file) => SelectSystem(file));
            IsSelectable = true;
        }

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("Carregar jogo", (_) => game.MenuManager.Open(romSelectorMenu));
            yield return new MenuOption("Carregar estado", (_) => game.LoadState());
            yield return new MenuOption("Salvar estado", (_) => game.SaveState());
            yield return new MenuOption("Reiniciar", (_) => game.ResetGame());
            yield return new MenuOption("Fechar jogo", (_) => game.CloseGame());
            yield return new MenuOption("Opções gerais", (_) => game.MenuManager.Open(new GeneralOptionsMenu(game)));
            yield return new MenuOption("Opções do sistema", (_) => game.MenuManager.Open(new SystemOptionsMenu(game)));
            yield return new MenuOption("Debugger", (_) => game.MenuManager.Open(new DebuggerMenu(game)));
            yield return new MenuOption("Sobre", (_) => game.PopupManager.Open(new AboutPopup(game)));
            yield return new MenuOption("Sair", (_) => game.StopGame());
        }

        

        private void SelectSystem(string file)
        {
            game.MenuManager.Open(new EnumSelectorMenu<SystemType>(game, (system) => game.LoadSystem(system, file)));
        }
    }
}