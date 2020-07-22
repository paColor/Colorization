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
                    Assert.IsTrue(sylConf.GetSylButtonConfFor(j).buttonClickable);
                else
                    Assert.IsFalse(sylConf.GetSylButtonConfFor(j).buttonClickable);
            }
        }

        const string text1 = @"Monsieur Poiret était une espèce de mécanique. En l’apercevant
            s’étendre comme une ombre grise le long d’une allée au Jardin-des-Plantes, la 
            tête couverte d’une vieille casquette flasque, tenant à peine sa canne à pomme
            d’ivoire jauni dans sa main, laissant flotter les pans flétris de sa redingote qui 
            cachait mal une culotte presque vide, et des jambes en bas bleus qui flageolaient comme
            celles d’un homme ivre, montrant son gilet blanc sale et son jabot de grosse mousseline 
            recroquevillée qui s’unissait imparfaitement à sa cravate cordée autour de son cou de 
            dindon, bien des gens se demandaient si cette ombre chinoise appartenait à la race
            audacieuse des fils de Japhet qui papillonnent sur le boulevard italien.";

        private void CheckAlernateCF(CharFormatting cf1, Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cf = syc.NextCF();
                Assert.AreEqual(cf1, cf);
            }

            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf1);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf1);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf1);
        }

        const string text2 = @"C’était, disait-elle, une des plus anciennes et des plus estimées
            pensions bourgeoises du pays latin.";

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cfFirst = syc.NextCF();
                Assert.AreEqual(cf1, cfFirst);
                CharFormatting cfSecond = syc.NextCF();
                Assert.AreEqual(cf2, cfSecond);
            }

            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 8, cf1);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf1);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf1);
        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3, 
            Config c)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, Config c)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5, Config c)
        {

        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5, CharFormatting cf6, Config c)
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
            // Effacer tous les formatages
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
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Effacer tous les formatages
            SylConfig sC = conf.sylConf;
            for (int i = SylConfig.NrButtons - 1; i >= 0; i--)
            {
                if (sC.ButtonIsLastActive(i))
                {
                    sC.ClearButton(i);
                }
            }

            ResetEventCounters();
            sC.SylButtonModified(0, TestTheText.blueCF);
            Assert.IsTrue(sylButModNr.Contains(0));
            Assert.IsTrue(sylButModNr.Contains(1));
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, conf);

            ResetEventCounters();
            sC.SylButtonModified(1, TestTheText.redCF);
            Assert.IsTrue(sylButModNr.Contains(1));
            Assert.IsTrue(sylButModNr.Contains(2));
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, conf);

        }
    }
}
