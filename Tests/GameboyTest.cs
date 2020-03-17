using System;
using System.Diagnostics;
using System.Text;
using bEmu.Core;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Systems.Gameboy;
using bEmu.Core.Systems.Generic8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GameboyTest
    {
        [TestMethod]
        public void TestarGameboy()
        {
            var system = new bEmu.Core.Systems.Gameboy.System();
            string fileName = "../../../test_roms/gb/10-bit ops.gb";
            var disassembler = new bEmu.Core.CPUs.LR35902.Disassembler(system);
            var sb = new StringBuilder();

            system.MMU.LoadProgram(fileName);
            int counter = 0;

            Console.WriteLine();
            IOpcode opcode = default;

            while (!system.State.Halted && counter <= 20000)
            {
                var inst = disassembler.GetInstruction(system.State.PC).ToString();
                Console.WriteLine(system.State.ToString() + " op = " + inst);
                    
                counter++;
                opcode = system.Runner.StepCycle();
            }
        }
    }
}
