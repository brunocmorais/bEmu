using System;

namespace bEmu.Chip8
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Chip8Game game = new Chip8Game())
                game.Run();
        }
    }
}
