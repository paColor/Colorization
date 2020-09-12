using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ColorLibTest
{
    [TestClass]
    public class AutomRuleFilter_Test
    {
        [TestMethod]
        public void TestChaineSansAccents()
        {
            AutomRuleFilter.InitAutomat();
            string tst = "asdéertôjnüjjguü è asd ï ùuuöá - ÉàaqÀÏbghtFREÔç";
            string result = AutomRuleFilter.ChaineSansAccents(tst);
            Assert.AreEqual("asdeertojnujjguu e asd i uuuoa - ÉaaqÀÏbghtFREÔç", result);
            string tstS = tst.ToLower(ConfigBase.cultF);
            result = AutomRuleFilter.ChaineSansAccents(tstS);
            Assert.AreEqual("asdeertojnujjguu e asd i uuuoa - eaaqaibghtfreoç", result, "Chaîne sans accents!");
        }

        [TestMethod]
        public void Test_regle_ient()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(true, AutomRuleFilter.Regle_ient("démultiplient", 9));
            Assert.AreEqual(true, AutomRuleFilter.Regle_ient("revérifient", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ient("rient", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ient("démultiplient", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ient("bonjour", 4));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ient("ie", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ient("e", 0));

        }

        [TestMethod]
        public void Test_regle_mots_ent()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("parent", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("vent", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("ent", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("parents", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("parenté", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("latents", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("latents", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("revérifient", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("rient", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("impertinent", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("impertinent", 8));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("bonjour", 4));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("lents", 1));
            Assert.AreEqual(true, AutomRuleFilter.Regle_mots_ent("zent", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_mots_ent("e", 0));

        }

        [TestMethod]
        public void Test_Regle_ment()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(true, AutomRuleFilter.Regle_ment("rarement", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_ment("finalement", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("rarement", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_ment("ment", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("ent", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("gentil", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("rient", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("impertinent", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("impertinent", 9));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("e", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_ment("lents", 1));
            Assert.AreEqual(true, AutomRuleFilter.Regle_ment("joliment", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_ment("amplement", 6));


        }

        [TestMethod]
        public void Test_Regle_verbe_mer()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("rarement", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_verbe_mer("aiment", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_verbe_mer("rament", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("ment", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("ent", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("gentil", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("rient", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("impertinent", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_verbe_mer("impriment", 6));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("e", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("lents", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_verbe_mer("joliment", 5));

        }

        [TestMethod]
        public void Test_Regle_er()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("rarement", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_er("amer", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_er("amers", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_er("révolvers", 6));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("revolvers", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("alemers", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("er", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("aimer", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("longer", 4));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("impertinent", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("e", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("fée", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_er("joliment", 5));

        }

        [TestMethod]
        public void Test_Regle_nc_ai_final()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("mangeai", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_nc_ai_final("papegai", 5));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("papegai", 1));
            Assert.AreEqual(true, AutomRuleFilter.Regle_nc_ai_final("déblai", 4));
            Assert.AreEqual(true, AutomRuleFilter.Regle_nc_ai_final("essai", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("essa", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("essais", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("ai", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("aimer", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("impertinent", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("a", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_nc_ai_final("jolimai", 5));
        }

        [TestMethod]
        public void Test_Regle_avoir()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("eure", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_avoir("eue", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_avoir("eussiez", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_avoir("eûtes", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_avoir("eûmes", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_avoir("eusses", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("eusses", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("essa", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("essais", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("e", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_avoir("a", 0));
        }

        [TestMethod]
        public void Test_Regle_s_final()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_s_final("jours", 4));
            Assert.AreEqual(true, AutomRuleFilter.Regle_s_final("hypothalamus", 11));
            Assert.AreEqual(true, AutomRuleFilter.Regle_s_final("mérinos", 6));
            Assert.AreEqual(true, AutomRuleFilter.Regle_s_final("hélas", 4));
            Assert.AreEqual(true, AutomRuleFilter.Regle_s_final("forceps", 6));
            Assert.AreEqual(true, AutomRuleFilter.Regle_s_final("pédibus", 6));
            Assert.AreEqual(false, AutomRuleFilter.Regle_s_final("eusses", 5));
            Assert.AreEqual(false, AutomRuleFilter.Regle_s_final("essa", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_s_final("essais", 5));
        }

        [TestMethod]
        public void Test_SansSFinal()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual("jour", AutomRuleFilter.SansSFinal("jours"));
            Assert.AreEqual("hypothalamu", AutomRuleFilter.SansSFinal("hypothalamus"));
            Assert.AreEqual("", AutomRuleFilter.SansSFinal("s"));
            Assert.AreEqual("valise", AutomRuleFilter.SansSFinal("valise"));
            Assert.AreEqual("a", AutomRuleFilter.SansSFinal("a"));
        }

        [TestMethod]
        public void Test_Regle_t_final()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(false, AutomRuleFilter.Regle_t_final("acessits", 6));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("accessit", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("accessits", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("ut", 1));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("ruts", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("prétérit", 7));
            Assert.AreEqual(false, AutomRuleFilter.Regle_t_final("put", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("uppercut", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_t_final("fat", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_t_final("t", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_t_final("raté", 2));
        }

        [TestMethod]
        public void Test_Regle_tien()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("abstiennes", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("appartiendrai", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("soutiens", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("kantien", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("proustien", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("tien", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("détiendraient", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("entretiendrez", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("tiendraient", 0));
            Assert.AreEqual(true, AutomRuleFilter.Regle_tien("retiendront", 2));

            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("titien", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("titien", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("entretiens", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("t", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("raté", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("entretient", 9));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("martien", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("béotiens", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("entretiendrez", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("haïtien", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("tir", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("rit", 2));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("été", 1));
            Assert.AreEqual(false, AutomRuleFilter.Regle_tien("proustie", 5));
        }

        [TestMethod]
        public void Test_Regle_finD()
        {
            AutomRuleFilter.InitAutomat();
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("apartheid", 8));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("aïd", 2));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("bleds", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("caïds", 3));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("polaroid", 7));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("tabloïd", 6));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("barouds", 5));
            Assert.AreEqual(true, AutomRuleFilter.Regle_finD("plaid", 4));

            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("badaud", 5));
            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("bord", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("bonds", 3));
            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("d", 0));
            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("chauds", 4));
            Assert.AreEqual(false, AutomRuleFilter.Regle_finD("chaudement", 4));
        }

        /********************************************
         * Let's test the constructor
         * ******************************************/

        [TestMethod]
        public void Test_AutomRuleFilter()
        {
            AutomRuleFilter arf;
            int pos;
            string tstRFS;
            string rTxt;
            Regex r;

            AutomRuleFilter.InitAutomat();

            // ****************************** TEST 1 **************************************
            tstRFS = @"{ '+':/$/ i, '-':/ ^/ i }";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('}', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \^\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \$\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: True\r\n"));

            // ****************************** TEST 2 **************************************
            tstRFS = @"{ '-':/ ^/ i, '+':/ st$/ i }";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('}', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \^\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \^st\$\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: True\r\n"));

            // ****************************** TEST 3 **************************************
            tstRFS = @"this.Regle_nc_ai_final,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('l', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"crf: Regle_nc_ai_final\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));

            // ****************************** TEST 4 **************************************
            tstRFS = @"{'+':/(')/i}";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('}', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"crf: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \^\('\)\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));

            // ****************************** TEST 5 **************************************
            tstRFS = @"this.Regle_ient,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('t', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"crf: Regle_ient\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));

            // ****************************** TEST 6 **************************************
            tstRFS = @"{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i}";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            Assert.AreEqual('}', tstRFS[pos]);
            rTxt = arf.ToString();
            StringAssert.Matches(rTxt, new Regex(@"crf: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \[bcçdfghjklmnpqrstvwxz]\(u\?\)\$\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \^ll\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));

        } // Test_AutomRuleFilter

        [TestMethod]
        public void Test_SpeedComplexe()
        {
            AutomRuleFilter arf;
            int pos;
            string tstRFS;
            TheText tt;
            List<PhonWord> pws;
            int limit = 1;

            TheText.Init();
            Config conf = new Config();

            tstRFS = @"{'+':/(ur|ag|isi|estp|ei)/i}";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("l'automne et les Violons");
            pws = tt.GetPhonWordList(conf);
            for (int i=0; i<limit; i++)
            {
                pos = 3;
                _ = arf.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
            }
        }

        [TestMethod]
        public void Test_SpeedSimple()
        {
            AutomRuleFilter arf1, arf2, arf3, arf4, arf5;
            int pos;
            string tstRFS1, tstRFS2, tstRFS3, tstRFS4, tstRFS5;
            TheText tt;
            List<PhonWord> pws;
            int limit = 1;

            TheText.Init();
            Config conf = new Config();

            tstRFS1 = @"{'+':/(ur)/i}";
            tstRFS2 = @"{'+':/(ag)/i}";
            tstRFS3 = @"{'+':/isi/i}";
            tstRFS4 = @"{'+':/(estp)/i}";
            tstRFS5 = @"{'+':/(ei)/i}";
            pos = 0;
            arf1 = new AutomRuleFilter(tstRFS1, ref pos);
            pos = 0;
            arf2 = new AutomRuleFilter(tstRFS2, ref pos);
            pos = 0;
            arf3 = new AutomRuleFilter(tstRFS3, ref pos);
            pos = 0;
            arf4 = new AutomRuleFilter(tstRFS4, ref pos);
            pos = 0;
            arf5 = new AutomRuleFilter(tstRFS5, ref pos);

            tt = new TheText("l'automne et les Violons");
            pws = tt.GetPhonWordList(conf);

            for (int i = 0; i < limit; i++)
            {
                pos = 3;
                _ = arf1.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
                pos = 3;
                _ = arf2.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
                pos = 3;
                _ = arf3.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
                pos = 3;
                _ = arf4.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
                pos = 3;
                _ = arf5.Check(pws[4], pos, FP(pws[4], pos), SP(pws[4], pos));
            }
        }


        /********************************************
         * Let's test the Check Method
         * ******************************************/


        private string FP(PhonWord pw, int pos)
        // First Part
        {
            return pw.GetWord().Substring(0, pos);
        }

        private string SP(PhonWord pw, int pos)
        // Second Part
        {
            string secondPart;
            string w = pw.GetWord();
            if (pos == w.Length - 1)
                secondPart = "";
            else
                secondPart = w.Substring(pos + 1, w.Length - (pos + 1));
            return secondPart;
        }


        [TestMethod]
        public void TestCheck1()
        {
            // Let's reuse the rulefilters of the Test_AutomRuleFilter method
            AutomRuleFilter arf;
            int pos;
            string tstRFS;
            string rTxt;
            bool cRes;
            TheText tt;
            PhonWord pw;
            List<PhonWord> pws;
            int wordI;

            TheText.Init();
            Config conf = new Config();

            // ****************************** TEST 1 **************************************
            tstRFS = @"{ '+':/$/ i, '-':/ ^/ i }";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("l'automne et les Violons");
            pw = new PhonWord(tt, 0, 0, conf); // "l"
            Assert.IsTrue(arf.Check(pw, 0, "", ""));
            pw = new PhonWord(tt, 17, 23, conf); // Violons
            Assert.AreEqual("violons", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 1, "v", "olons"));

            // ****************************** TEST 2 **************************************
            tstRFS = @"{ '-':/ ^/ i, '+':/st$/ i }";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("le chst est bleu");
            pw = new PhonWord(tt, 8, 10, conf); // "est"
            Assert.AreEqual("est", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "st"));
            Assert.IsFalse(arf.Check(pw, 1, "e", "t"));
            pw = new PhonWord(tt, 3, 6, conf); // "chst"
            Assert.AreEqual("chst", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 1, "c", "st"));
            Assert.IsFalse(arf.Check(pw, 0, "", "ast"));
            pw = new PhonWord(tt, 12, 15, conf); // "bleu"
            Assert.AreEqual("bleu", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 2, "bl", "u"));
            Assert.IsFalse(arf.Check(pw, 3, "ble", ""));

            // ****************************** TEST 3 **************************************
            tstRFS = @"this.Regle_nc_ai_final,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("Je mangeai chantais plaisir balai essai");
            pw = new PhonWord(tt, 0, 1, conf); // "je"
            Assert.AreEqual("je", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 1, "j", ""));
            pw = new PhonWord(tt, 3, 9, conf); // "mangeai"
            Assert.AreEqual("mangeai", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 0, "", "angeai"));
            Assert.IsFalse(arf.Check(pw, 1, "m", "ngeai"));
            Assert.IsFalse(arf.Check(pw, 2, "ma", "geai"));
            Assert.IsFalse(arf.Check(pw, 3, "man", "eai"));
            Assert.IsFalse(arf.Check(pw, 4, "mang", "ai"));
            Assert.IsFalse(arf.Check(pw, 5, "mange", "i"));
            Assert.IsFalse(arf.Check(pw, 6, "mangea", ""));
            pw = new PhonWord(tt, 11, 18, conf); // "chantais"
            Assert.AreEqual("chantais", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 5, "chant", "is"));
            pw = new PhonWord(tt, 20, 26, conf); // "plaisir"
            Assert.AreEqual("plaisir", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 2, "pl", "isir"));
            pw = new PhonWord(tt, 28, 32, conf); // "balai"
            Assert.AreEqual("balai", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 3, "bal", "i"));
            Assert.IsFalse(arf.Check(pw, 1, "b", "lai"));
            pw = new PhonWord(tt, 34, 38, conf); // "essai"
            Assert.AreEqual("essai", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 3, "ess", "i"));
            Assert.IsFalse(arf.Check(pw, 1, "e", "sai"));
            Assert.IsFalse(arf.Check(pw, 4, "essa", ""));

            // ****************************** TEST 4 **************************************
            tstRFS = @"{'+':/('|’)/i}";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("j'aimé l'arbre d'abord t'es c'est");
            pw = new PhonWord(tt, 0, 1, conf); // "j'"
            Assert.AreEqual("j\'", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "\'"));
            Assert.IsFalse(arf.Check(pw, 1, "j", ""));
            pw = new PhonWord(tt, 7, 8, conf); // "l'"
            Assert.AreEqual("l\'", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "\'"));
            Assert.IsFalse(arf.Check(pw, 1, "l", ""));
            pw = new PhonWord(tt, 15, 16, conf); // "d'"
            Assert.AreEqual("d\'", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "\'"));
            Assert.IsFalse(arf.Check(pw, 1, "d", ""));
            pw = new PhonWord(tt, 23, 24, conf); // "t'"
            Assert.AreEqual("t\'", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "\'"));
            Assert.IsFalse(arf.Check(pw, 1, "t", ""));
            pw = new PhonWord(tt, 28, 29, conf); // "c'"
            Assert.AreEqual("c\'", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 0, "", "\'"));
            Assert.IsFalse(arf.Check(pw, 1, "c", ""));
            pw = new PhonWord(tt, 2, 5, conf); // "aimé"
            Assert.AreEqual("aimé", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 1, "a", "mé"));
            Assert.IsFalse(arf.Check(pw, 2, "aim", ""));

            // ****************************** TEST 6 **************************************
            tstRFS = @"{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i}";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("briefing, berlingot, bille, imbécile, limbe, afin, paille, triage, guilleret");
            pws = tt.GetPhonWordList(conf);

            // briefing
            wordI = 0;
            pos = 5;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // berlingot
            wordI = 1;
            pos = 4;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // bille
            wordI = 2;
            pos = 1;
            Assert.IsTrue(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // imbécile
            wordI = 3;
            pos = 5;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // limbe
            wordI = 4;
            pos = 1;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // afin
            wordI = 5;
            pos = 2;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // paille
            wordI = 6;
            pos = 2;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // triage
            wordI = 7;
            pos = 2;
            Assert.IsFalse(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

            // guilleret
            wordI = 8;
            pos = 2;
            Assert.IsTrue(arf.Check(pws[wordI], pos, FP(pws[wordI], pos), SP(pws[wordI], pos)));

        }

        [TestMethod]
        public void TestCheck2()
            // les Regles. Vérifions qu'elles fonctionnent.
            // Chaque règle est testée en détail ailleurs.
        {
            AutomRuleFilter arf;
            int pos;
            string tstRFS;
            string rTxt;
            bool cRes;
            TheText tt;
            PhonWord pw;

            TheText.Init();
            Config conf = new Config();

            tstRFS = @"this.Regle_ient,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("glorifient quotient rififi mortifient qui i");
            pw = new PhonWord(tt, 0, 9, conf); // "glorifient'"
            Assert.AreEqual("glorifient", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 6, "glorif", "ent"));
            Assert.IsTrue(arf.Check(pw, 8, "glorifie", "t"));
            pw = new PhonWord(tt, 11, 18, conf); // "quotient'"
            Assert.AreEqual("quotient", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 4, "quot", "ent"));
            pw = new PhonWord(tt, 20, 25, conf); // "rififi'"
            Assert.AreEqual("rififi", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 3, "rif", "fi"));
            pw = new PhonWord(tt, 27, 36, conf); // "mortifient'"
            Assert.AreEqual("mortifient", pw.GetWord());
            Assert.IsTrue(arf.Check(pw, 6, "mortif", "ent"));
            Assert.IsFalse(arf.Check(pw, 4, "mort", "fient"));
            pw = new PhonWord(tt, 38, 40, conf); // "qui'"
            Assert.AreEqual("qui", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 2, "qu", ""));
            pw = new PhonWord(tt, 42, 42, conf); // "i'"
            Assert.AreEqual("i", pw.GetWord());
            Assert.IsFalse(arf.Check(pw, 0, "", ""));


            tstRFS = @"this.Regle_mots_ent,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("interférent");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 8, "interfér", "nt"));

            tstRFS = @"this.Regle_ment,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("assurément");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 7, "assurém", "nt"));
            tt = new TheText("amplement");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 6, "amplem", "nt"));


            tstRFS = @"this.Regle_verbe_mer,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("clament");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 4, "clam", "nt"));

            tstRFS = @"this.Regle_er,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("amer");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 2, "am", "r"));

            tstRFS = @"this.Regle_avoir,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("eûmes");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 0, "", "ûmes"));

            tstRFS = @"this.Regle_s_final,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("versus");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 5, "versu", ""));

            tstRFS = @"this.Regle_t_final,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("comput");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 5, "compu", ""));

            tstRFS = @"this.Regle_tien,";
            pos = 0;
            arf = new AutomRuleFilter(tstRFS, ref pos);
            tt = new TheText("tienne");
            pw = new PhonWord(tt, 0, tt.ToString().Length-1, conf);
            Assert.IsTrue(arf.Check(pw, 0, "", "ienne"));

        }

    } // class
} // namespace
