using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using bEmu.Core.Components;
using bEmu.Core.UI;

namespace bEmu.Core.UI.Menus
{
    public class FileSelectorMenu : Menu
    {
        private string currentDirectory;
        private readonly Action<string> action;
        public override string Title => "Selecione um arquivo";

        public FileSelectorMenu(IMainGame game, Action<string> action) : base(game) 
        { 
            this.action = action;
            IsSelectable = true;
        }

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            if (currentDirectory == null)
                currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            yield return new MenuOption("..", (_) =>
            {
                var dir = Directory.GetParent(currentDirectory);
                currentDirectory = dir != null ? dir.FullName : currentDirectory;
            });

            var entries = Directory.GetFileSystemEntries(currentDirectory)
                .Select(x => new KeyValuePair<string, string>(x, Path.GetFileName(x)))
                .Where(x => !x.Value.StartsWith('.')).OrderBy(x => x.Key);

            foreach (var entry in entries)
            {
                var attr = File.GetAttributes(entry.Key);

                if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    yield return new MenuOption(entry.Value, (_) => currentDirectory = entry.Key);
                else
                    yield return new MenuOption(entry.Value, (_) => action(entry.Key));
            }
        }
    }
}