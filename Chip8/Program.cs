using System;

namespace bEmu.Chip8
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    throw new Exception("Informe o caminho de uma ROM.");    

                using (Chip8Game game = new Chip8Game(args[0]))
                    game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
