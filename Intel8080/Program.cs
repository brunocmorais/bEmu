using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace bEmu.Intel8080
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    throw new Exception("Informe o nome do jogo.");

                    string gameToRun = Path.GetFileNameWithoutExtension(args[0]);

                    if (gameToRun == "invaders" || gameToRun == "invadpt2")
                    {
                        using (var game = new SpaceInvadersGame(gameToRun, args[0]))
                            game.Run();
                    }
                    else
                    {
                        var gameInfos = JsonConvert.DeserializeObject<IList<GameInfo>>(File.ReadAllText("games.json"));
                        var gameInfo = gameInfos.FirstOrDefault(x => x.zipName == gameToRun);

                        if (gameInfo == null)
                            throw new Exception($"O jogo {gameToRun} não é suportado. Jogos suportados: {string.Join(", ", gameInfos.Select(x => x.zipName))}");

                        using (var game = new Generic8080Game(args[0], gameInfo.fileNames, gameInfo.memoryPositions))
                            game.Run();
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
