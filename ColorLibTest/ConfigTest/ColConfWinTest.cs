using System;
using System.Collections.Generic;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.ConfigTest
{
    [TestClass]
    public class ColConfWinTest
    {
        // CERAS colors
        public static RGB cNoir = new RGB(000, 000, 000);
        public static RGB cJaune = new RGB(240, 222, 000);
        public static RGB cOrange = new RGB(237, 125, 049);
        public static RGB cVertSapin = new RGB(051, 153, 102);
        public static RGB cViolet = new RGB(164, 020, 210);
        public static RGB cBleuFoncé = new RGB(000, 020, 208);
        public static RGB cRouge = new RGB(255, 000, 000);
        public static RGB cMarron = new RGB(171, 121, 066);
        public static RGB cBleu = new RGB(071, 115, 255);
        public static RGB cTurquoise = new RGB(015, 201, 221);
        public static RGB cGris = new RGB(166, 166, 166);
        public static RGB cRose = new RGB(255, 100, 177);
        public static RGB cVertGren = new RGB(127, 241, 000);
        public static RGB cGrisUn = new RGB(222, 222, 222);

        // more colors
        public static RGB cGrisNeutre = new RGB(221, 221, 221);
        public static RGB cBleuPur = new RGB(000, 000, 255);
        public static RGB cBleuClair = new RGB(091, 215, 255);
        public static RGB cRougeFonce = new RGB(175, 000, 000);
        public static RGB cBlanc = new RGB(255, 255, 255);

        // CharFormattings
        public static CharFormatting cfOI = new CharFormatting(true, false, false, false, true, cNoir,
                                                        false, cGrisNeutre);
        public static CharFormatting cfBlack = CharFormatting.BlackCF;
        public static CharFormatting cfO = new CharFormatting(cJaune);
        public static CharFormatting cfAN = new CharFormatting(cOrange);
        public static CharFormatting cf5 = new CharFormatting(cVertSapin);
        public static CharFormatting cfE = new CharFormatting(cViolet);
        public static CharFormatting cfe = new CharFormatting(cBleuFoncé);
        public static CharFormatting cfu = new CharFormatting(cRouge);
        public static CharFormatting cfON = new CharFormatting(cMarron);
        public static CharFormatting cf2 = new CharFormatting(cBleu);
        public static CharFormatting cfOIN = new CharFormatting(cTurquoise);
        public static CharFormatting cfMUET = new CharFormatting(cGris);
        public static CharFormatting cfeRose = new CharFormatting(cRose);
        public static CharFormatting cfILL = new CharFormatting(false, true, false, false, true, cVertGren,
                                                        false, cGrisNeutre);
        public static CharFormatting cfVGrenou = new CharFormatting(cVertGren);
        public static CharFormatting cf1 = new CharFormatting(false, false, true);
        public static CharFormatting cfGrisUn = new CharFormatting(cGrisUn);

        public static CharFormatting cfNeutre = new CharFormatting(cGrisNeutre);
        public static CharFormatting cfBLEU = new CharFormatting(cBleuPur);
        public static CharFormatting cfBLEUCLAIR = new CharFormatting(cBleuClair);
        public static CharFormatting cfROUGEF = new CharFormatting(cRougeFonce);
        public static CharFormatting cfBLANC = new CharFormatting(cBlanc);

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

        

        [TestMethod]
        public void TestePredefCols()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];

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

            // La seule chose qui compte pour les couleurs CERAS est qu'elles soient attribuées
            // correctement aux sons.

            // configuration par défaut: CERAS rosé.

            HashSet<string> cerasSons = new HashSet<string>()
            {
                "oi", "o", "an", "5", "è", "u", "on", "2", "oin", "_muet", "é", "ill", "1"
            };
            HashSet<string> tousLesSons = new HashSet<string>(ColConfWin.sonsValides);
            tousLesSons.ExceptWith(cerasSons);
            HashSet<string> sonsNonCeras = tousLesSons;

            Assert.IsTrue(cerasSons.IsSubsetOf(ColConfWin.sonsValides));

            Assert.AreEqual(cfOI,    ccw.GetCF("oi"));
            Assert.AreEqual(cfO,     ccw.GetCF("o"));
            Assert.AreEqual(cfAN,    ccw.GetCF("an"));
            Assert.AreEqual(cf5,     ccw.GetCF("5"));
            Assert.AreEqual(cfE,     ccw.GetCF("è"));
            Assert.AreEqual(cfu,     ccw.GetCF("u"));
            Assert.AreEqual(cfON,    ccw.GetCF("on"));
            Assert.AreEqual(cf2,     ccw.GetCF("2"));
            Assert.AreEqual(cfOIN,   ccw.GetCF("oin"));
            Assert.AreEqual(cfMUET,  ccw.GetCF("_muet"));
            Assert.AreEqual(cfeRose, ccw.GetCF("é"));
            Assert.AreEqual(cfILL,   ccw.GetCF("ill"));
            Assert.AreEqual(cf1,     ccw.GetCF("1"));

            foreach (string son in cerasSons)
                Assert.IsTrue(ccw.GetCheck(son));
            foreach (string son in sonsNonCeras)
                Assert.IsFalse(ccw.GetCheck(son));

            ResetEventCounters();
            ccw.SetCeras();
            Assert.AreEqual(2, SonCharFormattingModifiedEvents.Count);  // é et ill
            Assert.AreEqual(1, SonCBModifiedEvents.Count); // ill
            Assert.AreEqual("ill", SonCBModifiedEvents[0].son);

            Assert.AreEqual(cfOI, ccw.GetCF("oi"));
            Assert.AreEqual(cfO, ccw.GetCF("o"));
            Assert.AreEqual(cfAN, ccw.GetCF("an"));
            Assert.AreEqual(cf5, ccw.GetCF("5"));
            Assert.AreEqual(cfE, ccw.GetCF("è"));
            Assert.AreEqual(cfe, ccw.GetCF("é"));
            Assert.AreEqual(cfu, ccw.GetCF("u"));
            Assert.AreEqual(cfON, ccw.GetCF("on"));
            Assert.AreEqual(cf2, ccw.GetCF("2"));
            Assert.AreEqual(cfOIN, ccw.GetCF("oin"));
            Assert.AreEqual(cfMUET, ccw.GetCF("_muet"));
            Assert.AreEqual(cf1, ccw.GetCF("1"));

            cerasSons.Remove("ill");
            sonsNonCeras.Add("ill");
            foreach (string son in cerasSons)
                Assert.IsTrue(ccw.GetCheck(son));
            foreach (string son in sonsNonCeras)
                Assert.IsFalse(ccw.GetCheck(son));
        }

        [TestMethod]
        public void TestDefaultMuettes()
        {
            ColConfWin ccw = conf.colors[PhonConfType.muettes];
            Assert.AreEqual(cfMUET, ccw.GetCF("_muet"));

            HashSet<string> sonsMuets = new HashSet<string>() { "_muet" };
            HashSet<string> tousLesSons = new HashSet<string>(ColConfWin.sonsValides);
            tousLesSons.ExceptWith(sonsMuets);
            HashSet<string> sonsNonMuets = tousLesSons;

            foreach (string son in sonsMuets)
                Assert.IsTrue(ccw.GetCheck(son));
            foreach (string son in sonsNonMuets)
                Assert.IsFalse(ccw.GetCheck(son));
        }

        [TestMethod]
        public void TestRuleFlags()
        {
            ColConfWin ccw = conf.colors[PhonConfType.muettes];
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.undefined));
            Assert.ThrowsException<ArgumentOutOfRangeException>
                (() => ccw.GetFlag(ColConfWin.RuleFlag.last));

            ccw = conf.colors[PhonConfType.phonemes];
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.undefined));
            Assert.ThrowsException<ArgumentOutOfRangeException>
                (() => ccw.GetFlag(ColConfWin.RuleFlag.last));

            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.undefined));

            ccw.IllRuleToUse = ColConfWin.IllRule.undefined;
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
            Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.undefined));
        }
    }
}
