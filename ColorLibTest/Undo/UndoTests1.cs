using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using ColorLibTest.ConfigTest;
using System;

namespace ColorLibTest.Undo
{
    [TestClass]
    public class UndoTests1
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
            InitNLog.StartNLog();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            InitNLog.CloseNLog();
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
