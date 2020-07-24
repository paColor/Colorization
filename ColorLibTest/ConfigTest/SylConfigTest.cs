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

            syc.ResetCounter();
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
            index = ttt.S.IndexOf("couverte"); // pos 28
            ttt.AssertCF(index, 8, cf2);
        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3, 
            Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cfFirst = syc.NextCF();
                Assert.AreEqual(cf1, cfFirst);
                CharFormatting cfSecond = syc.NextCF();
                Assert.AreEqual(cf2, cfSecond);
                CharFormatting cfThird = syc.NextCF();
                Assert.AreEqual(cf3, cfThird);
            }

            syc.ResetCounter();
            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 8, cf1);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf3);
            index = ttt.S.IndexOf("une");
            ttt.AssertCF(index, 3, cf1);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("couverte"); // pos 28
            ttt.AssertCF(index, 8, cf1);
        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cfFirst = syc.NextCF();
                Assert.AreEqual(cf1, cfFirst);
                CharFormatting cfSecond = syc.NextCF();
                Assert.AreEqual(cf2, cfSecond);
                CharFormatting cfThird = syc.NextCF();
                Assert.AreEqual(cf3, cfThird);
                CharFormatting cfFourth = syc.NextCF();
                Assert.AreEqual(cf4, cfFourth);
            }

            syc.ResetCounter();
            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 8, cf1);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf3);
            index = ttt.S.IndexOf("une");
            ttt.AssertCF(index, 3, cf4);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf1);
            index = ttt.S.IndexOf("couverte"); // pos 28
            ttt.AssertCF(index, 8, cf4);
        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5, Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cfFirst = syc.NextCF();
                Assert.AreEqual(cf1, cfFirst);
                CharFormatting cfSecond = syc.NextCF();
                Assert.AreEqual(cf2, cfSecond);
                CharFormatting cfThird = syc.NextCF();
                Assert.AreEqual(cf3, cfThird);
                CharFormatting cfFourth = syc.NextCF();
                Assert.AreEqual(cf4, cfFourth);
                CharFormatting cfFifth = syc.NextCF();
                Assert.AreEqual(cf5, cfFifth);
            }

            syc.ResetCounter();
            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 8, cf1);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf3);
            index = ttt.S.IndexOf("une");
            ttt.AssertCF(index, 3, cf4);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf5);
            index = ttt.S.IndexOf("couverte"); // pos 28
            ttt.AssertCF(index, 8, cf3);
        }

        private void CheckAlernateCF(CharFormatting cf1, CharFormatting cf2, CharFormatting cf3,
            CharFormatting cf4, CharFormatting cf5, CharFormatting cf6, Config c)
        {
            SylConfig syc = c.sylConf;
            syc.ResetCounter();
            for (int i = 0; i < 10; i++)
            {
                CharFormatting cfFirst = syc.NextCF();
                Assert.AreEqual(cf1, cfFirst);
                CharFormatting cfSecond = syc.NextCF();
                Assert.AreEqual(cf2, cfSecond);
                CharFormatting cfThird = syc.NextCF();
                Assert.AreEqual(cf3, cfThird);
                CharFormatting cfFourth = syc.NextCF();
                Assert.AreEqual(cf4, cfFourth);
                CharFormatting cfFifth = syc.NextCF();
                Assert.AreEqual(cf5, cfFifth);
                CharFormatting cfSixth = syc.NextCF();
                Assert.AreEqual(cf6, cfSixth);
            }

            syc.ResetCounter();
            TestTheText ttt = new TestTheText(text1);
            ttt.MarkWords(c);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 8, cf1);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 6, cf2);
            index = ttt.S.IndexOf("était");
            ttt.AssertCF(index, 5, cf3);
            index = ttt.S.IndexOf("une");
            ttt.AssertCF(index, 3, cf4);
            index = ttt.S.IndexOf("espèce");
            ttt.AssertCF(index, 6, cf5);
            index = ttt.S.IndexOf("de");
            ttt.AssertCF(index, 2, cf6);
            index = ttt.S.IndexOf("couverte"); // pos 28
            ttt.AssertCF(index, 8, cf4);
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
            Assert.IsTrue(sylButModNr.Count == 2);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, conf);

            ResetEventCounters();
            sC.SylButtonModified(1, TestTheText.redCF);
            Assert.IsTrue(sylButModNr.Contains(1));
            Assert.IsTrue(sylButModNr.Contains(2));
            Assert.IsTrue(sylButModNr.Count == 2);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, conf);

            ResetEventCounters();
            sC.SylButtonModified(2, TestTheText.fixCFs[2]);
            Assert.IsTrue(sylButModNr.Contains(2));
            Assert.IsTrue(sylButModNr.Contains(3));
            Assert.IsTrue(sylButModNr.Count == 2);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, TestTheText.fixCFs[2], conf);

            ResetEventCounters();
            sC.SylButtonModified(3, TestTheText.fixCFs[3]);
            Assert.IsTrue(sylButModNr.Contains(3));
            Assert.IsTrue(sylButModNr.Contains(4));
            Assert.IsTrue(sylButModNr.Count == 2);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, TestTheText.fixCFs[2],
                TestTheText.fixCFs[3], conf);

            ResetEventCounters();
            sC.SylButtonModified(4, TestTheText.fixCFs[4]);
            Assert.IsTrue(sylButModNr.Contains(4));
            Assert.IsTrue(sylButModNr.Contains(5));
            Assert.IsTrue(sylButModNr.Count == 2);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, TestTheText.fixCFs[2],
                TestTheText.fixCFs[3], TestTheText.fixCFs[4], conf);

            ResetEventCounters();
            sC.SylButtonModified(5, TestTheText.fixCFs[5]);
            Assert.IsTrue(sylButModNr.Contains(5));
            Assert.IsTrue(sylButModNr.Count == 1);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, TestTheText.fixCFs[2],
                TestTheText.fixCFs[3], TestTheText.fixCFs[4], TestTheText.fixCFs[5], conf);

            ResetEventCounters();
            sC.SylButtonModified(3, TestTheText.fixCFs[7]);
            Assert.IsTrue(sylButModNr.Contains(3));
            Assert.IsTrue(sylButModNr.Count == 1);
            CheckConsistency(sC);
            CheckAlernateCF(TestTheText.blueCF, TestTheText.redCF, TestTheText.fixCFs[2],
                TestTheText.fixCFs[7], TestTheText.fixCFs[4], TestTheText.fixCFs[5], conf);
        }

        [TestMethod]
        public void TestMethod3()
        {
            TestMethod2();
            SylConfig sC = conf.sylConf;
            // on sait que tous les boutons ont un CF
            for (int i = 0; i < SylConfig.NrButtons - 1; i++)
            {
                Assert.ThrowsException<ArgumentException>(() => sC.ClearButton(i));
                CheckConsistency(sC);
            }
        }

        [TestMethod]
        public void TestMethod4()
        {
            SylConfig sC = conf.sylConf;
            CheckConsistency(sC);

            ResetEventCounters();
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);

            sC.chercherDierese = false;
            ResetEventCounters();
            sC.chercherDierese = true;
            Assert.AreEqual(true, sC.chercherDierese);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(1, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.chercherDierese = false;
            Assert.AreEqual(false, sC.chercherDierese);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(1, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.chercherDierese = false;
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            ResetEventCounters();

            sC.DoubleConsStd = false;
            ResetEventCounters();
            sC.DoubleConsStd = true;
            Assert.AreEqual(true, sC.DoubleConsStd);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(1, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.DoubleConsStd = false;
            Assert.AreEqual(false, sC.DoubleConsStd);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(1, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.DoubleConsStd = false;
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            ResetEventCounters();

            sC.marquerMuettes = false;
            ResetEventCounters();
            sC.marquerMuettes = true;
            Assert.AreEqual(true, sC.marquerMuettes);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(1, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.marquerMuettes = false;
            Assert.AreEqual(false, sC.marquerMuettes);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(1, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.marquerMuettes = false;
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            ResetEventCounters();
            
            sC.mode = SylConfig.Mode.undefined;
            ResetEventCounters();
            sC.mode = SylConfig.Mode.ecrit;
            Assert.AreEqual(SylConfig.Mode.ecrit, sC.mode);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(1, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.mode = SylConfig.Mode.oral;
            Assert.AreEqual(SylConfig.Mode.oral, sC.mode);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(1, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.mode = SylConfig.Mode.poesie;
            Assert.AreEqual(SylConfig.Mode.poesie, sC.mode);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(1, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.mode = SylConfig.Mode.poesie;
            Assert.AreEqual(0, ModeModifiedEventRaised);
            ResetEventCounters();
            sC.mode = SylConfig.Mode.undefined;
            Assert.AreEqual(SylConfig.Mode.undefined, sC.mode);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(1, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();

            sC.nbrPieds = 99;
            ResetEventCounters();
            sC.nbrPieds = 0;
            Assert.AreEqual(0, sC.nbrPieds);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(1, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.nbrPieds = 17;
            Assert.AreEqual(17, sC.nbrPieds);
            Assert.AreEqual(0, SylButtonModifiedEventRaised);
            Assert.AreEqual(0, DoubleConsStdModifiedEventRaised);
            Assert.AreEqual(0, ModeModifiedEventRaised);
            Assert.AreEqual(0, MarquerMuettesModifiedRaised);
            Assert.AreEqual(0, ChercherDiereseModifiedRaised);
            Assert.AreEqual(0, FinDeVersModifiedRaised);
            Assert.AreEqual(1, NbrPiedsModifiedRaised);
            ResetEventCounters();
            sC.nbrPieds = 17;
            Assert.AreEqual(0, NbrPiedsModifiedRaised);
            ResetEventCounters();
        }

        [TestMethod]
        public void TestMethod5()
        {
            // La détection de syllabes et ses trois modes est testée suffisament ailleurs.
            // Il manque la véréfication que le marquage des muettes fonctionne.
            TestTheText ttt = new TestTheText(text1);

            // hypothèse: la config par défaut contient des paramètres qui marquent les syllabes
            conf.sylConf.mode = SylConfig.Mode.ecrit;
            conf.sylConf.marquerMuettes = true;
            conf.colors[PhonConfType.muettes].SetCFSon("_muet", TestTheText.fixCFs[0]);
            ttt.MarkSyls(conf);

            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertColor(index + 7, TestTheText.fixCols[0]);
            index = ttt.S.IndexOf("était");
            ttt.AssertColor(index + 4, TestTheText.fixCols[0]);
            index = ttt.S.IndexOf("long");
            ttt.AssertColor(index + 3, TestTheText.fixCols[0]);
        }
    }
}
