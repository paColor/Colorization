using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class PBDQConfigTest
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

        private int MarkAsBlackModifiedEventRaised;
        private List<int> lButNrs = new List<int>(); // modified buttons
        private Config conf;

        private void HandleLetterButtonModifiedEvent(object sender, LetterButtonModifiedEventArgs e)
        {
            lButNrs.Add(e.buttonNr);
        }

        private void HandleMarkAsBlackModifiedEvent(object sender, EventArgs e)
        {
            MarkAsBlackModifiedEventRaised++;
        }

        private void ResetEventCounters()
        {
            MarkAsBlackModifiedEventRaised = 0;
            lButNrs.Clear();
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
            conf.pBDQ.MarkAsBlackModifiedEvent += HandleMarkAsBlackModifiedEvent;
            conf.pBDQ.LetterButtonModifiedEvent += HandleLetterButtonModifiedEvent;
            ResetEventCounters();
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

        private void CheckConsistency(PBDQConfig pC)
        {
            List<char> letters = new List<char>();
            for (char ci = 'A'; ci <= 'Z'; ci++)
            {
                letters.Add(ci);
            }
            for (char ci = 'a'; ci <= 'z'; ci++)
            {
                letters.Add(ci);
            }
            Assert.AreEqual(52, letters.Count);
            CharFormatting cfOthers = pC.GetCfForPBDQLetter('#');
            Assert.AreEqual(cfOthers, pC.GetCfForPBDQLetter(PBDQConfig.inactiveLetter));

            for (int i = 0; i < PBDQConfig.nrButtons; i++)
            {
                char c;
                CharFormatting cf = pC.GetCfForPBDQButton(i, out c);
                if (c == PBDQConfig.inactiveLetter)
                {
                    Assert.AreEqual(cfOthers, cf);
                }
                else
                {
                    Assert.AreNotEqual(PBDQConfig.inactiveLetter, c);
                    Assert.AreEqual(cf, pC.GetCfForPBDQLetter(c));
                    Assert.AreEqual(c, pC.GetLetterForButtonNr(i));
                    if (letters.Contains(c))
                    {
                        letters.Remove(c);
                    }
                }
            }

            
            foreach (char c2 in letters)
            {
                Assert.AreEqual(cfOthers, pC.GetCfForPBDQLetter(c2));
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            PBDQConfig pC = conf.pBDQ;

            pC.ChangeBlackSettingTo(true);
            ResetEventCounters();
            pC.ChangeBlackSettingTo(false);
            Assert.AreEqual(1, MarkAsBlackModifiedEventRaised);
            Assert.AreEqual(false, pC.markAsBlack);
            ResetEventCounters();
            pC.ChangeBlackSettingTo(true);
            Assert.AreEqual(1, MarkAsBlackModifiedEventRaised);
            Assert.AreEqual(true, pC.markAsBlack);

            CheckConsistency(pC);
            // Clean everything
            for (int i = 0; i < PBDQConfig.nrButtons; i++)
            {
                char cOut;
                Assert.IsTrue(pC.UpdateLetter(i, PBDQConfig.inactiveLetter, CharFormatting.NeutralCF));
                CheckConsistency(pC);
                Assert.AreEqual(PBDQConfig.inactiveLetter, pC.GetLetterForButtonNr(i));
                Assert.IsFalse(pC.UpdateLetter(i, CharFormatting.NeutralCF));
            }
            CheckConsistency(pC);
            Assert.IsTrue(pC.UpdateLetter(0, 'p', TestTheText.fixCFs[0]));
            CheckConsistency(pC);
            Assert.IsTrue(pC.UpdateLetter(1, 'b', TestTheText.fixCFs[1]));
            CheckConsistency(pC);
            Assert.IsTrue(pC.UpdateLetter(2, 'd', TestTheText.fixCFs[2]));
            CheckConsistency(pC);
            Assert.IsTrue(pC.UpdateLetter(3, 'q', TestTheText.fixCFs[3]));
            CheckConsistency(pC);
        }

        [TestMethod]
        public void TestMethod2()
        {
            PBDQConfig pC = conf.pBDQ;
            char c;
            CharFormatting dummyCF = CharFormatting.NeutralCF;
            Assert.ThrowsException<ArgumentException>(() => pC.GetCfForPBDQButton(-1, out c));
            Assert.ThrowsException<ArgumentException>(() => pC.GetLetterForButtonNr(-1));
            Assert.ThrowsException<ArgumentException>(() => pC.UpdateLetter(-1, dummyCF));
            Assert.ThrowsException<ArgumentException>(() => pC.UpdateLetter(-1, 'z', dummyCF));
            Assert.ThrowsException<ArgumentException>(() => pC.GetCfForPBDQButton(PBDQConfig.nrButtons, out c));
            Assert.ThrowsException<ArgumentException>(() => pC.GetLetterForButtonNr(PBDQConfig.nrButtons));
            Assert.ThrowsException<ArgumentException>(() => pC.UpdateLetter(PBDQConfig.nrButtons, dummyCF));
            Assert.ThrowsException<ArgumentException>(() => pC.UpdateLetter(PBDQConfig.nrButtons, 'z', dummyCF));
        }

        [TestMethod]
        public void TestMethod3()
        {
            PBDQConfig pC = conf.pBDQ;
            CheckConsistency(pC);
            Assert.IsTrue(pC.UpdateLetter(0, 'M', TestTheText.fixCFs[0]));
            Assert.IsTrue(pC.UpdateLetter(1, 'P', TestTheText.fixCFs[1]));
            Assert.IsTrue(pC.UpdateLetter(2, 'e', TestTheText.fixCFs[2]));
            Assert.IsTrue(pC.UpdateLetter(3, 'h', TestTheText.fixCFs[3]));
            CheckConsistency(pC);

            TestTheText ttt = new TestTheText(text1);
            ttt.MarkLetters(conf);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, TestTheText.fixCFs[0]);
            ttt.AssertCF(index + 5, TestTheText.fixCFs[2]);
            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, TestTheText.fixCFs[1]);
            index = ttt.S.IndexOf("cachait");
            ttt.AssertCF(index + 3, TestTheText.fixCFs[3]);

            string Ms = ttt.GetCharsInCol(TestTheText.fixCols[0]);
            Assert.IsTrue(Ms.Length > 0);
            foreach (char c in Ms)
                Assert.AreEqual('M', c);

            string Ps = ttt.GetCharsInCol(TestTheText.fixCols[1]);
            Assert.IsTrue(Ps.Length > 0);
            foreach (char c in Ps)
                Assert.AreEqual('P', c);

            string es = ttt.GetCharsInCol(TestTheText.fixCols[2]);
            Assert.IsTrue(es.Length > 0);
            foreach (char c in es)
                Assert.AreEqual('e', c);

            string hs = ttt.GetCharsInCol(TestTheText.fixCols[3]);
            Assert.IsTrue(hs.Length > 0);
            foreach (char c in hs)
                Assert.AreEqual('h', c);
        }

        [TestMethod]
        public void TestMethod4()
        {
            // Il faut vérifier que le flag markAsBlack a bien l'effet voulu.
            Assert.IsTrue(false);
        }
    }
}
