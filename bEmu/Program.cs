using System;
using bEmu.Systems;
using bEmu.Systems.Exceptions;
using bEmu.Factory;
using Microsoft.Xna.Framework;

namespace bEmu
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    throw new SystemNotSupportedException();

                if (args.Length == 1)
                    throw new Exception("Informe o caminho da ROM.");

                using (var game = GameFactory.GetGame(Enum.Parse<SupportedSystems>(args[0]), args[1]))
                    game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}
