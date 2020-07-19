using System;
using System.Collections.Generic;
using System.Globalization;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class SylConfigTest
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


        private int SylButtonModifiedEventRaised;
        private int DoubleConsStdModifiedEventRaised;
        private int ModeModifiedEventRaised;
        private int MarquerMuettesModifiedRaised;
        private int ChercherDiereseModifiedRaised;
        private int FinDeVersModifiedRaised;
        private int NbrPiedsModifiedRaised;
        private List<int> sylButModNr = new List<int>(); // modified buttons
        private Config conf;

        private void HandleSylButtonModied(object sender, SylButtonModifiedEventArgs e)
        {
            SylButtonModifiedEventRaised++;
            sylButModNr.Add(e.buttonNr);
        }

        private void HandleDoubleConsStdModified(object sender, EventArgs e)
        {
            DoubleConsStdModifiedEventRaised++;
        }

        private void HandleModeModifiedEvent(object sender, EventArgs e)
        {
            ModeModifiedEventRaised++;
        }

        private void HandleMarquerMuettesModified(object sender, EventArgs e)
        {
            MarquerMuettesModifiedRaised++;
        }
        private void HandleChercherDiereseModified(object sender, EventArgs e)
        {
            ChercherDiereseModifiedRaised++;
        }
        private void HandleFinDeVersModified(object sender, EventArgs e)
        {
            FinDeVersModifiedRaised++;
        }

        private void HandleNbrPiedsModified(object sender, EventArgs e)
        {
            NbrPiedsModifiedRaised++;
        }

        private void ResetEventCounters()
        {
            SylButtonModifiedEventRaised = 0;
            DoubleConsStdModifiedEventRaised = 0;
            ModeModifiedEventRaised = 0;
            MarquerMuettesModifiedRaised = 0;
            ChercherDiereseModifiedRaised = 0;
            FinDeVersModifiedRaised = 0;
            NbrPiedsModifiedRaised = 0;
            sylButModNr.Clear();
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
            

        }

        private void CheckConsistency (SylConfig sylConf)
        {
            int i = 0;
            while (i < SylConfig.NrButtons && !sylConf.ButtonIsActivableOne(i))
            {
                i++;
            }
            // i is the activable button (or SylConfig.NrButtons)
            for (int j = 0; j < SylConfig.NrButtons; j++)
            {
                if (j == i - 1)
                    Assert.IsTrue(sylConf.ButtonIsLastActive(j));
                else
                    Assert.IsFalse(sylConf.ButtonIsLastActive(j));
                if (j != i)
                    Assert.IsFalse(sylConf.ButtonIsActivableOne(j));
                if (j <= i)
                    Assert.IsTrue(sylConf.GetSylButtonConfFor(i + 1).buttonClickable);
                else
                    Assert.IsFalse(sylConf.GetSylButtonConfFor(i + 1).buttonClickable);
            }
        }

        private void CheckAlernateCF(CharFormatting cf1)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5, CharFormatting cf6)
        {

        }

        
        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            conf = new Config();
            conf.sylConf.ChercherDiereseModified += HandleChercherDiereseModified;
            conf.sylConf.DoubleConsStdModifiedEvent += HandleDoubleConsStdModified;
            conf.sylConf.FinDeVersModified += HandleFinDeVersModified;
            conf.sylConf.MarquerMuettesModified += HandleMarquerMuettesModified;
            conf.sylConf.ModeModifiedEvent += HandleModeModifiedEvent;
            conf.sylConf.NbrPiedsModified += HandleNbrPiedsModified;
            conf.sylConf.SylButtonModifiedEvent += HandleSylButtonModied;
            ResetEventCounters();
        }


       [TestMethod]
        public void TestMethod1()
        {
            SylConfig sC = conf.sylConf;
            for (int i = SylConfig.NrButtons - 1; i >= 0; i--)
            {
                if (sC.ButtonIsLastActive(i))
                {
                    sC.ClearButton(i);
                    Assert.IsTrue(sylButModNr.Contains(i));
                    if (i < SylConfig.NrButtons - 2)
                    {
                        Assert.IsTrue(sylButModNr.Contains(i+1));
                    }
                }
                else
                {
                    Assert.ThrowsException<ArgumentException>(() => sC.ClearButton(i));
                }
                CheckConsistency(sC);
                ResetEventCounters();
            }
            Assert.IsTrue(sC.ButtonIsActivableOne(0));
            // conf.sylConf.SylButtonModified(0, TestTheText.blueCF);
            Assert.ThrowsException<ArgumentException> 
                (() => sC.SylButtonModified(1, TestTheText.redCF));
            CheckConsistency(sC);
            sC.SylButtonModified(0, TestTheText.blueCF);
            CheckConsistency(sC);
            sC.SylButtonModified(1, TestTheText.redCF);
            CheckConsistency(sC);
        }
    }
}
