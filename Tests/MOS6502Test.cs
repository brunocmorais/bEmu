using bEmu.Core.CPU.MOS6502;
using bEmu.Core.IO;
using bEmu.Core.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MOS6502Test
    {
        [TestMethod]
        public void Test()
        {
            var rom = new ROMReader().Read("test_roms/6502/6502_functional_test.bin");
            var system = new bEmu.Systems.NES.System(rom);
            system.Update();
        }
    }
}