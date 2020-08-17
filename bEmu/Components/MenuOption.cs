using System;

namespace bEmu.Components
{
    public class MenuOption
    {
        public MenuOption(string description, string name, Type type, Action<bool?> action)
        {
            Description = description;
            Name = name;
            Type = type;
            Action = action;
        }

        public string Description { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Action<bool?> Action { get; set; }
    }
}