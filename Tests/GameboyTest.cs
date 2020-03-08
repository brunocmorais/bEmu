using System;
using System.Text;
using bEmu.Core.CPUs.Intel8080;
using bEmu.Core.Systems.Generic8080;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GameboyTest
    {
        private const int Pc = 0x100;

        [TestMethod]
        public void TestarGameboy()
        {
            var system = new bEmu.Core.Systems.Gameboy.System();
            system.Runner.StepCycle();
        }
    }
}
