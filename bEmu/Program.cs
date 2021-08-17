using System;
using System.Linq;
using bEmu.MonoGame;

namespace bEmu
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                using (var game = new Main())
                    game.Run();
            }
            else
            {
                CLI.CLI.ParseArgs(args);
            }
        }
    }
}
