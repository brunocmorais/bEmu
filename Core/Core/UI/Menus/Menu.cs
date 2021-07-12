using System.Collections.Generic;
using System.Linq;
using bEmu.Core;
using bEmu.Core.Components;
using bEmu.Core.Enums;
using bEmu.Core.UI;
using bEmu.Core.Util;

namespace bEmu.Core.UI.Menus
{

    public abstract class Menu : IMenu
    {
        protected readonly IMainGame game;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int SelectedOption { get; protected set; }
        public abstract string Title { get; }
        private double lastSelectionUpdate = 0;
        public MenuOption[] MenuOptions { get; protected set; }
        public bool IsSelectable { get; set; }
        const int Delay = 150;

        public Menu(IMainGame game)
        {
            this.game = game;
            SelectedOption = 0;
            UpdateMenuOptions();
        }

        public void UpdateMenuOptions()
        {
            MenuOptions = GetMenuOptions().ToArray();
        }

        public abstract IEnumerable<MenuOption> GetMenuOptions();

        protected void UpdateSize()
        {
            this.Width = game.System.Width * game.Options.Size;
            this.Height = game.System.Height * game.Options.Size;
        }

        public virtual void Update(double totalMilliseconds)
        {
            var pressedKeys = GamePadUtils.GetPressedGamePadKeys();
            var option = MenuOptions[SelectedOption];
            bool updateSelection = (totalMilliseconds - lastSelectionUpdate) > Delay;
            bool simpleOption = option.Type == typeof(void);

            UpdateSize();

            if (pressedKeys.Contains(GamePadKey.Down) && updateSelection)
                UpdateSelectedOption(1, totalMilliseconds);
            else if (pressedKeys.Contains(GamePadKey.Up) && updateSelection)
                UpdateSelectedOption(-1, totalMilliseconds);
            else if (pressedKeys.Contains(GamePadKey.Right) && simpleOption && updateSelection)
                UpdateSelectedOption(10, totalMilliseconds);
            else if (pressedKeys.Contains(GamePadKey.Left) && simpleOption && updateSelection)
                UpdateSelectedOption(-10, totalMilliseconds);

            if (this.IsSelectable)
            {
                if (GamePadUtils.HasBeenPressed(GamePadKey.Enter) && simpleOption)
                    ExecuteAction(option, true);
                else if (GamePadUtils.HasBeenPressed(GamePadKey.Right) && !simpleOption)
                    ExecuteAction(option, false, true);
                else if (GamePadUtils.HasBeenPressed(GamePadKey.Left) && !simpleOption)
                    ExecuteAction(option, false, false);
            }
        }

        private void ExecuteAction(MenuOption option, bool resetSelectedOption, bool? parameter = null)
        {
            option.Action(parameter);

            if (resetSelectedOption)
                SelectedOption = 0;

            UpdateMenuOptions();
        }

        private void UpdateSelectedOption(int number, double totalMilliseconds)
        {
            if (number >= 0)
                SelectedOption = (SelectedOption + number) % MenuOptions.Length;
            else
                SelectedOption = (SelectedOption < -number ? MenuOptions.Length + number - SelectedOption : SelectedOption + number) % MenuOptions.Length;

            if (SelectedOption < 0)
                SelectedOption += MenuOptions.Length;

            lastSelectionUpdate = totalMilliseconds;
        }
    }
}