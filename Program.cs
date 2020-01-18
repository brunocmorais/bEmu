using System;
using System.IO;
using System.Threading;

namespace Intel8080
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Game1())
                game.Run();
            
            //DebugCpu();
        }

        private static void DebugCpu()
        {
            string programName = "cpudiag";

            var cpu = new CPU();
            cpu.State.LoadProgram(programName, 0x100);
            byte opcode = default(byte);

            cpu.State.Memory[0] = 0xc3;
            cpu.State.Memory[1] = 0;
            cpu.State.Memory[2] = 0x01;

            cpu.State.Memory[368] = 0x7;

            // cpu.State.Memory[0x59c] = 0xc3;
            // cpu.State.Memory[0x59d] = 0xc2;
            // cpu.State.Memory[0x59e] = 0x05;

            var disasm = new Disassembler(cpu.State.Memory);

            while (!cpu.State.Halted)
            {
                //string input = Console.ReadLine();

                //if (string.IsNullOrEmpty(input))
                //    opcode = cpu.EmularCiclo();
                //else
                {
                    //int ciclos = int.Parse(input);

                    //for (int i = 0; i < ciclos; i++)
                    {
                        Console.WriteLine(disasm.GetInstruction(cpu.State.PC).Print());
                        opcode = cpu.EmularCiclo();
                        Thread.Sleep(10);
                        //Console.WriteLine("Opcode:" + opcode.ToString("X").PadLeft(2, '0'));
                    }
                }

                //Console.WriteLine(cpu.State);
            }
        }
    }
}
