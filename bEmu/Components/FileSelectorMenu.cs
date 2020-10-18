using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using bEmu.Classes;
using bEmu.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace bEmu.Components
{
    public class FileSelectorMenu : Menu
    {
        private string currentDirectory;
        private readonly Action<string> action;
        public override string Title => "Selecione um arquivo";

        public FileSelectorMenu(IMainGame game, Action<string> action) : base(game) 
        { 
            currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.action = action;
        }

        protected override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("..", null, typeof(void), (_) =>
            {
                var dir = Directory.GetParent(currentDirectory);
                currentDirectory = dir != null ? dir.FullName : currentDirectory;
            });

            foreach (var entry in Directory.GetFileSystemEntries(currentDirectory).OrderBy(x => Path.GetFileName(x)))
            {
                var attr = File.GetAttributes(entry);

                if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    yield return new MenuOption(Path.GetFileName(entry), null, typeof(void), (_) => currentDirectory = entry);
                else
                    yield return new MenuOption(Path.GetFileName(entry), null, typeof(void), (_) => action(entry));
            }
        }
    }
}