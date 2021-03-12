using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using ColorLibTest.ConfigTest;
using System;
using NLog;

namespace ColorLibTest.Undo
{
    [TestClass]
    public class UndoTests1
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
            //InitNLog.StartNLog();
            NLog.LogManager.Setup().SetupExtensions(s => s.AutoLoadAssemblies(false));
            var nLogConfig = LogManager.Configuration;
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            logconsole.Layout = "${longdate} ${uppercase:${level}} ${logger} ${message}";
            nLogConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);  // everything equal or higher than Debug
            nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.UndoFactory*");
            nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.ColConfWin*");
            LogManager.Configuration = nLogConfig;
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            // InitNLog.CloseNLog();
            NLog.LogManager.Shutdown();
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            UndoFactory.Clear();
        }

        [TestMethod]
        public void TestColConfWinUndo_1()
        {
            ColConfWin ccw = new ColConfWin(PhonConfType.phonemes);
            ccw.SetCeras();
            Assert.AreEqual(ColConfWinTest.cfAN, ccw.GetCF("an"));
            ccw.SetCbxAndCF("m", ColConfWinTest.cfBLEUCLAIR);
            Assert.AreEqual(ColConfWinTest.cfBLEUCLAIR, ccw.GetCF("m"));
            Assert.IsTrue(ccw.GetCheck("m"));
            ccw.SetCFSon("m", ColConfWinTest.cfON);
            Assert.AreEqual(ColConfWinTest.cfON, ccw.GetCF("m"));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWinTest.cfBLEUCLAIR, ccw.GetCF("m"));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWinTest.cfBlack, ccw.GetCF("m"));
            Assert.IsFalse(ccw.GetCheck("m"));
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(ColConfWinTest.cfBLEUCLAIR, ccw.GetCF("m"));
            Assert.IsTrue(ccw.GetCheck("m"));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWinTest.cfe, ccw.GetCF("é"));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWinTest.cfeRose, ccw.GetCF("é"));
        }
    }
}
