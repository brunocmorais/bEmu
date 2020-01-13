using System;
using System.IO;
using System.Threading;

namespace Intel8080
{
    class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
             using (var game = new Game1())
                 game.Run();
            
            //DebugCpu();
        }

        private static void DebugCpu()
        {
            // var disasm = new Disassembler("invaders");
            // var instructions = disasm.GetInstructionsText();
            // File.WriteAllText("invaders.asm", instructions);

            var cpu = new CPU();
            cpu.State.LoadProgram("invaders");
            byte opcode = default(byte);

            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    opcode = cpu.EmularCiclo();
                else
                {
                    int ciclos = int.Parse(input);

                    for (int i = 0; i < ciclos; i++)
                    {
                        opcode = cpu.EmularCiclo();
                    }
                }

                Console.WriteLine("Opcode:" + opcode.ToString("X").PadLeft(2, '0'));
                Console.WriteLine(cpu.State);
            }
        }
    }
}
