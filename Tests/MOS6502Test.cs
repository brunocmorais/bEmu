using bEmu.Core.CPU.MOS6502;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MOS6502Test
    {
        [TestMethod]
        public void Test()
        {
            var system = new bEmu.Systems.NES.System("test_roms/6502/6502_functional_test.bin");
            system.Update();
        }
    }
}