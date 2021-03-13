using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using ColorLibTest.ConfigTest;
using System;
using NLog;

namespace ColorLibTest.Undo
{
    [TestClass]
    public class UndoTests
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
            // nLogConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);  // everything equal or higher than Debug
            // nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.UndoFactory*");
            // nLogConfig.AddRule(LogLevel.Trace, LogLevel.Trace, logconsole, "ColorLib.ColConfWin*");
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
            ccw.SetChkSon("é", false);
            Assert.IsFalse(ccw.GetCheck("é"));
            UndoFactory.UndoLastAction();
            Assert.IsTrue(ccw.GetCheck("é"));
        }

        [TestMethod]
        public void TestColConfWinUndo_2()
        {
            ColConfWin ccw = new ColConfWin(PhonConfType.phonemes);
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);

            Assert.AreEqual(ColConfWin.DefBeh.transparent, ccw.defBeh);
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.noir);
            Assert.AreEqual(ColConfWin.DefBeh.noir, ccw.defBeh);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWin.DefBeh.transparent, ccw.defBeh);
        }

        [TestMethod]
        public void TestColConfWinUndo_3()
        {
            ColConfWin ccw = new ColConfWin(PhonConfType.phonemes);
            ccw.SetCeras();
            Assert.AreEqual(ColConfWinTest.cfAN, ccw.GetCF("an"));
            ccw.SetCbxAndCF("m", ColConfWinTest.cfBLEUCLAIR);
            Assert.AreEqual(ColConfWinTest.cfBLEUCLAIR, ccw.GetCF("m"));
            Assert.IsTrue(ccw.GetCheck("m"));
            ccw.SetCFSon("m", ColConfWinTest.cfON);
            Assert.AreEqual(ColConfWinTest.cfON, ccw.GetCF("m"));
            ccw.IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            ccw.SetDefaultBehaviourTo(ColConfWin.DefBeh.noir);
            Assert.AreEqual(ColConfWin.DefBeh.noir, ccw.defBeh);
            ccw.Reset();
            Assert.AreEqual(ColConfWinTest.cfeRose, ccw.GetCF("é"));
            Assert.IsFalse(ccw.GetCheck("m"));
            Assert.AreEqual(ColConfWin.IllRule.ceras, ccw.IllRuleToUse);
            Assert.AreEqual(ColConfWin.DefBeh.transparent, ccw.defBeh);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(ColConfWinTest.cfON, ccw.GetCF("m"));
            Assert.IsTrue(ccw.GetCheck("m"));
            Assert.AreEqual(ColConfWin.IllRule.lirecouleur, ccw.IllRuleToUse);
            Assert.AreEqual(ColConfWin.DefBeh.noir, ccw.defBeh);
        }

        [TestMethod]
        public void TestArcConfigUndo_1()
        {
            ArcConfig ac = new ArcConfig();
            Assert.IsTrue(ac.GetABClickable(0));
            Assert.IsTrue(ac.GetABClickable(1));
            Assert.IsFalse(ac.GetABClickable(2));
            Assert.IsFalse(ac.GetABClickable(3));
            Assert.IsFalse(ac.GetABClickable(4));
            Assert.IsFalse(ac.GetABClickable(5));
            Assert.AreEqual(ColConfWin.predefinedColors[(int)PredefCol.darkBlue], ac.GetABColor(0));
            ac.SetArcButtonCol(1, TestTheText.col01);
            Assert.AreEqual(TestTheText.col01, ac.GetABColor(1));
            Assert.IsTrue(ac.GetABClickable(1));
            ac.SetArcButtonCol(2, TestTheText.col02);
            Assert.AreEqual(TestTheText.col02, ac.GetABColor(2));
            Assert.IsTrue(ac.GetABClickable(2));
            ac.SetArcButtonCol(3, TestTheText.col03);
            Assert.AreEqual(TestTheText.col03, ac.GetABColor(3));
            Assert.IsTrue(ac.GetABClickable(3));
            ac.SetArcButtonCol(4, TestTheText.col04);
            Assert.AreEqual(TestTheText.col04, ac.GetABColor(4));
            Assert.IsTrue(ac.GetABClickable(4));
            ac.SetArcButtonCol(5, TestTheText.col05);
            Assert.AreEqual(TestTheText.col05, ac.GetABColor(5));
            Assert.IsTrue(ac.GetABClickable(5));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(CharFormatting.neutralArcsCol, ac.GetABColor(5));
            Assert.IsTrue(ac.GetABClickable(5));
            UndoFactory.UndoLastAction();
            Assert.IsFalse(ac.GetABClickable(5));
            Assert.AreEqual(CharFormatting.neutralArcsCol, ac.GetABColor(4));
            Assert.IsTrue(ac.GetABClickable(4));
            ac.Reset();
            Assert.AreEqual(CharFormatting.neutralArcsCol, ac.GetABColor(1));
            Assert.IsTrue(ac.GetABClickable(1));
            Assert.IsFalse(ac.GetABClickable(2));
            Assert.IsFalse(ac.GetABClickable(3));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.col02, ac.GetABColor(2));
            Assert.IsTrue(ac.GetABClickable(2));
            Assert.AreEqual(TestTheText.col03, ac.GetABColor(3));
            Assert.IsTrue(ac.GetABClickable(3));
        }

        [TestMethod]
        public void TestArcConfigUndo_2()
        {
            ArcConfig ac = new ArcConfig();
            ac.Decalage = 0.2f;
            Assert.AreEqual(0.2f, ac.Decalage);
            ac.Ecartement = 50;
            Assert.AreEqual(50, ac.Ecartement);
            ac.Epaisseur = 1.2f;
            Assert.AreEqual(1.2f, ac.Epaisseur);
            ac.Hauteur = 80;
            Assert.AreEqual(80, ac.Hauteur);

            ac.Decalage = 0.3f;
            Assert.AreEqual(0.3f, ac.Decalage);
            ac.Ecartement = 60;
            Assert.AreEqual(60, ac.Ecartement);
            ac.Epaisseur = 1.3f;
            Assert.AreEqual(1.3f, ac.Epaisseur);
            ac.Hauteur = 90;
            Assert.AreEqual(90, ac.Hauteur);

            UndoFactory.UndoLastAction();
            Assert.AreEqual(80, ac.Hauteur);
            Assert.AreEqual(1.3f, ac.Epaisseur);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(1.2f, ac.Epaisseur);
            Assert.AreEqual(60, ac.Ecartement);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(50, ac.Ecartement);
            Assert.AreEqual(0.3f, ac.Decalage);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(0.2f, ac.Decalage);
            Assert.AreEqual(50, ac.Ecartement);
            Assert.AreEqual(1.2f, ac.Epaisseur);
            Assert.AreEqual(80, ac.Hauteur);

            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(0.3f, ac.Decalage);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(60, ac.Ecartement);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(1.3f, ac.Epaisseur);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(90, ac.Hauteur);
        }

        [TestMethod]
        public void TestDuoConfig()
        {
            DuoConfig dc = new DuoConfig();

            Assert.AreEqual(DuoConfig.ColorisFunction.syllabes, dc.colorisFunction);
            dc.colorisFunction = DuoConfig.ColorisFunction.mots;
            Assert.AreEqual(DuoConfig.ColorisFunction.mots, dc.colorisFunction);
            dc.colorisFunction = DuoConfig.ColorisFunction.lettres;
            Assert.AreEqual(DuoConfig.ColorisFunction.lettres, dc.colorisFunction);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(DuoConfig.ColorisFunction.mots, dc.colorisFunction);

            Assert.AreEqual(DuoConfig.Alternance.mots, dc.alternance);
            dc.alternance = DuoConfig.Alternance.lignes;
            Assert.AreEqual(DuoConfig.Alternance.lignes, dc.alternance);
            dc.alternance = DuoConfig.Alternance.mots;
            Assert.AreEqual(DuoConfig.Alternance.mots, dc.alternance);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(DuoConfig.Alternance.lignes, dc.alternance);

            Assert.AreEqual(1, dc.nbreAlt);
            dc.nbreAlt = 2;
            Assert.AreEqual(2, dc.nbreAlt);
            dc.nbreAlt = 3;
            Assert.AreEqual(3, dc.nbreAlt);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(2, dc.nbreAlt);

            dc.Reset();
            Assert.AreEqual(DuoConfig.ColorisFunction.syllabes, dc.colorisFunction);
            Assert.AreEqual(DuoConfig.Alternance.mots, dc.alternance);
            Assert.AreEqual(1, dc.nbreAlt);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(DuoConfig.ColorisFunction.mots, dc.colorisFunction);
            Assert.AreEqual(DuoConfig.Alternance.lignes, dc.alternance);
            Assert.AreEqual(2, dc.nbreAlt);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(DuoConfig.ColorisFunction.syllabes, dc.colorisFunction);
            Assert.AreEqual(DuoConfig.Alternance.mots, dc.alternance);
            Assert.AreEqual(1, dc.nbreAlt);
        }

        [TestMethod]
        public void TestPBDQConfig()
        {
            PBDQConfig pc = new PBDQConfig();
            pc.UpdateLetter(3, 'e', TestTheText.fixCFs[1]);
            Assert.AreEqual(TestTheText.fixCFs[1], pc.GetCfForPBDQLetter('e'));
            Assert.AreEqual('e', pc.GetLetterForButtonNr(3));
            pc.UpdateLetter(4, 'f', TestTheText.fixCFs[2]);
            Assert.AreEqual(TestTheText.fixCFs[2], pc.GetCfForPBDQLetter('f'));
            pc.UpdateLetter(4, 'k', TestTheText.fixCFs[3]);
            Assert.AreEqual(TestTheText.fixCFs[3], pc.GetCfForPBDQLetter('k'));
            Assert.AreEqual(TestTheText.fixCFs[3], pc.GetCfForPBDQButton(4, out _));
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.fixCFs[2], pc.GetCfForPBDQButton(4, out _));
            UndoFactory.UndoLastAction();
            UndoFactory.UndoLastAction();
            Assert.AreEqual('q', pc.GetLetterForButtonNr(3));
            Assert.IsFalse(pc.markAsBlack);
            pc.SetMarkAsBlackTo(true);
            Assert.IsTrue(pc.markAsBlack);
            UndoFactory.UndoLastAction();
            Assert.IsFalse(pc.markAsBlack);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsTrue(pc.markAsBlack);
            pc.UpdateLetter(4, 'k', TestTheText.fixCFs[3]);
            pc.Reset();
            Assert.IsFalse(pc.markAsBlack);
            Assert.AreEqual(' ', pc.GetLetterForButtonNr(4));
            UndoFactory.UndoLastAction();
            Assert.IsTrue(pc.markAsBlack);
            Assert.AreEqual(TestTheText.fixCFs[3], pc.GetCfForPBDQButton(4, out _));
            Assert.AreEqual('k', pc.GetLetterForButtonNr(4));
            UndoFactory.RedoLastCanceledAction();
            Assert.IsFalse(pc.markAsBlack);
            Assert.AreEqual(' ', pc.GetLetterForButtonNr(4));
        }

        private void CheckAllP(PonctConfig pc, CharFormatting cf)
        {
            for (Ponctuation p = Ponctuation.point; p < Ponctuation.lastP; p++)
            {
                Assert.AreEqual(cf, pc.GetCF(p));
            }
        }

        [TestMethod]
        public void TestPonctConfig()
        {
            PonctConfig pc = new PonctConfig();
            CharFormatting origMasterCF = pc.MasterCF;
            CheckAllP(pc, origMasterCF);
            pc.MasterCF = TestTheText.blueCF;
            Assert.AreEqual(TestTheText.blueCF, pc.MasterCF);
            CheckAllP(pc, TestTheText.blueCF);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(origMasterCF, pc.MasterCF);
            CheckAllP(pc, origMasterCF);

            pc.MajDebCB = false;
            pc.MajDebCB = true;
            pc.MajDebCF = TestTheText.blueCF;
            Assert.AreEqual(TestTheText.blueCF, pc.MajDebCF);
            pc.MajDebCF = TestTheText.redCF;
            Assert.AreEqual(TestTheText.redCF, pc.MajDebCF);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.blueCF, pc.MajDebCF);
            UndoFactory.UndoLastAction();
            UndoFactory.UndoLastAction();
            Assert.IsFalse(pc.MajDebCB);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsTrue(pc.MajDebCB);

            pc.SetCF(Ponctuation.point, TestTheText.blueCF);
            Assert.AreEqual(PonctConfig.State.off, pc.MasterState);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(origMasterCF, pc.GetCF(Ponctuation.point));
            Assert.AreEqual(PonctConfig.State.master, pc.MasterState);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(PonctConfig.State.off, pc.MasterState);
            Assert.AreEqual(TestTheText.blueCF, pc.GetCF(Ponctuation.point));
            pc.SetCB(Ponctuation.virgule, false);
            Assert.IsFalse(pc.GetCB(Ponctuation.virgule));
            UndoFactory.UndoLastAction();
            Assert.IsTrue(pc.GetCB(Ponctuation.virgule));
            UndoFactory.RedoLastCanceledAction();
            Assert.IsFalse(pc.GetCB(Ponctuation.virgule));

            pc.Reset();
            Assert.IsTrue(pc.GetCB(Ponctuation.virgule));
            CheckAllP(pc, origMasterCF);
            Assert.IsFalse(pc.MajDebCB);
            UndoFactory.UndoLastAction();
            Assert.IsTrue(pc.MajDebCB);
            Assert.IsFalse(pc.GetCB(Ponctuation.virgule));
            Assert.AreEqual(TestTheText.blueCF, pc.GetCF(Ponctuation.point));
            UndoFactory.RedoLastCanceledAction();
            Assert.IsTrue(pc.GetCB(Ponctuation.virgule));
            CheckAllP(pc, origMasterCF);
            Assert.IsFalse(pc.MajDebCB);
        }

        [TestMethod]
        public void TestSylConfig_1()
        {
            var sc = new SylConfig();
            Assert.IsTrue(sc.GetSylButtonConfFor(0).buttonClickable);
            Assert.IsTrue(sc.GetSylButtonConfFor(1).buttonClickable);
            Assert.IsTrue(sc.GetSylButtonConfFor(2).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(3).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(4).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(5).buttonClickable);
            sc.SetSylButtonCF(0, TestTheText.redCF);
            sc.SetSylButtonCF(1, TestTheText.redCF);
            sc.SetSylButtonCF(2, TestTheText.redCF);
            Assert.AreEqual(TestTheText.redCF, sc.GetSylButtonConfFor(0).cf);
            Assert.AreEqual(TestTheText.redCF, sc.GetSylButtonConfFor(1).cf);
            Assert.AreEqual(TestTheText.redCF, sc.GetSylButtonConfFor(2).cf);
            sc.SetSylButtonCF(1, TestTheText.blueCF);
            Assert.AreEqual(TestTheText.blueCF, sc.GetSylButtonConfFor(1).cf);
            sc.SetSylButtonCF(2, TestTheText.blueCF);
            Assert.AreEqual(TestTheText.blueCF, sc.GetSylButtonConfFor(2).cf);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.redCF, sc.GetSylButtonConfFor(2).cf);
            Assert.AreEqual(TestTheText.blueCF, sc.GetSylButtonConfFor(1).cf);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.redCF, sc.GetSylButtonConfFor(1).cf);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(TestTheText.blueCF, sc.GetSylButtonConfFor(1).cf);

            sc.DoubleConsStd = true;
            sc.DoubleConsStd = false;
            Assert.IsFalse(sc.DoubleConsStd);
            UndoFactory.UndoLastAction();
            Assert.IsTrue(sc.DoubleConsStd);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsFalse(sc.DoubleConsStd);

            sc.mode = SylConfig.Mode.ecrit;
            sc.mode = SylConfig.Mode.oral;
            sc.mode = SylConfig.Mode.poesie;
            Assert.AreEqual(SylConfig.Mode.poesie, sc.mode);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(SylConfig.Mode.oral, sc.mode);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(SylConfig.Mode.ecrit, sc.mode);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(SylConfig.Mode.oral, sc.mode);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(SylConfig.Mode.poesie, sc.mode);

            sc.marquerMuettes = true;
            sc.marquerMuettes = false;
            Assert.IsFalse(sc.marquerMuettes);
            UndoFactory.UndoLastAction();
            Assert.IsTrue(sc.marquerMuettes);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsFalse(sc.marquerMuettes);

            sc.chercherDierese = true;
            sc.chercherDierese = false;
            Assert.IsFalse(sc.chercherDierese);
            UndoFactory.UndoLastAction();
            Assert.IsTrue(sc.chercherDierese);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsFalse(sc.chercherDierese);

            sc.nbrPieds = 12;
            sc.nbrPieds = 8;
            sc.nbrPieds = 9;
            Assert.AreEqual(9, sc.nbrPieds);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(8, sc.nbrPieds);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(12, sc.nbrPieds);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(8, sc.nbrPieds);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(9, sc.nbrPieds);

            sc.Reset();
            Assert.IsTrue(sc.DoubleConsStd);
            Assert.AreEqual(SylConfig.Mode.ecrit, sc.mode);
            Assert.IsTrue(sc.marquerMuettes);
            Assert.IsTrue(sc.chercherDierese);
            Assert.AreEqual(0, sc.nbrPieds);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(TestTheText.blueCF, sc.GetSylButtonConfFor(1).cf);
            Assert.IsFalse(sc.DoubleConsStd);
            Assert.AreEqual(SylConfig.Mode.poesie, sc.mode);
            Assert.IsFalse(sc.marquerMuettes);
            Assert.IsFalse(sc.chercherDierese);
            Assert.AreEqual(9, sc.nbrPieds);
            UndoFactory.RedoLastCanceledAction();
            Assert.IsTrue(sc.DoubleConsStd);
            Assert.AreEqual(SylConfig.Mode.ecrit, sc.mode);
            Assert.IsTrue(sc.marquerMuettes);
            Assert.IsTrue(sc.chercherDierese);
            Assert.AreEqual(0, sc.nbrPieds);
            Assert.IsTrue(sc.GetSylButtonConfFor(0).buttonClickable);
            Assert.IsTrue(sc.GetSylButtonConfFor(1).buttonClickable);
            Assert.IsTrue(sc.GetSylButtonConfFor(2).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(3).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(4).buttonClickable);
            Assert.IsFalse(sc.GetSylButtonConfFor(5).buttonClickable);
        }

        [TestMethod]
        public void TestSylConfig_2()
        {
            var sc = new SylConfig();
            Assert.IsNull(sc.ExcMots);
            var exc1 = new ExceptionMots();
            sc.ExcMots = exc1;
            Assert.AreEqual(exc1, sc.ExcMots);
            UndoFactory.UndoLastAction();
            Assert.IsNull(sc.ExcMots);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(exc1, sc.ExcMots);
            sc.ResetExceptionMots();
            Assert.IsNull(sc.ExcMots);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(exc1, sc.ExcMots);
            var exc2 = new ExceptionMots();
            sc.ExcMots = exc2;
            Assert.AreEqual(exc2, sc.ExcMots);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(exc1, sc.ExcMots);
            UndoFactory.UndoLastAction();
            Assert.IsNull(sc.ExcMots);
        }

        [TestMethod]
        public void TestUnsetBehConf()
        {
            var ubc = new UnsetBehConf();
            for (Ucbx u = Ucbx.bold; u < Ucbx.last; u++)
            {
                ubc.SetCbuFlag(u, true);
                ubc.SetCbuFlag(u, false);
                Assert.IsFalse(ubc.GetCbuFlag(u));
                UndoFactory.UndoLastAction();
                Assert.IsTrue(ubc.GetCbuFlag(u));
                UndoFactory.RedoLastCanceledAction();
                Assert.IsFalse(ubc.GetCbuFlag(u));
                UndoFactory.UndoLastAction();
                Assert.IsTrue(ubc.GetCbuFlag(u));
            }
            ubc.Reset();
            for (Ucbx u = Ucbx.bold; u < Ucbx.last; u++)
            {
                Assert.IsFalse(ubc.GetCbuFlag(u));
            }
            UndoFactory.UndoLastAction();
            for (Ucbx u = Ucbx.bold; u < Ucbx.last; u++)
            {
                Assert.IsTrue(ubc.GetCbuFlag(u));
            }
            UndoFactory.RedoLastCanceledAction();
            for (Ucbx u = Ucbx.bold; u < Ucbx.last; u++)
            {
                Assert.IsFalse(ubc.GetCbuFlag(u));
            }
        }

        [TestMethod]
        public void TestConfig()
        {
            Config c = new Config();
            c.SetConfigName("Name1");
            c.SetConfigName("Name2");
            c.SetConfigName("Name3");
            Assert.AreEqual("Name3", c.GetConfigName());
            UndoFactory.UndoLastAction();
            Assert.AreEqual("Name2", c.GetConfigName());
            UndoFactory.UndoLastAction();
            Assert.AreEqual("Name1", c.GetConfigName());
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual("Name2", c.GetConfigName());
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual("Name3", c.GetConfigName());

            DuoConfig dc1 = new DuoConfig();
            c.duoConf = dc1;
            DuoConfig dc2 = new DuoConfig();
            c.duoConf = dc2;
            DuoConfig dc3 = new DuoConfig();
            c.duoConf = dc3;
            Assert.AreEqual(dc3, c.duoConf);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(dc2, c.duoConf);
            UndoFactory.UndoLastAction();
            Assert.AreEqual(dc1, c.duoConf);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(dc2, c.duoConf);
            UndoFactory.RedoLastCanceledAction();
            Assert.AreEqual(dc3, c.duoConf);

        }
    }
}
