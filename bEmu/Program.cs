using System;
using bEmu.MonoGame;

namespace bEmu
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new Main())
                game.Run();
        }
    }
}
