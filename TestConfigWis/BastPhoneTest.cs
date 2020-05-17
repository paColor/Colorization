using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;

namespace TestConfigWis
{
    [TestClass]
    public class BastPhoneTest
    {

        
        [TestMethod]
        public void TestMethod1()
        {
            TheText.Init();
            var conf = new Config();
            var tx = new BastText("Bastien", conf);
            tx.ColorizePhons(PhonConfType.phonemes);
            var vertSapin = new RGB(51, 153, 102);

            Assert.AreEqual(vertSapin, tx.charFormattings[5].color);

        }
    }
}
