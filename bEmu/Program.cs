using System;
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
