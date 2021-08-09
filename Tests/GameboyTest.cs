using System;
using System.Diagnostics;
using System.Text;
using bEmu.Core;
using bEmu.Core.CPU.Intel8080;
using bEmu.Core.CPU;
using bEmu.Systems.Gameboy;
using bEmu.Systems.Generic8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GameboyTest
    {
        [TestMethod]
        public void TestarGameboy()
        {
            string fileName = "../../../test_roms/gb/10-bit ops.gb";
            var system = new bEmu.Systems.Gameboy.System(fileName);
            var disassembler = new bEmu.Core.CPU.LR35902.Disassembler(system.MMU);
            var sb = new StringBuilder();

            system.MMU.LoadProgram();
            int counter = 0;

            Console.WriteLine();
            IOpcode opcode = default;

            while (!system.State.Halted && counter <= 20000)
            {
                var inst = disassembler.GetInstruction((system.State as bEmu.Core.CPU.LR35902.State).PC).ToString();
                Console.WriteLine(system.State.ToString() + " op = " + inst);
                    
                counter++;
                opcode = system.Runner.StepCycle();
            }
        }

        [TestMethod]
        public void Daa()
        {
            var system = new bEmu.Systems.Gameboy.System("");
            var state = (system.State as bEmu.Systems.Gameboy.State);
            var fakeRom = new byte[32768];
            fakeRom[0x100] = 0x27;

            system.MMU.LoadProgram(fakeRom);
            state.F = 0b01010000;
            state.A = 0x00;
            
            system.Runner.StepCycle();

            Assert.AreEqual(state.A, 0xA0);
        }
    }
}
