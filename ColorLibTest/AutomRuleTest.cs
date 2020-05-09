using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace ColorLibTest
{
    [TestClass]
    public class AutomRuleTest
    {
        [TestMethod]
        public void TestAutomRule()
        {
            AutomRule ar;
            int pos;
            string tstAR;
            string rTxt;
            Regex r;
            List<string> vRN = new List<string> { "chr", "in", "_ent" };
            vRN.Sort();

            AutomRule.InitAutomat();

            // ****************************** TEST 1 **************************************
            tstAR = @"'chr':[{'+':/hr/i},'k',2] // de chrétien à synchronisé";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            Assert.AreEqual(']', tstAR[pos]);
            rTxt = ar.ToString();
            StringAssert.Matches(rTxt, new Regex(@"RuleName: chr\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"rf: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \^hr\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"p: k\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"incr: 2\r\n"));

            // ****************************** TEST 2 **************************************
            tstAR = @"'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3] // toute succession 'ein' 'eim' suivie d'une consonne ou d'une fin de mot";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            Assert.AreEqual(']', tstAR[pos]);
            rTxt = ar.ToString();
            StringAssert.Matches(rTxt, new Regex(@"RuleName: in\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"rf: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \^i\[nm]\(\[bcçdfghjklnmpqrstvwxz]\|\$\)\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"p: e_tilda\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"incr: 3\r\n"));

            // ****************************** TEST 3 **************************************
            tstAR = @"'_ent':[this.Regle_mots_ent,'a_tilda',2] // quelques mots (adverbes ou noms) terminés par ent";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            Assert.AreEqual(']', tstAR[pos]);
            rTxt = ar.ToString();
            StringAssert.Matches(rTxt, new Regex(@"RuleName: _ent\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"rf: Regle_mots_ent\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"p: a_tilda\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"incr: 2\r\n"));

            // ****************************** TEST 4 **************************************
            tstAR = @"'_ent':[this.Regle_mots_ent,'faux',2] // quelques mots (adverbes ou noms) terminés par ent";
            pos = 0;
            try
            {
                ar = new AutomRule(tstAR, ref pos, vRN);
                Assert.IsTrue(false); // should never be reached
            }
            catch (System.ArgumentException e)
            {
                StringAssert.Contains(e.Message, "faux");
            }

            // ****************************** TEST 5 **************************************
            tstAR = @"'_ent':[this.Regle_mots_ent,'a_tilda',2,IllCeras] 
                // quelques mots (adverbes ou noms) terminés par ent, avec flag";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            Assert.AreEqual(']', tstAR[pos]);
            rTxt = ar.ToString();
            StringAssert.Matches(rTxt, new Regex(@"RuleName: _ent\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"rf: Regle_mots_ent\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"prevRegex: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"follRegEx: \r\n"));
            StringAssert.Matches(rTxt, new Regex(@"isFirstLetter: False\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"p: a_tilda\r\n"));
            StringAssert.Matches(rTxt, new Regex(@"incr: 2\r\n"));

        } // TestAutomRule

        private string FP (PhonWord pw, int pos)
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
        public void TestTryApplyRule()
        {
            AutomRule ar;
            int pos;
            string tstAR;
            TheText tt;
            List<string> vRN = new List<string> { "chr", "in", "_ent" };
            vRN.Sort();
            List<PhonWord> pws;
            int wordI;
            bool result;

            AutomRule.InitAutomat();

            // ****************************** TEST 1 **************************************
            tstAR = @"'chr':[{'+':/hr/i},'k',2] // de chrétien à synchronisé";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            tt = TheText.NewTestTheText("chrétien, synchronisé, chien, apache, roch, rocher, cornichon");
            pws = tt.GetPhonWords();
            foreach (PhonWord pw in pws)
                pw.ClearPhons();

            // chrétien
            wordI = 0;
            pos = 0;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(2, pos);
            Assert.AreEqual("k", pws[wordI].Phonetique());

            // synchronisé
            wordI = 1;
            pos = 3;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("k", pws[wordI].Phonetique());

            // chien
            wordI = 2;
            pos = 0;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(0, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // apache
            wordI = 3;
            pos = 3;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(3, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // roch
            wordI = 4;
            pos = 2;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(2, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // rocher
            wordI = 5;
            pos = 2;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(2, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // cornichon
            wordI = 6;
            pos = 5;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());


            // ****************************** TEST 2 **************************************

            tstAR = @"'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3] // toute succession 'ein' 'eim' suivie d'une consonne ou d'une fin de mot";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            tt = TheText.NewTestTheText("plein, geindre, weimarienne, astreignant, atteint, autoneige, palme");
            pws = tt.GetPhonWords();
            foreach (PhonWord pw in pws)
                pw.ClearPhons();

            // plein
            wordI = 0;
            pos = 2;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // geindre
            wordI = 1;
            pos = 1;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // weimarienne
            wordI = 2;
            pos = 1;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(1, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // astreignant
            wordI = 3;
            pos = 4;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // atteint
            wordI = 4;
            pos = 3;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(6, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // autoneige
            wordI = 5;
            pos = 5;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // palme
            wordI = 6;
            pos = 4;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // ****************************** TEST 3 **************************************
            tstAR = @"'_ent':[this.Regle_mots_ent,'a_tilda',2] // quelques mots (adverbes ou noms) terminés par ent";
            pos = 0;
            ar = new AutomRule(tstAR, ref pos, vRN);
            tt = TheText.NewTestTheText("indécent, triment, palme");
            pws = tt.GetPhonWords();
            foreach (PhonWord pw in pws)
                pw.ClearPhons();

            // indécent
            wordI = 0;
            pos = 5;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsTrue(result);
            Assert.AreEqual(7, pos);
            Assert.AreEqual("@", pws[wordI].Phonetique());

            // indécent
            wordI = 0;
            pos = 3;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(3, pos);
            Assert.AreEqual("@", pws[wordI].Phonetique()); // @ reste du test précédent. Rien ne lui est ajouté...

            // triment
            wordI = 1;
            pos = 4;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

            // palme
            wordI = 2;
            pos = 4;
            result = ar.TryApplyRule(pws[wordI], ref pos, FP(pws[wordI], pos), SP(pws[wordI], pos));
            Assert.IsFalse(result);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("", pws[wordI].Phonetique());

        } // TestTryApplyRule
    }
}
