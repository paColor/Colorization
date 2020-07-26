using System;
using System.Collections.Generic;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class ColConfWinTest
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private List<SonConfigModifiedEventArgs> SonCharFormattingModifiedEvents = new List<SonConfigModifiedEventArgs>();
        private List<SonConfigModifiedEventArgs> SonCBModifiedEvents = new List<SonConfigModifiedEventArgs>();
        private List<PhonConfModifiedEventArgs> IllModifiedEvents = new List<PhonConfModifiedEventArgs>();
        private List<PhonConfModifiedEventArgs> DefBehModifiedEvents = new List<PhonConfModifiedEventArgs>();
        private Config conf;

        private void HandleSonCharFormattingModified(object sender, SonConfigModifiedEventArgs e)
        {
            SonCharFormattingModifiedEvents.Add(e);
        }

        private void HandleSonCBModified(object sender, SonConfigModifiedEventArgs e)
        {
            SonCBModifiedEvents.Add(e);
        }

        private void HandleIllModified(object sender, PhonConfModifiedEventArgs e)
        {
            IllModifiedEvents.Add(e);
        }

        private void HandleDefBehModified(object sender, PhonConfModifiedEventArgs e)
        {
            DefBehModifiedEvents.Add(e);
        }

        private void ResetEventCounters()
        {
            SonCharFormattingModifiedEvents.Clear();
            SonCBModifiedEvents.Clear();
            IllModifiedEvents.Clear();
            DefBehModifiedEvents.Clear();
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            conf = new Config();
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            ccw.DefBehModifiedEvent += HandleDefBehModified;
            ccw.IllModifiedEvent += HandleIllModified;
            ccw.SonCBModifiedEvent += HandleSonCBModified;
            ccw.SonCharFormattingModifiedEvent += HandleSonCharFormattingModified;
            ResetEventCounters();
        }

        [TestMethod]
        public void TestMethod1()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];

            // IllRuleToUse
            ccw.IllRuleToUse = ColConfWin.IllRule.undefined;
            ResetEventCounters();
            ccw.IllRuleToUse = ColConfWin.IllRule.ceras;
            Assert.AreEqual(1, IllModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, IllModifiedEvents[0].pct);
            ResetEventCounters();
            ccw.IllRuleToUse = ColConfWin.IllRule.ceras;
            Assert.AreEqual(0, IllModifiedEvents.Count);


        }
    }
}
