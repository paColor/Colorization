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

        private void CheckConsistency(ColConfWin ccw)
        {
            Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.undefined));
            Assert.ThrowsException<ArgumentOutOfRangeException>
                        (() => ccw.GetFlag(ColConfWin.RuleFlag.last));
            switch (ccw.IllRuleToUse)
            {
                case ColConfWin.IllRule.ceras:
                    Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
                    Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
                    break;
                case ColConfWin.IllRule.lirecouleur:
                    Assert.IsTrue(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
                    Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
                    break;
                case ColConfWin.IllRule.undefined:
                    Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllLireCouleur));
                    Assert.IsFalse(ccw.GetFlag(ColConfWin.RuleFlag.IllCeras));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Valeur inconnue pour IllRuleToUse");
                    break;
            }
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

            CheckConsistency(ccw);
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

            CheckConsistency(ccw);
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

            CheckConsistency(ccw);
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

            CheckConsistency(ccw);
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

            CheckConsistency(ccw);
        }

        [TestMethod]
        public void TestRuleFlags()
        {
            ColConfWin ccw = conf.colors[PhonConfType.muettes];
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            CheckConsistency(ccw);

            ccw = conf.colors[PhonConfType.phonemes];
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            CheckConsistency(ccw);

            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            CheckConsistency(ccw);

            ccw.IllRuleToUse = ColConfWin.IllRule.undefined;
            Assert.AreEqual(ColConfWin.IllRule.undefined, ccw.IllRuleToUse);
            CheckConsistency(ccw);
        }

        private Dictionary<string, CharFormatting> SetTestConfig(ColConfWin ccw)
        {
            Dictionary<string, CharFormatting> toReturn = new Dictionary<string, CharFormatting>();
            int i = 0;
            foreach (string son in ColConfWin.sonsValides)
            {
                ccw.SetCbxAndCF(son, TestTheText.fixCFs[i]);
                Assert.IsTrue(ccw.GetCheck(son));
                Assert.AreEqual(TestTheText.fixCFs[i], ccw.GetCF(son));
                toReturn.Add(son, TestTheText.fixCFs[i]);
                i++;
            }
            return toReturn;
        }

        [TestMethod]
        public void TestGetForPhons()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            Dictionary<string, CharFormatting> son2CF = SetTestConfig(ccw);
            Assert.AreEqual(ccw.GetCF("a"),     ccw.GetCF(Phonemes.a));
            Assert.AreEqual(ccw.GetCF("q"),     ccw.GetCF(Phonemes.q));
            Assert.AreEqual(ccw.GetCF("q_caduc"), ccw.GetCF(Phonemes.q_caduc));
            Assert.AreEqual(ccw.GetCF("i"),     ccw.GetCF(Phonemes.i));
            Assert.AreEqual(ccw.GetCF("o"),     ccw.GetCF(Phonemes.o));
            Assert.AreEqual(ccw.GetCF("o"),     ccw.GetCF(Phonemes.o_comp));
            Assert.AreEqual(ccw.GetCF("u"),     ccw.GetCF(Phonemes.u));
            Assert.AreEqual(ccw.GetCF("y"),     ccw.GetCF(Phonemes.y));
            Assert.AreEqual(ccw.GetCF("é"),     ccw.GetCF(Phonemes.e));
            Assert.AreEqual(ccw.GetCF("è"),     ccw.GetCF(Phonemes.E));
            Assert.AreEqual(ccw.GetCF("è"),     ccw.GetCF(Phonemes.E_comp));
            Assert.AreEqual(ccw.GetCF("é"),     ccw.GetCF(Phonemes.e_comp));
            Assert.AreEqual(ccw.GetCF("5"),     ccw.GetCF(Phonemes.e_tilda));
            Assert.AreEqual(ccw.GetCF("an"),    ccw.GetCF(Phonemes.a_tilda));
            Assert.AreEqual(ccw.GetCF("on"),    ccw.GetCF(Phonemes.o_tilda));
            Assert.AreEqual(ccw.GetCF("1"),     ccw.GetCF(Phonemes.x_tilda));
            Assert.AreEqual(ccw.GetCF("2"),     ccw.GetCF(Phonemes.x2));
            Assert.AreEqual(ccw.GetCF("oi"),    ccw.GetCF(Phonemes.oi));
            Assert.AreEqual(ccw.GetCF("oin"),   ccw.GetCF(Phonemes.w_e_tilda));
            Assert.AreEqual(ccw.GetCF("w"),     ccw.GetCF(Phonemes.w));
            Assert.AreEqual(ccw.GetCF("j"),     ccw.GetCF(Phonemes.j));
            Assert.AreEqual(ccw.GetCF("ng"),    ccw.GetCF(Phonemes.J));
            Assert.AreEqual(ccw.GetCF("ij"),    ccw.GetCF(Phonemes.i_j));
            Assert.AreEqual(ccw.GetCF("gn"),    ccw.GetCF(Phonemes.N));
            Assert.AreEqual(ccw.GetCF("p"),     ccw.GetCF(Phonemes.p));
            Assert.AreEqual(ccw.GetCF("b"),     ccw.GetCF(Phonemes.b));
            Assert.AreEqual(ccw.GetCF("t"),     ccw.GetCF(Phonemes.t));
            Assert.AreEqual(ccw.GetCF("d"),     ccw.GetCF(Phonemes.d));
            Assert.AreEqual(ccw.GetCF("k"),     ccw.GetCF(Phonemes.k));
            Assert.AreEqual(ccw.GetCF("g"),     ccw.GetCF(Phonemes.g));
            Assert.AreEqual(ccw.GetCF("f"),     ccw.GetCF(Phonemes.f));
            Assert.AreEqual(ccw.GetCF("v"),     ccw.GetCF(Phonemes.v));
            Assert.AreEqual(ccw.GetCF("s"),     ccw.GetCF(Phonemes.s));
            Assert.AreEqual(ccw.GetCF("z"),     ccw.GetCF(Phonemes.z));
            Assert.AreEqual(ccw.GetCF("ch"),    ccw.GetCF(Phonemes.S));
            Assert.AreEqual(ccw.GetCF("ge"),    ccw.GetCF(Phonemes.Z));
            Assert.AreEqual(ccw.GetCF("m"),     ccw.GetCF(Phonemes.m));
            Assert.AreEqual(ccw.GetCF("n"),     ccw.GetCF(Phonemes.n));
            Assert.AreEqual(ccw.GetCF("l"),     ccw.GetCF(Phonemes.l));
            Assert.AreEqual(ccw.GetCF("r"),     ccw.GetCF(Phonemes.R));
            Assert.AreEqual(ccw.GetCF("f"),     ccw.GetCF(Phonemes.f_ph));
            Assert.AreEqual(ccw.GetCF("k"),     ccw.GetCF(Phonemes.k_qu));
            Assert.AreEqual(ccw.GetCF("g"),     ccw.GetCF(Phonemes.g_u));
            Assert.AreEqual(ccw.GetCF("s"),     ccw.GetCF(Phonemes.s_c));
            Assert.AreEqual(ccw.GetCF("s"),     ccw.GetCF(Phonemes.s_t));
            Assert.AreEqual(ccw.GetCF("s"),     ccw.GetCF(Phonemes.s_x));
            Assert.AreEqual(ccw.GetCF("z"),     ccw.GetCF(Phonemes.z_s));
            Assert.AreEqual(ccw.GetCF("ks"),    ccw.GetCF(Phonemes.ks));
            Assert.AreEqual(ccw.GetCF("gz"),    ccw.GetCF(Phonemes.gz));
            Assert.AreEqual(ccw.GetCF("_muet"), ccw.GetCF(Phonemes.verb_3p));
            Assert.AreEqual(ccw.GetCF("_muet"), ccw.GetCF(Phonemes._muet));
            Assert.AreEqual(ccw.GetCF("ill"),   ccw.GetCF(Phonemes.j_ill));
            Assert.AreEqual(ccw.GetCF("ill"),   ccw.GetCF(Phonemes.i_j_ill));
            Assert.AreEqual(ccw.GetCF("j"),     ccw.GetCF(Phonemes.ji));
            Assert.AreEqual(ccw.GetCF("47"),    ccw.GetCF(Phonemes.chiffre));
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

        [TestMethod]
        public void TestMiseEnCouleur()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            Dictionary<string, CharFormatting> son2CF = SetTestConfig(ccw);
            TestTheText ttt = new TestTheText(text1);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            int index = ttt.S.IndexOf("Monsieur");
            ttt.AssertCF(index, 1, son2CF["m"]);
            ttt.AssertCF(index + 1, 2, son2CF["q"]);
            ttt.AssertCF(index + 3, 1, son2CF["s"]);
            ttt.AssertCF(index + 4, 1, son2CF["j"]);
            ttt.AssertCF(index + 5, 2, son2CF["2"]);
            ttt.AssertCF(index + 7, 1, son2CF["_muet"]);

            index = ttt.S.IndexOf("imparfaitement");
            ttt.AssertCF(index,      2, son2CF["5"]);
            ttt.AssertCF(index + 2,  1, son2CF["p"]);
            ttt.AssertCF(index + 3,  1, son2CF["a"]);
            ttt.AssertCF(index + 4,  1, son2CF["r"]);
            ttt.AssertCF(index + 5,  1, son2CF["f"]);
            ttt.AssertCF(index + 6,  2, son2CF["è"]);
            ttt.AssertCF(index + 8,  1, son2CF["t"]);
            ttt.AssertCF(index + 9,  1, son2CF["q"]);
            ttt.AssertCF(index + 10, 1, son2CF["m"]);
            ttt.AssertCF(index + 11, 2, son2CF["an"]);
            ttt.AssertCF(index + 13, 1, son2CF["_muet"]);

            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            index = ttt.S.IndexOf("papillonnent");
            ttt.AssertCF(index,     1, son2CF["p"]);
            ttt.AssertCF(index + 1, 1, son2CF["a"]);
            ttt.AssertCF(index + 2, 1, son2CF["p"]);
            ttt.AssertCF(index + 3, 3, son2CF["ill"]);
            ttt.AssertCF(index + 6, 1, son2CF["o"]);
            ttt.AssertCF(index + 7, 2, son2CF["n"]);
            ttt.AssertCF(index + 9, 1, son2CF["q_caduc"]);
            ttt.AssertCF(index + 10, 2, son2CF["_muet"]);

            index = ttt.S.IndexOf("couverte");
            ttt.AssertCF(index, 1, son2CF["k"]);
            ttt.AssertCF(index + 1, 2, son2CF["u"]);
            ttt.AssertCF(index + 3, 1, son2CF["v"]);
            ttt.AssertCF(index + 4, 1, son2CF["è"]);
            ttt.AssertCF(index + 5, 1, son2CF["r"]);
            ttt.AssertCF(index + 6, 1, son2CF["t"]);
            ttt.AssertCF(index + 7, 1, son2CF["q_caduc"]);

            index = ttt.S.IndexOf("ombre");
            ttt.AssertCF(index, 2, son2CF["on"]);
            ttt.AssertCF(index + 2, 1, son2CF["b"]);
            ttt.AssertCF(index + 3, 1, son2CF["r"]);
            ttt.AssertCF(index + 4, 1, son2CF["q_caduc"]);

            index = ttt.S.IndexOf("mécanique");
            ttt.AssertCF(index, 1, son2CF["m"]);
            ttt.AssertCF(index + 1, 1, son2CF["é"]);
            ttt.AssertCF(index + 2, 1, son2CF["k"]);
            ttt.AssertCF(index + 3, 1, son2CF["a"]);
            ttt.AssertCF(index + 4, 1, son2CF["n"]);
            ttt.AssertCF(index + 5, 1, son2CF["i"]);
            ttt.AssertCF(index + 6, 2, son2CF["k"]);
            ttt.AssertCF(index + 8, 1, son2CF["q_caduc"]);

            index = ttt.S.IndexOf("culotte");
            ttt.AssertCF(index, 1, son2CF["k"]);
            ttt.AssertCF(index + 1, 1, son2CF["y"]);
            ttt.AssertCF(index + 2, 1, son2CF["l"]);
            ttt.AssertCF(index + 3, 1, son2CF["o"]);
            ttt.AssertCF(index + 4, 2, son2CF["t"]);
            ttt.AssertCF(index + 6, 1, son2CF["q_caduc"]);

            index = ttt.S.IndexOf("Poiret");
            ttt.AssertCF(index, 1, son2CF["p"]);
            ttt.AssertCF(index + 1, 2, son2CF["oi"]);
            ttt.AssertCF(index + 3, 1, son2CF["r"]);
            ttt.AssertCF(index + 4, 2, son2CF["è"]);

            TestTheText ttt2 = new TestTheText("pria");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["p"]);
            ttt2.AssertCF(1, 1, son2CF["r"]);
            ttt2.AssertCF(2, 1, son2CF["ij"]);
            ttt2.AssertCF(3, 1, son2CF["a"]);

            index = ttt.S.IndexOf(@"celles d’un");
            ttt.AssertCF(index + 9, 2, son2CF["1"]);

            ttt2 = new TestTheText("soin");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["s"]);
            ttt2.AssertCF(1, 3, son2CF["oin"]);

            ttt2 = new TestTheText("parking");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["p"]);
            ttt2.AssertCF(1, 1, son2CF["a"]);
            ttt2.AssertCF(2, 1, son2CF["r"]);
            ttt2.AssertCF(3, 1, son2CF["k"]);
            ttt2.AssertCF(4, 1, son2CF["i"]);
            ttt2.AssertCF(5, 2, son2CF["ng"]);

            index = ttt.S.IndexOf("Jardin");
            ttt.AssertCF(index, 1, son2CF["ge"]);
            ttt.AssertCF(index + 1, 1, son2CF["a"]);
            ttt.AssertCF(index + 2, 1, son2CF["r"]);
            ttt.AssertCF(index + 3, 1, son2CF["d"]);
            ttt.AssertCF(index + 4, 2, son2CF["5"]);

            index = ttt.S.IndexOf("chinoise");
            ttt.AssertCF(index, 2, son2CF["ch"]);
            ttt.AssertCF(index + 2, 1, son2CF["i"]);
            ttt.AssertCF(index + 3, 1, son2CF["n"]);
            ttt.AssertCF(index + 4, 2, son2CF["oi"]);
            ttt.AssertCF(index + 6, 1, son2CF["z"]);
            ttt.AssertCF(index + 7, 1, son2CF["q_caduc"]);

            index = ttt.S.IndexOf("grise");
            ttt.AssertCF(index, 1, son2CF["g"]);
            ttt.AssertCF(index + 1, 1, son2CF["r"]);
            ttt.AssertCF(index + 2, 1, son2CF["i"]);
            ttt.AssertCF(index + 3, 1, son2CF["z"]);
            ttt.AssertCF(index + 4, 1, son2CF["q_caduc"]);

            ttt2 = new TestTheText("ligne");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["l"]);
            ttt2.AssertCF(1, 1, son2CF["i"]);
            ttt2.AssertCF(2, 2, son2CF["gn"]);
            ttt2.AssertCF(4, 1, son2CF["q_caduc"]);

            ttt2 = new TestTheText("rixe");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["r"]);
            ttt2.AssertCF(1, 1, son2CF["i"]);
            ttt2.AssertCF(2, 1, son2CF["ks"]);
            ttt2.AssertCF(3, 1, son2CF["q_caduc"]);

            ttt2 = new TestTheText("examen");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["è"]);
            ttt2.AssertCF(1, 1, son2CF["gz"]);
            ttt2.AssertCF(2, 1, son2CF["a"]);
            ttt2.AssertCF(3, 1, son2CF["m"]);
            ttt2.AssertCF(4, 2, son2CF["5"]);

            ttt2 = new TestTheText("kiwi");
            ttt2.ColorizePhons(conf, PhonConfType.phonemes);
            ttt2.AssertCF(0, 1, son2CF["k"]);
            ttt2.AssertCF(1, 1, son2CF["i"]);
            ttt2.AssertCF(2, 1, son2CF["w"]);
            ttt2.AssertCF(3, 1, son2CF["i"]);

            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            index = ttt.S.IndexOf("papillonnent");
            ttt.AssertCF(index, 1, son2CF["p"]);
            ttt.AssertCF(index + 1, 1, son2CF["a"]);
            ttt.AssertCF(index + 2, 1, son2CF["p"]);
            ttt.AssertCF(index + 3, 1, son2CF["i"]);
            ttt.AssertCF(index + 4, 2, son2CF["j"]);
            ttt.AssertCF(index + 6, 1, son2CF["o"]);
            ttt.AssertCF(index + 7, 2, son2CF["n"]);
            ttt.AssertCF(index + 9, 1, son2CF["q_caduc"]);
            ttt.AssertCF(index + 10, 2, son2CF["_muet"]);
        }

        const string txt47 = 
            @"Tu approuves 1867 donc mon projet ? dit M. de Rênal2, reme35rciant sa
              23femme, p45ar un sourire, de l’excellente31 idée qu’elle venait 
              d’avoir. Allons, voilà qui est déc0123456789idé.";

        [TestMethod]
        public void TestChiffres()
        {
            TestTheText ttt = new TestTheText(txt47);
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            Dictionary<string, CharFormatting> son2CF = SetTestConfig(ccw);

            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            int index = ttt.S.IndexOf("1867");
            ttt.AssertCF(index, son2CF["mil"]);
            ttt.AssertCF(index + 1, son2CF["cen"]);
            ttt.AssertCF(index + 2, son2CF["diz"]);
            ttt.AssertCF(index + 3, son2CF["uni"]);

            index = ttt.S.IndexOf("Rênal");
            ttt.AssertCF(index + 5, son2CF["uni"]);

            index = ttt.S.IndexOf("reme35rciant");
            ttt.AssertCF(index + 4, son2CF["diz"]);
            ttt.AssertCF(index + 5, son2CF["uni"]);

            index = ttt.S.IndexOf("23femme");
            ttt.AssertCF(index, son2CF["diz"]);
            ttt.AssertCF(index + 1, son2CF["uni"]);

            index = ttt.S.IndexOf("p45ar");
            ttt.AssertCF(index + 1, son2CF["diz"]);

            index = ttt.S.IndexOf("déc0123456789idé");
            ttt.AssertCF(index + 3, 6, son2CF["47"]);
            ttt.AssertCF(index + 9, son2CF["mil"]);
            ttt.AssertCF(index + 10, son2CF["cen"]);
            ttt.AssertCF(index + 11, son2CF["diz"]);
            ttt.AssertCF(index + 12, son2CF["uni"]);
        }

        [TestMethod]
        public void TestOutOfRange()
        {
            ColConfWin ccw = conf.colors[PhonConfType.phonemes];
            Assert.ThrowsException<KeyNotFoundException>(() => ccw.GetCheck("farfelu"));
            Assert.ThrowsException<ArgumentNullException>(() => ccw.GetCheck(null));

            Assert.ThrowsException<KeyNotFoundException>(() => ccw.GetCF("farfelu"));
            Assert.ThrowsException<ArgumentNullException>(() => ccw.GetCF(null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() 
                => ccw.SetCbxAndCF("farfelu", CharFormatting.BlackCF));
            Assert.ThrowsException<ArgumentNullException>(() 
                => ccw.SetCbxAndCF(null, CharFormatting.BlackCF));
            Assert.ThrowsException<ArgumentNullException>(() 
                => ccw.SetCbxAndCF("a", null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ccw.ClearSon("farfelu"));
            Assert.ThrowsException<ArgumentNullException>(() => ccw.ClearSon(null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() 
                => ccw.SetCFSon("farfelu", CharFormatting.BlackCF));
            Assert.ThrowsException<ArgumentNullException>(() 
                => ccw.SetCFSon(null, CharFormatting.BlackCF));
            Assert.ThrowsException<ArgumentNullException>(() 
                => ccw.SetCFSon("a", null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => ccw.SetChkSon("farfelu", false));
            Assert.ThrowsException<ArgumentNullException>(() => ccw.SetChkSon(null, true));
        }
    }
}
