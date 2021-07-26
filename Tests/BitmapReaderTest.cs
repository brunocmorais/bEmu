using bEmu.Core.Image;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class BitmapReaderTest
    {
        byte[] bitmapTest = 
        {
            0x42, 0x4D, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00, 
            0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x18, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x13, 0x0B, 0x00, 0x00, 0x13, 0x0B, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0x00, 
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00
        };

        [TestMethod]
        public void TestBitmapReader()
        {
            var bitmapReader = new BitmapReader(bitmapTest);
            Assert.AreEqual(bitmapReader[0, 0], 0x0000FFFF);
            Assert.AreEqual(bitmapReader[0, 1], 0x00FF00FF);
            Assert.AreEqual(bitmapReader[1, 0], 0xFF0000FF);
            Assert.AreEqual(bitmapReader[1, 1], 0xFFFFFFFF);
        }
    }
}