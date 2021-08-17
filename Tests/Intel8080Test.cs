using System;
using System.Collections.Generic;
using System.Text;
using bEmu.Core.CPU.Intel8080;
using bEmu.Core.IO;
using bEmu.Systems.Generic8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bEmu.Core.Extensions;

namespace Tests
{
    [TestClass]
    public class Intel8080Test
    {
        private const int Pc = 0x100;

        [TestMethod]
        public void TestarIntel8080()
        {
            var system = new bEmu.Systems.Generic8080.System("test_roms/intel8080/cpudiag");
            system.SetStartPoint(Pc);
            bEmu.Core.CPU.Intel8080.Disassembler disassembler = new bEmu.Core.CPU.Intel8080.Disassembler(system.MMU);
            var sb = new StringBuilder();
            string diag = "";

            while (!system.State.Halted)
            {
                if ((system.State as bEmu.Core.CPU.Intel8080.State).PC >= Pc)
                    sb.AppendLine(disassembler.GetInstruction((system.State as bEmu.Core.CPU.Intel8080.State).PC - Pc).ToString());

                var opcode = system.Runner.StepCycle() as Opcode;

                if (opcode == 0xCD) // call
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
