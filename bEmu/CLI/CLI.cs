using System;
using System.Linq;
using System.Text;
using System.Threading;
using bEmu.Core.Audio;
using bEmu.Core.Enums;
using bEmu.Core.Extensions;
using bEmu.Core.Factory;
using bEmu.Core.IO;
using bEmu.Core.Video;
using bEmu.MonoGame;
using bEmu.Systems.Factory;
using Microsoft.Xna.Framework.Audio;

namespace bEmu.CLI
{
    public static class CLI
    {
        public static void ParseArgs(string[] args)
        {
            try
            {
                switch (args[0])
                {
                    case "load":
                        Load(args[1], args[2]);
                        break;
                    case "scale":
                        Scale(args[1], args[2], int.Parse(args[3]));
                        break;
                    case "disasm":
                        Disasm(args[1], args[2]);
                        break;
                    case "play":
                        Play(args[1]);
                        break;
                    default:
                        PrintGenericHelp();
                        break;
                }   
            }
            catch (Exception)
            {
                PrintGenericHelp();
                throw;
            }
        }

        private static void Play(string fileName)
        {

        }

        private static void Disasm(string systemType, string fileName)
        {
            var system = SystemFactory.Instance.Get(Enum.Parse<SystemType>(systemType), fileName);
            system.LoadProgram();

            var disasm = DisassemblerFactory.Instance.Get(system.Type, system.MMU);
            var sb = new StringBuilder();

            foreach (var instruction in disasm.GetInstructions())
                sb.AppendLine(instruction.ToString());

            var file = FileExtensions.GetFileNameWithoutExtension(fileName) + ".asm";
            FileManager.Write(file, Encoding.UTF8.GetBytes(sb.ToString()));
        }

        private static void Scale(string scaler, string fileName, int passes)
        {
            var bitmap = BitmapScaler.Scale(fileName, Enum.Parse<ScalerType>(scaler), passes);
            var newFileName = FileExtensions.GetFileNameWithoutExtension(fileName) + "_scaled.bmp"; 
            FileManager.Write(newFileName, bitmap.ToBytes());
        }

        private static void Load(string systemName, string fileName)
        {
            var systemType = Enum.Parse<SystemType>(systemName);

            using (var game = new Main(systemType, fileName))
                game.Run();
        }

        private static void PrintGenericHelp()
        {
            Console.WriteLine(
@"bEmu CLI - Comandos disponíveis:

asm [cpu] [file]
disasm [cpu] [file]
load [system] [file]
scale [scaler] [file] [passes]
play [file]
");
        }
    }
}