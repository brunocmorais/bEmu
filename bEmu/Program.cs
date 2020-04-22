using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using bEmu.Core;
using bEmu.Core.Systems;
using Newtonsoft.Json;

namespace bEmu
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                string supportedSystems = string.Join("\n", Enum.GetNames(typeof(SupportedSystems)));

                if (args.Length == 0)
                    throw new Exception("Informe o sistema. Sistemas suportados: \n\n" + supportedSystems);

                if (args.Length == 1)
                    throw new Exception("Informe o caminho da ROM.");

                switch (Enum.Parse<SupportedSystems>(args[0]))
                {
                    case SupportedSystems.Generic8080:
                        LaunchGeneric8080(args);
                        break;
                    case SupportedSystems.Chip8:
                        LaunchChip8(args);
                        break;
                    case SupportedSystems.GameBoy:
                        LaunchGameboy(args);
                        break;
                    default:
                    throw new Exception("Sistema não suportado. Sistemas suportados: \n\n" + supportedSystems);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private static void LaunchGameboy(string[] args)
        {
            using (GameboyGame game = new GameboyGame(args[1]))
                game.Run();
        }

        private static void LaunchChip8(string[] args)
        {
            using (Chip8Game game = new Chip8Game(args[1]))
                game.Run();
        }

        private static void LaunchGeneric8080(string[] args)
        {
            string gameToRun = Path.GetFileNameWithoutExtension(args[1]);

            var gameInfos = JsonConvert.DeserializeObject<IList<GameInfo>>(File.ReadAllText("gamesIntel8080.json"));
            var gameInfo = gameInfos.FirstOrDefault(x => x.ZipName == gameToRun);

            if (gameInfo == null)
                throw new Exception($"O jogo {gameToRun} não é suportado. Jogos suportados: {string.Join(", ", gameInfos.Select(x => x.ZipName))}");

            if (gameToRun == "invaders" || gameToRun == "invadpt2")
            {
                using (var game = new SpaceInvadersGame(args[1], gameInfo.FileNames, gameInfo.MemoryPositions))
                    game.Run();
            }
            else
                using (var game = new Generic8080Game(args[1], gameInfo.FileNames, gameInfo.MemoryPositions))
                    game.Run();
        }
    }
}
