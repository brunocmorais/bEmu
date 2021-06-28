using System;

namespace bEmu.Menus
{
    public class MenuOption
    {
        public MenuOption(string description, string name, Type type, Action<bool?> action, MenuOptionAlignment menuOptionAlignment = MenuOptionAlignment.Center)
        {
            Description = description;
            Name = name;
            Type = type;
            Action = action;
            MenuOptionAlignment = menuOptionAlignment;
        }

        public MenuOption(string description, MenuOptionAlignment menuOptionAlignment = MenuOptionAlignment.Center)
        {
            Description = description;
            Name = null;
            Type = typeof(void);
            Action = (b) => {};
            MenuOptionAlignment = menuOptionAlignment;
        }

        public MenuOption(string description, Action<bool?> action, MenuOptionAlignment menuOptionAlignment = MenuOptionAlignment.Center)
        {
            Description = description;
            Action = action;
            Type = typeof(void);
            Name = null;
            MenuOptionAlignment = menuOptionAlignment;
        }

        public string Description { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Action<bool?> Action { get; set; }
        public MenuOptionAlignment MenuOptionAlignment { get; set; }
    }
}