using System;
using System.Text;
using bEmu.Core.CPUs.Intel8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Intel8080Test
    {
        private const int Pc = 0x100;

        [TestMethod]
        public void TestarIntel8080()
        {
            var system = new bEmu.Systems.Generic8080.System("cpudiag");
            system.SetStartPoint(Pc);
            system.MMU.LoadProgram(Pc);
            bEmu.Core.CPUs.Intel8080.Disassembler disassembler = new bEmu.Core.CPUs.Intel8080.Disassembler(system.MMU);
            var sb = new StringBuilder();
            string diag = "";

            while (!system.State.Halted)
            {
                if (system.State.PC >= Pc)
                    sb.AppendLine(disassembler.GetInstruction(system.State.PC - Pc).ToString());

                var opcode = system.Runner.StepCycle();

                if (opcode.Byte == 0xCD) // call
                {
                    diag = (system.Runner as Intel8080<bEmu.Systems.Generic8080.State, bEmu.Systems.Generic8080.MMU>).CallDiagnosticsRoutine();

                    if (!string.IsNullOrWhiteSpace(diag) && diag.Trim() != "CPU IS OPERATIONAL")
                        Assert.Fail(sb.ToString());
                }
            }

            Console.Write(diag?.Trim());
        }
    }
}
