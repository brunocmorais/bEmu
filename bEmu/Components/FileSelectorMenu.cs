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

        public FileSelectorMenu(BaseGame game, SpriteBatch spriteBatch, Fonts fonts, Action<string> action) : base(game, spriteBatch, fonts) 
        { 
            currentDirectory = Environment.CurrentDirectory;
            this.action = action;
        }

        public override void Update()
        {
            base.Update();
        }

        protected override IEnumerable<MenuOption> GetMenuOptions()
        {
            yield return new MenuOption("..", null, typeof(void), (_) =>
            {
                var dir = Directory.GetParent(currentDirectory);
                currentDirectory = dir != null ? dir.FullName : currentDirectory;
            });

            foreach (var file in Directory.GetFileSystemEntries(currentDirectory))
            {
                var attr = File.GetAttributes(file);
                
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    yield return new MenuOption(Path.GetFileName(file), null, typeof(void), (_) => currentDirectory = file);
                else
                    yield return new MenuOption(Path.GetFileName(file), null, typeof(void), (_) => action(file));
            }
        }
    }
}