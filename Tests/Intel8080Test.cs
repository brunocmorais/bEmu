using System;
using System.Text;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Systems.Generic8080;
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
            var system = new bEmu.Core.Systems.Generic8080.System();
            system.SetStartPoint(Pc);
            system.MMU.LoadProgram("cpudiag", Pc);
            Disassembler disassembler = new Disassembler("cpudiag");
            var sb = new StringBuilder();
            string diag = "";

            while (!system.State.Halted)
            {
                if (system.State.PC >= Pc)
                    sb.AppendLine(disassembler.GetInstruction(system.State.PC - Pc).ToString());

                var opcode = system.Runner.StepCycle();

                if (opcode.Byte == 0xCD) // call
                {
                    diag = (system.Runner as Intel8080<bEmu.Core.CPUs.Intel8080.State>).CallDiagnosticsRoutine();

                    if (!string.IsNullOrWhiteSpace(diag) && diag.Trim() != "CPU IS OPERATIONAL")
                        Assert.Fail(sb.ToString());
                }
            }

            Console.Write(diag?.Trim());
        }
    }
}
