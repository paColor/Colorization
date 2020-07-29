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
        public void TestEvents1()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];

            // IllRuleToUse
            ccw.IllRuleToUse = ColConfWin.IllRule.undefined;
            ResetEventCounters();
            ccw.IllRuleToUse = ColConfWin.IllRule.ceras;
            Assert.AreEqual(1, IllModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, IllModifiedEvents[0].pct);
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            ResetEventCounters();
            ccw.IllRuleToUse = ColConfWin.IllRule.ceras;
            Assert.AreEqual(0, IllModifiedEvents.Count);
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            ResetEventCounters();

            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.AreEqual(1, IllModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, IllModifiedEvents[0].pct);
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            ResetEventCounters();
            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.AreEqual(0, IllModifiedEvents.Count);
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            ResetEventCounters();

            // defBeh
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.noir);
            ResetEventCounters();
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.transparent);
            Assert.AreEqual(1, DefBehModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, DefBehModifiedEvents[0].pct);
            Assert.AreEqual(ColConfWin.DefBeh.transparent, ccw.defBeh);
            ResetEventCounters();
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.transparent);
            Assert.AreEqual(0, DefBehModifiedEvents.Count);
            Assert.AreEqual(ColConfWin.DefBeh.transparent, ccw.defBeh);
            ResetEventCounters();
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.noir);
            Assert.AreEqual(1, DefBehModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, DefBehModifiedEvents[0].pct);
            Assert.AreEqual(ColConfWin.DefBeh.noir, ccw.defBeh);
            ResetEventCounters();
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.undefined);
            Assert.AreEqual(1, DefBehModifiedEvents.Count);
            Assert.AreEqual(PhonConfType.phonemes, DefBehModifiedEvents[0].pct);
            Assert.AreEqual(ColConfWin.DefBeh.undefined, ccw.defBeh);
            ResetEventCounters();
        }

        [TestMethod]
        public void TestEvents2()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            int i = 0;
            foreach (string son in ColConfWin.sonsValides)
            {
                ResetEventCounters();
                bool prevCbxVal = ccw.GetCheck(son);
                if (!prevCbxVal)
                    ccw.SetChkSon(son, true);
                CharFormatting prevCF = ccw.GetCF(son);
                if (!prevCbxVal)
                    ccw.SetChkSon(son, false);

                ResetEventCounters();
                ccw.ClearSon(son);
                if (prevCbxVal)
                {
                    Assert.AreEqual(1, SonCBModifiedEvents.Count);
                    Assert.AreEqual(son, SonCBModifiedEvents[0].son);
                    Assert.AreEqual(PhonConfType.phonemes, SonCBModifiedEvents[0].pct);
                }
                else
                {
                    Assert.AreEqual(0, SonCBModifiedEvents.Count);
                }
                Assert.IsFalse(ccw.GetCheck(son));

                if (prevCF != TestTheText.blackCF)
                {
                    Assert.AreEqual(1, SonCharFormattingModifiedEvents.Count);
                    Assert.AreEqual(son, SonCharFormattingModifiedEvents[0].son);
                    Assert.AreEqual(PhonConfType.phonemes, SonCharFormattingModifiedEvents[0].pct);
                    ccw.SetChkSon(son, true);
                    Assert.AreEqual(TestTheText.blackCF, ccw.GetCF(son));
                    ccw.SetChkSon(son, false);
                }
                else
                {
                    Assert.AreEqual(0, SonCharFormattingModifiedEvents.Count);
                }

                ResetEventCounters();
                ccw.SetCbxAndCF(son, TestTheText.fixCFs[i]);
                Assert.AreEqual(1, SonCBModifiedEvents.Count);
                Assert.AreEqual(son, SonCBModifiedEvents[0].son);
                Assert.AreEqual(PhonConfType.phonemes, SonCBModifiedEvents[0].pct);
                Assert.IsTrue(ccw.GetCheck(son));

                Assert.AreEqual(1, SonCharFormattingModifiedEvents.Count);
                Assert.AreEqual(son, SonCharFormattingModifiedEvents[0].son);
                Assert.AreEqual(PhonConfType.phonemes, SonCharFormattingModifiedEvents[0].pct);
                Assert.AreEqual(TestTheText.fixCFs[i], ccw.GetCF(son));
                i++;
            }

            ResetEventCounters();
            ccw.ClearAllCbxSons();
            Assert.AreEqual(i, SonCBModifiedEvents.Count);
            foreach (string son in ColConfWin.sonsValides)
            {
                bool found = false;
                foreach (SonConfigModifiedEventArgs a in SonCBModifiedEvents)
                {
                    if (a.son == son)
                    {
                        found = true;
                        Assert.IsFalse(ccw.GetCheck(son));
                        break;
                    }
                }
                Assert.IsTrue(found);
            }

            ResetEventCounters();
            ccw.SetAllCbxSons();
            Assert.AreEqual(i, SonCBModifiedEvents.Count);
            foreach (string son in ColConfWin.sonsValides)
            {
                bool found = false;
                foreach (SonConfigModifiedEventArgs a in SonCBModifiedEvents)
                {
                    if (a.son == son)
                    {
                        found = true;
                        Assert.IsTrue(ccw.GetCheck(son));
                        break;
                    }
                }
                Assert.IsTrue(found);
            }
        }

        [TestMethod]
        public void TestEvents3()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            int i = 0;

            foreach (string son in ColConfWin.sonsValides)
            {
                ccw.SetChkSon(son, false);
                ResetEventCounters();
                ccw.SetChkSon(son, true);
                Assert.AreEqual(1, SonCBModifiedEvents.Count);
                Assert.AreEqual(son, SonCBModifiedEvents[0].son);
                Assert.AreEqual(PhonConfType.phonemes, SonCBModifiedEvents[0].pct);
                Assert.IsTrue(ccw.GetCheck(son));

                ResetEventCounters();
                ccw.SetChkSon(son, true);
                Assert.AreEqual(0, SonCBModifiedEvents.Count);
                Assert.IsTrue(ccw.GetCheck(son));

                ResetEventCounters();
                ccw.SetCFSon(son, TestTheText.fixCFs[i]);
                Assert.AreEqual(1, SonCharFormattingModifiedEvents.Count);
                Assert.AreEqual(son, SonCharFormattingModifiedEvents[0].son);
                Assert.AreEqual(PhonConfType.phonemes, SonCharFormattingModifiedEvents[0].pct);
                Assert.IsTrue(ccw.GetCheck(son));
                Assert.AreEqual(TestTheText.fixCFs[i], ccw.GetCF(son));

                ResetEventCounters();
                ccw.SetCFSon(son, TestTheText.fixCFs[i]);
                Assert.AreEqual(0, SonCharFormattingModifiedEvents.Count);
                Assert.AreEqual(TestTheText.fixCFs[i], ccw.GetCF(son));

                i++;
            }
        }

        // CERAS colors
        static RGB cNoir       = new RGB(000, 000, 000);
        static RGB cJaune      = new RGB(240, 222, 000);
        static RGB cOrange     = new RGB(237, 125, 049);
        static RGB cVertSapin  = new RGB(051, 153, 102);
        static RGB cViolet     = new RGB(164, 020, 210);
        static RGB cBleuFoncé  = new RGB(000, 020, 208);
        static RGB cRouge      = new RGB(255, 000, 000);
        static RGB cMarron     = new RGB(171, 121, 066);
        static RGB cBleu       = new RGB(071, 115, 255);
        static RGB cTurquoise  = new RGB(015, 201, 221);
        static RGB cGris       = new RGB(166, 166, 166);
        static RGB cRose       = new RGB(255, 100, 177);
        static RGB cVertGren   = new RGB(127, 241, 000);
        static RGB cGrisUn     = new RGB(222, 222, 222);

        // more colors
        static RGB cGrisNeutre = new RGB(221, 221, 221);
        static RGB cBleuPur    = new RGB(000, 000, 255);
        static RGB cBleuClair  = new RGB(091, 215, 255);
        static RGB cRougeFonce = new RGB(175, 000, 000);
        static RGB cBlanc      = new RGB(255, 255, 255);

        // CharFormattings
        CharFormatting cfOI        = new CharFormatting(true, false, false, false, true, cNoir, 
                                                        false, cGrisNeutre);
        CharFormatting cfBlack     = CharFormatting.BlackCF;
        CharFormatting cfO         = new CharFormatting(cJaune);
        CharFormatting cfAN        = new CharFormatting(cOrange);
        CharFormatting cf5         = new CharFormatting(cVertSapin);
        CharFormatting cfE         = new CharFormatting(cViolet);
        CharFormatting cfe         = new CharFormatting(cBleuFoncé);
        CharFormatting cfu         = new CharFormatting(cRouge);
        CharFormatting cfON        = new CharFormatting(cMarron);
        CharFormatting cf2         = new CharFormatting(cBleu);
        CharFormatting cfOIN       = new CharFormatting(cTurquoise);
        CharFormatting cfMUET      = new CharFormatting(cGris);
        CharFormatting cfeRose     = new CharFormatting(cRose);
        CharFormatting cfILL       = new CharFormatting(false, true, false, false, true, cVertGren,
                                                        false, cGrisNeutre);
        CharFormatting cfVGrenou   = new CharFormatting(cVertGren);
        CharFormatting cf1         = new CharFormatting(false, false, true);
        CharFormatting cfGrisUn = new CharFormatting(cGrisUn);

        CharFormatting cfNeutre    = new CharFormatting(cGrisNeutre);
        CharFormatting cfBLEU      = new CharFormatting(cBleuPur);
        CharFormatting cfBLEUCLAIR = new CharFormatting(cBleuClair);
        CharFormatting cfROUGEF    = new CharFormatting(cRougeFonce);
        CharFormatting cfBLANC     = new CharFormatting(cBlanc);

        [TestMethod]
        public void TestePredefCols()
        {
            // couleurs 
            Assert.AreEqual(cNoir,       ColConfWin.predefinedColors[(int)CERASColor.CERAS_oi]);
            Assert.AreEqual(cJaune,      ColConfWin.predefinedColors[(int)CERASColor.CERAS_o]);
            Assert.AreEqual(cOrange,     ColConfWin.predefinedColors[(int)CERASColor.CERAS_an]);
            Assert.AreEqual(cVertSapin,  ColConfWin.predefinedColors[(int)CERASColor.CERAS_5]);
            Assert.AreEqual(cViolet,     ColConfWin.predefinedColors[(int)CERASColor.CERAS_E]);
            Assert.AreEqual(cBleuFoncé,  ColConfWin.predefinedColors[(int)CERASColor.CERAS_e]);
            Assert.AreEqual(cRouge,      ColConfWin.predefinedColors[(int)CERASColor.CERAS_u]);
            Assert.AreEqual(cMarron,     ColConfWin.predefinedColors[(int)CERASColor.CERAS_on]);
            Assert.AreEqual(cBleu,       ColConfWin.predefinedColors[(int)CERASColor.CERAS_eu]);
            Assert.AreEqual(cTurquoise,  ColConfWin.predefinedColors[(int)CERASColor.CERAS_oin]);
            Assert.AreEqual(cGris,       ColConfWin.predefinedColors[(int)CERASColor.CERAS_muet]);
            Assert.AreEqual(cRose,       ColConfWin.predefinedColors[(int)CERASColor.CERAS_rosé]);
            Assert.AreEqual(cVertGren,   ColConfWin.predefinedColors[(int)CERASColor.CERAS_ill]);
            Assert.AreEqual(cGrisUn,     ColConfWin.predefinedColors[(int)CERASColor.CERAS_1]);

            Assert.AreEqual(cNoir,       ColConfWin.predefinedColors[(int)PredefCol.black]);
            Assert.AreEqual(cJaune,      ColConfWin.predefinedColors[(int)PredefCol.darkYellow]);
            Assert.AreEqual(cOrange,     ColConfWin.predefinedColors[(int)PredefCol.orange]);
            Assert.AreEqual(cVertSapin,  ColConfWin.predefinedColors[(int)PredefCol.darkGreen]);
            Assert.AreEqual(cViolet,     ColConfWin.predefinedColors[(int)PredefCol.violet]);
            Assert.AreEqual(cBleuFoncé,  ColConfWin.predefinedColors[(int)PredefCol.darkBlue]);
            Assert.AreEqual(cRouge,      ColConfWin.predefinedColors[(int)PredefCol.red]);
            Assert.AreEqual(cMarron,     ColConfWin.predefinedColors[(int)PredefCol.brown]);
            Assert.AreEqual(cBleu,       ColConfWin.predefinedColors[(int)PredefCol.blue]);
            Assert.AreEqual(cTurquoise,  ColConfWin.predefinedColors[(int)PredefCol.turquoise]);
            Assert.AreEqual(cGris,       ColConfWin.predefinedColors[(int)PredefCol.grey]);
            Assert.AreEqual(cRose,       ColConfWin.predefinedColors[(int)PredefCol.pink]);
            Assert.AreEqual(cVertGren,   ColConfWin.predefinedColors[(int)PredefCol.frogGreen]);
            Assert.AreEqual(cGrisUn,     ColConfWin.predefinedColors[(int)PredefCol.cerasUn]);

            Assert.AreEqual(cGrisNeutre, ColConfWin.predefinedColors[(int)PredefCol.neutral]);
            Assert.AreEqual(cBleuPur,    ColConfWin.predefinedColors[(int)PredefCol.pureBlue]);
            Assert.AreEqual(cBleuClair,  ColConfWin.predefinedColors[(int)PredefCol.lightBlue]);
            Assert.AreEqual(cRougeFonce, ColConfWin.predefinedColors[(int)PredefCol.darkRed]);
            Assert.AreEqual(cBlanc,      ColConfWin.predefinedColors[(int)PredefCol.white]);

            // CharFormatting
            // CERAScf
            Assert.AreEqual(cfOI,        ColConfWin.cerasCF[(int)CERASColor.CERAS_oi]);
            Assert.AreEqual(cfO,         ColConfWin.cerasCF[(int)CERASColor.CERAS_o]);
            Assert.AreEqual(cfAN,        ColConfWin.cerasCF[(int)CERASColor.CERAS_an]);
            Assert.AreEqual(cf5,         ColConfWin.cerasCF[(int)CERASColor.CERAS_5]);
            Assert.AreEqual(cfE,         ColConfWin.cerasCF[(int)CERASColor.CERAS_E]);
            Assert.AreEqual(cfe,         ColConfWin.cerasCF[(int)CERASColor.CERAS_e]);
            Assert.AreEqual(cfu,         ColConfWin.cerasCF[(int)CERASColor.CERAS_u]);
            Assert.AreEqual(cfON,        ColConfWin.cerasCF[(int)CERASColor.CERAS_on]);
            Assert.AreEqual(cf2,         ColConfWin.cerasCF[(int)CERASColor.CERAS_eu]);
            Assert.AreEqual(cfOIN,       ColConfWin.cerasCF[(int)CERASColor.CERAS_oin]);
            Assert.AreEqual(cfMUET,      ColConfWin.cerasCF[(int)CERASColor.CERAS_muet]);
            Assert.AreEqual(cfeRose,     ColConfWin.cerasCF[(int)CERASColor.CERAS_rosé]);
            Assert.AreEqual(cfILL,       ColConfWin.cerasCF[(int)CERASColor.CERAS_ill]);
            Assert.AreEqual(cf1,         ColConfWin.cerasCF[(int)CERASColor.CERAS_1]);

            // coloredCF
            Assert.AreEqual(cfBlack,     ColConfWin.coloredCF[(int)PredefCol.black]);
            Assert.AreEqual(cfO,         ColConfWin.coloredCF[(int)PredefCol.darkYellow]);
            Assert.AreEqual(cfAN,        ColConfWin.coloredCF[(int)PredefCol.orange]);
            Assert.AreEqual(cf5,         ColConfWin.coloredCF[(int)PredefCol.darkGreen]);
            Assert.AreEqual(cfE,         ColConfWin.coloredCF[(int)PredefCol.violet]);
            Assert.AreEqual(cfe,         ColConfWin.coloredCF[(int)PredefCol.darkBlue]);
            Assert.AreEqual(cfu,         ColConfWin.coloredCF[(int)PredefCol.red]);
            Assert.AreEqual(cfON,        ColConfWin.coloredCF[(int)PredefCol.brown]);
            Assert.AreEqual(cf2,         ColConfWin.coloredCF[(int)PredefCol.blue]);
            Assert.AreEqual(cfOIN,       ColConfWin.coloredCF[(int)PredefCol.turquoise]);
            Assert.AreEqual(cfMUET,      ColConfWin.coloredCF[(int)PredefCol.grey]);
            Assert.AreEqual(cfeRose,     ColConfWin.coloredCF[(int)PredefCol.pink]);
            Assert.AreEqual(cfVGrenou,   ColConfWin.coloredCF[(int)PredefCol.frogGreen]);
            Assert.AreEqual(cfGrisUn,    ColConfWin.coloredCF[(int)PredefCol.cerasUn]);
            Assert.AreEqual(cfNeutre,    ColConfWin.coloredCF[(int)PredefCol.neutral]);
            Assert.AreEqual(cfBLEU,      ColConfWin.coloredCF[(int)PredefCol.pureBlue]);
            Assert.AreEqual(cfBLEUCLAIR, ColConfWin.coloredCF[(int)PredefCol.lightBlue]);
            Assert.AreEqual(cfROUGEF,    ColConfWin.coloredCF[(int)PredefCol.darkRed]);
            Assert.AreEqual(cfBLANC,     ColConfWin.coloredCF[(int)PredefCol.white]);
        }

        [TestMethod]
        public void Testefault1()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            HashSet<string> testedSons = new HashSet<string>();

        }
    }
}
