using System;
using System.Text;
using bEmu.Core.CPUs.Intel8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Intel8080Test
    {
        [TestMethod]
        public void TestarIntel8080()
        {
            Intel8080 cpu = new Intel8080(0x100);
            cpu.State.LoadProgram("cpudiag", 0x100);
            Disassembler disassembler = new Disassembler("cpudiag");
            var sb = new StringBuilder();
            string diag = "";

            while (!cpu.State.Halted)
            {
                if (cpu.State.PC >= 0x100)
                    sb.AppendLine(disassembler.GetInstruction(cpu.State.PC - 0x100).ToString());

                var opcode = cpu.StepCycle();

                if (opcode.Byte == 0xCD) // call
                {
                    diag = cpu.CallDiagnosticsRoutine();

                    if (!string.IsNullOrWhiteSpace(diag) && diag.Trim() != "CPU IS OPERATIONAL")
                        Assert.Fail(sb.ToString());
                }
            }

            Console.Write(diag?.Trim());
        }
    }
}
