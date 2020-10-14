using System;
using bEmu.Systems;
using bEmu.Systems.Exceptions;
using bEmu.Factory;
using Microsoft.Xna.Framework;

namespace bEmu
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
