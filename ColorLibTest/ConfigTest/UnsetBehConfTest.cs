using System;
using System.Collections.Generic;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class UnsetBehConfTest
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

        private List<CheckboxUnsetModifiedEventArgs> checkboxUnsetModifiedEvents = new List<CheckboxUnsetModifiedEventArgs>(); 
        private Config conf;

        private void HandleCheckboxUnsetModified(object sender, CheckboxUnsetModifiedEventArgs e)
        {
            checkboxUnsetModifiedEvents.Add(e);
        }

        private void ResetEventCounters()
        {
            checkboxUnsetModifiedEvents.Clear();
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            conf = new Config();
            conf.unsetBeh.CheckboxUnsetModifiedEvent += HandleCheckboxUnsetModified;
            ResetEventCounters();
        }

        [TestMethod]
        public void TestDefault()
        {
            UnsetBehConf uB = conf.unsetBeh;
            CheckConsistency(uB);
            // Default config - everything should be false
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                Assert.IsFalse(uB.GetCbuFlag((Ucbx)i));
            }
        }

        [TestMethod]
        public void TestAll()
        {
            UnsetBehConf uB = conf.unsetBeh;
            ResetEventCounters();
            uB.SetCbuFlag("All", true);
            Assert.AreEqual((int)Ucbx.last, checkboxUnsetModifiedEvents.Count);
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                Assert.IsTrue(IsInEventList((Ucbx)i));
            }
            CheckConsistency(uB);
            Assert.IsTrue(uB.GetCbuFlag("Bold"));
            Assert.IsTrue(uB.GetCbuFlag("Italic"));
            Assert.IsTrue(uB.GetCbuFlag("Underline"));
            Assert.IsTrue(uB.GetCbuFlag("Color"));
            Assert.IsTrue(uB.GetCbuFlag("Hilight"));
            Assert.IsTrue(uB.GetCbuFlag("All"));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.color));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.all));

            ResetEventCounters();
            uB.SetCbuFlag("All", false);
            Assert.AreEqual((int)Ucbx.last, checkboxUnsetModifiedEvents.Count);
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                Assert.IsTrue(IsInEventList((Ucbx)i));
            }
            CheckConsistency(uB);
            Assert.IsFalse(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsFalse(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));

            ResetEventCounters();
            uB.SetCbuFlag("All", false);
            Assert.AreEqual(0, checkboxUnsetModifiedEvents.Count);
            CheckConsistency(uB);
            Assert.IsFalse(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsFalse(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));
        }

        [TestMethod]
        public void TestReset()
        {
            UnsetBehConf uB = conf.unsetBeh;
            uB.SetCbuFlag("All", true);
            CheckConsistency(uB);
            uB.Reset();
            CheckConsistency(uB);
            Assert.IsFalse(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsFalse(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));
        }

        [TestMethod]
        public void TestFlags()
        {
            UnsetBehConf uB = conf.unsetBeh;
            ResetEventCounters();
            uB.SetCbuFlag("Bold", true);
            Assert.AreEqual(1, checkboxUnsetModifiedEvents.Count);
            Assert.AreEqual("Bold", checkboxUnsetModifiedEvents[0].unsetCBName);
            Assert.IsTrue(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsFalse(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));

            ResetEventCounters();
            uB.SetCbuFlag("Underline", true);
            Assert.AreEqual(1, checkboxUnsetModifiedEvents.Count);
            Assert.AreEqual("Underline", checkboxUnsetModifiedEvents[0].unsetCBName);
            Assert.IsTrue(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsTrue(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));

            ResetEventCounters();
            uB.SetCbuFlag("Bold", false);
            Assert.AreEqual(1, checkboxUnsetModifiedEvents.Count);
            Assert.AreEqual("Bold", checkboxUnsetModifiedEvents[0].unsetCBName);
            Assert.IsFalse(uB.GetCbuFlag("Bold"));
            Assert.IsFalse(uB.GetCbuFlag("Italic"));
            Assert.IsTrue(uB.GetCbuFlag("Underline"));
            Assert.IsFalse(uB.GetCbuFlag("Color"));
            Assert.IsFalse(uB.GetCbuFlag("Hilight"));
            Assert.IsFalse(uB.GetCbuFlag("All"));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.bold));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.italic));
            Assert.IsTrue(uB.GetCbuFlag(Ucbx.underline));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.color));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.hilight));
            Assert.IsFalse(uB.GetCbuFlag(Ucbx.all));
        }

        [TestMethod]
        public void TestExceptions()
        {
            UnsetBehConf uB = conf.unsetBeh;
            Assert.ThrowsException<ArgumentNullException>(() => uB.SetCbuFlag(null, true));
            Assert.ThrowsException<KeyNotFoundException>(() => uB.SetCbuFlag("alea", true));

            Assert.ThrowsException<ArgumentNullException>(() => _ = uB.GetCbuFlag(null));
            Assert.ThrowsException<KeyNotFoundException>(() => _ = uB.GetCbuFlag("alea"));

            Assert.ThrowsException<IndexOutOfRangeException>(() => _ = uB.GetCbuFlag(Ucbx.last));
        }

        private void CheckConsistency(UnsetBehConf uB)
        {
            if (uB.GetCbuFlag(Ucbx.all))
            {
                for (int i = 0; i < (int)Ucbx.all; i++)
                {
                    Assert.IsTrue(uB.GetCbuFlag((Ucbx)i));
                }
            }
        }

        private bool IsInEventList(Ucbx flag)
        { 
            foreach (CheckboxUnsetModifiedEventArgs e in checkboxUnsetModifiedEvents)
            {
                if (e.unsetCBX == flag)
                    return true;
            }
            return false; ;
        }

        // -------------------- Testons que le formatage est bien géré par les flags --------------

        const string text1 = @"Monsieur Poiret était une espèce de mécanique. En l’apercevant
            s’étendre comme une ombre grise le long d’une allée au Jardin-des-Plantes, la 
            tête couverte d’une vieille casquette flasque, tenant à peine sa canne à pomme
            d’ivoire jauni dans sa main, laissant flotter les pans flétris de sa redingote qui 
            cachait mal une culotte presque vide, et des jambes en bas bleus qui flageolaient comme
            celles d’un homme ivre, montrant son gilet blanc sale et son jabot de grosse mousseline 
            recroquevillée qui s’unissait imparfaitement à sa cravate cordée autour de son cou de 
            dindon, bien des gens se demandaient si cette ombre chinoise appartenait à la race
            audacieuse des fils de Japhet qui papillonnent sur le boulevard italien.";

        [TestMethod]
        public void TestFormatage1()
        {
            UnsetBehConf uB = conf.unsetBeh;
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            SylConfig sC = conf.sylConf;

            RGB color = new RGB(25, 100, 200);
            RGB hiColor = new RGB(200, 100, 25);
            CharFormatting cfAll = new CharFormatting(true, true, true, true, true, color, true, hiColor);
            ccw.SetCbxAndCF("m", cfAll);

            TestTheText ttt = new TestTheText(text1);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            ttt.AssertBold(0, true);
            ttt.AssertItalic(0, true);
            ttt.AssertUnderline(0, true);
            ttt.AssertColor(0, color);
            ttt.AssertChangeHilight(0, true);
            ttt.AssertHilightColor(0, hiColor);
            ttt.AssertBold(10, true); // La config par défaut fait ça.
            ttt.AssertColor(10, TestTheText.black);

            ccw.SetCerasRose();
            uB.SetCbuFlag("Bold", true);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            ttt.AssertBold(0, false);
            ttt.AssertItalic(0, true);
            ttt.AssertUnderline(0, true);
            ttt.AssertColor(0, color);
            ttt.AssertChangeHilight(0, true);
            ttt.AssertHilightColor(0, hiColor);
            ttt.AssertBold(10, true); // Ceras rosé fait ça.
            ttt.AssertColor(10, TestTheText.black);

            uB.SetCbuFlag("All", true);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            ttt.AssertBold(0, false);
            ttt.AssertItalic(0, false);
            ttt.AssertUnderline(0, false);
            ttt.AssertColor(0, TestTheText.black);
            ttt.AssertChangeHilight(0, false);
            ttt.AssertBold(10, true); // Ceras rosé fait ça.
            ttt.AssertColor(10, TestTheText.black);

            ttt.ColorizePhons(conf, PhonConfType.muettes);
            ttt.AssertBold(0, false);
            ttt.AssertItalic(0, false);
            ttt.AssertUnderline(0, false);
            ttt.AssertColor(0, TestTheText.black);
            ttt.AssertChangeHilight(0, false);
            ttt.AssertBold(10, false);
            ttt.AssertColor(10, TestTheText.black);
        }
    }
}
