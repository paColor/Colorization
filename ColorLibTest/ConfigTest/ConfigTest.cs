using System;
using System.Collections.Generic;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class ConfigTest
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

        private int configNameModifiedEventRaised;
        private static int listSavedConfigsModifiedRaised;
        private List<ConfigReplacedEventArgs> configReplacedEvents = new List<ConfigReplacedEventArgs>();
        private List<DuoConfReplacedEventArgs> duoConfReplacedEvents = new List<DuoConfReplacedEventArgs>();
        private Config conf;

        private void HandleConfigReplaced(object sender, ConfigReplacedEventArgs e)
        {
            configReplacedEvents.Add(e);
        }

        private void HandleDuoConfReplaced(object sender, DuoConfReplacedEventArgs e)
        {
            duoConfReplacedEvents.Add(e);
        }

        private void HandleConfigNameModified(object sender, EventArgs e)
        {
            configNameModifiedEventRaised++;
        }

        private static void HandleListSavedConfigsModified(object sender, EventArgs e)
        {
            listSavedConfigsModifiedRaised++;
        }

        private void ResetEventCounters()
        {
            configNameModifiedEventRaised = 0;
            listSavedConfigsModifiedRaised = 0;
            configReplacedEvents.Clear();
            duoConfReplacedEvents.Clear();
        }

        private void AssignHandlersTo(Config c)
        {
            c.ConfigNameModifiedEvent += HandleConfigNameModified;
            c.ConfigReplacedEvent += HandleConfigReplaced;
            c.DuoConfReplacedEvent += HandleDuoConfReplaced;
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
            Config.ListSavedConfigsModified += HandleListSavedConfigsModified;
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            conf = new Config();
            AssignHandlersTo(conf);
            ResetEventCounters();
        }

        [TestMethod]
        public void TestEvents1()
        {
            ResetEventCounters();
            const string dummyName = "TestZoulouZTango_RFT349";
            const string dummyName2 = "TestZoulouZTango_RFT349-bis";
            string errMsg;
            // Let's be sure... and clean up possible mess :-)
            Config.DeleteSavedConfig(dummyName, out errMsg);
            Config.DeleteSavedConfig(dummyName2, out errMsg);

            Assert.IsFalse(conf.LoadConfig(dummyName, out errMsg));
            Assert.IsFalse(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(0, configReplacedEvents.Count);

            conf.sylConf.mode = SylConfig.Mode.poesie;
            ResetEventCounters();
            Assert.IsTrue(conf.SaveConfig(dummyName, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(1, listSavedConfigsModifiedRaised);
            Assert.AreEqual(1, configNameModifiedEventRaised);
            Assert.AreEqual(0, configReplacedEvents.Count);
            List<string> confNames = Config.GetSavedConfigNames();
            Assert.IsTrue(confNames.Contains(dummyName));

            conf.sylConf.mode = SylConfig.Mode.ecrit;

            ResetEventCounters();
            Assert.IsTrue(conf.LoadConfig(dummyName, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(0, listSavedConfigsModifiedRaised);
            Assert.AreEqual(0, configNameModifiedEventRaised);
            Assert.AreEqual(1, configReplacedEvents.Count);
            conf = configReplacedEvents[0].newConfig;
            AssignHandlersTo(conf);
            Assert.AreEqual(dummyName, configReplacedEvents[0].newConfig.GetConfigName());
            Assert.AreEqual(0, duoConfReplacedEvents.Count);
            Assert.AreEqual(SylConfig.Mode.poesie, conf.sylConf.mode);

            conf.sylConf.mode = SylConfig.Mode.oral;
            ResetEventCounters();
            Assert.IsTrue(conf.SaveConfig(dummyName2, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(1, listSavedConfigsModifiedRaised);
            Assert.AreEqual(1, configNameModifiedEventRaised);
            Assert.AreEqual(0, configReplacedEvents.Count);
            confNames = Config.GetSavedConfigNames();
            Assert.IsTrue(confNames.Contains(dummyName2));

            conf.sylConf.mode = SylConfig.Mode.ecrit;

            ResetEventCounters();
            Assert.IsTrue(conf.LoadConfig(dummyName, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(0, listSavedConfigsModifiedRaised);
            Assert.AreEqual(0, configNameModifiedEventRaised);
            Assert.AreEqual(1, configReplacedEvents.Count);
            Assert.AreEqual(dummyName, configReplacedEvents[0].newConfig.GetConfigName());
            conf = configReplacedEvents[0].newConfig;
            AssignHandlersTo(conf);
            Assert.AreEqual(0, duoConfReplacedEvents.Count);
            Assert.AreEqual(SylConfig.Mode.poesie, conf.sylConf.mode);

            ResetEventCounters();
            Assert.IsTrue(conf.LoadConfig(dummyName2, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(0, listSavedConfigsModifiedRaised);
            Assert.AreEqual(0, configNameModifiedEventRaised);
            Assert.AreEqual(1, configReplacedEvents.Count);
            Assert.AreEqual(dummyName2, configReplacedEvents[0].newConfig.GetConfigName());
            conf = configReplacedEvents[0].newConfig;
            AssignHandlersTo(conf);
            Assert.AreEqual(0, duoConfReplacedEvents.Count);
            Assert.AreEqual(SylConfig.Mode.oral, conf.sylConf.mode);

            ResetEventCounters();
            Assert.IsTrue(Config.DeleteSavedConfig(dummyName, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(1, listSavedConfigsModifiedRaised);
            Assert.AreEqual(0, configNameModifiedEventRaised);
            Assert.AreEqual(0, configReplacedEvents.Count);
            confNames = Config.GetSavedConfigNames();
            Assert.IsFalse(confNames.Contains(dummyName));

            ResetEventCounters();
            Assert.IsTrue(Config.DeleteSavedConfig(dummyName2, out errMsg));
            Assert.IsTrue(String.IsNullOrEmpty(errMsg));
            Assert.AreEqual(1, listSavedConfigsModifiedRaised);
            Assert.AreEqual(0, configNameModifiedEventRaised);
            Assert.AreEqual(0, configReplacedEvents.Count);
            confNames = Config.GetSavedConfigNames();
            Assert.IsFalse(confNames.Contains(dummyName2));
        }
    }
}
