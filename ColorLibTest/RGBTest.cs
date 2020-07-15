using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;

namespace ColorLibTest
{
    [TestClass]
    public class RGBTest
    {
        [TestMethod]
        public void TestComparison()
        {
            RGB black = new RGB(0,0,0);
            RGB red = new RGB(255, 0, 0);
            RGB red2 = new RGB(255, 0, 0);

            Assert.IsTrue(red == red2);
            Assert.IsFalse(black == red);
            Assert.IsFalse(red != red2);
            Assert.IsFalse(black != black);
            Assert.IsTrue(black != red2);

        }
    }
}
