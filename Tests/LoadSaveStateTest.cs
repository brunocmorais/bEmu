using System.Linq;
using bEmu.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class LoadSaveStateTest
    {
        [TestMethod]
        public void LoadState()
        {
            var chip8 = new bEmu.Systems.Chip8.System(null);
            var bytes = chip8.State.SaveState();
        }
    }
}