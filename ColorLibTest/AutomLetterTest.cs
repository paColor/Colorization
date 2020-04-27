using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace ColorLibTest
{
    [TestClass]
    public class AutomLetterTest
    {
        [TestMethod]
        public void TestAutomLetter()
        {
            AutomLetter al;
            int pos;
            string tstAL;
            string rTxt;
            Regex r;

            AutomLetter.InitAutomat();
            TheText.Init();

            // ****************************** TEST 1 **************************************
            tstAL = @"'h' : [['*'],
				{'*':[{},'_muet',1]}]";
            pos = 0;
            al = new AutomLetter(tstAL, ref pos);
            Assert.AreEqual(']', tstAL[pos]);
            rTxt = al.ToString();
            r = new Regex(@"letter: h\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"ruleOrder:.*\*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"rf: \r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"prevRegex: \r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"follRegEx: \r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"isFirstLetter: False\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"p: _muet\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"incr: 1\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));

            // ****************************** TEST 2 **************************************
            tstAL = @"'f' : [['f','oeufs', '*'],
				{'f':[{'+':/f/i},'f',2],
				 'oeufs':[{'-':/(oeu|œu)/i,'+':/s/i},'_muet',1], // oeufs et boeufs
				 '*':[{},'f',1]}]";
            pos = 0;
            al = new AutomLetter(tstAL, ref pos);
            Assert.AreEqual(']', tstAL[pos]);
            rTxt = al.ToString();
            r = new Regex(@"letter: f\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"ruleOrder:.*f.*oeufs.*\*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: f\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*rf: \r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*prevRegex: \r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*follRegEx: \^f\r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*isFirstLetter: False\r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*p: f\r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: f.*incr: 2\r\n.*RuleName: oeufs", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: oeufs\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*rf: \r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*prevRegex: \(oeu\|œu\)\$\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*follRegEx: \^s\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*isFirstLetter: False\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*p: _muet\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: oeufs.*incr: 1\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: \*\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*rf: \r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*prevRegex: \r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*follRegEx: \r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*isFirstLetter: False\r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*p: f\r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: \*.*incr: 1\r\n", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            // ****************************** TEST 3 ******************************************************************
            tstAL = @"'i' : [['ing','n','m','nm','prec_2cons','lldeb','vill','mill','tranquille',
				'ill','@ill','@il','ll','ui','ient_1','ient_2','ie','i_voyelle', '*'],
				{'ing':[{'-':/[bcçdfghjklmnpqrstvwxz]/i,'+':/ng$/i},'i',1],
				'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
				'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
				'nm':[{'+':/[n|m]$/i},'e_tilda',2],
                'prec_2cons':[{'-':/[ptkcbdgfv][lr]/i},'i',1], // précédé de 2 consonnes (en position 3), doit apparaître comme [ij]
				'lldeb':[{'-':/^/i,'+':/ll/i},'i',1],
				'vill':[{'-':/v/i,'+':/ll/i},'i',1],
				'mill':[{'-':/m/i,'+':/ll/i},'i',1],
				'tranquille':[{'-':/tranqu/i,'+':/ll/i},'i',1],
				'ill':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i',1], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
				'@ill':[{'-':/[aeo]/i,'+':/ll/i},'j',3], // par défaut précédé d'une voyelle et suivi de 'll' donne le son [j]
				'@il':[{'-':/[aeou]/i,'+':/l(s?)$/i},'j',2], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [j]
				'll':[{'+':/ll/i},'j',3], // par défaut avec ll donne le son [j]
				'ui':[{'-':/u/i,'+':/ent/i},'i',1], // essuient, appuient
				'ient_1':[this.Regle_ient,'i',1], // règle spécifique pour différencier les verbes du premier groupe 3ème pers pluriel
				'ient_2':[{'+':/ent(s)?$/i},'j',1], // si la règle précédente ne fonctionne pas
				'ie':[{'+':/e(s|nt)?$/i},'i',1], // mots terminés par -ie(s|nt)
				'i_voyelle':[{'+':/[aäâeéèêëoôöuù]/i},'j',1], // i suivi d'une voyelle donne [j]
				'*':[{},'i',1]}]";
            pos = 0;
            al = new AutomLetter(tstAL, ref pos);
            Assert.AreEqual(']', tstAL[pos]);
            rTxt = al.ToString();
            r = new Regex(@"letter: i\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"ruleOrder:.*ing.*n.*m.*nm.*prec_2cons.*lldeb.*vill.*mill.*tranquille.*ill.*@ill.*@il.*ll.*ui.*ient_1.*ient_2.*ie.*i_voyelle.*\*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: m\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*rf: \r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*prevRegex: \r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*follRegEx: \^m\[bcçdfghjklnpqrstvwxz]\r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*isFirstLetter: False\r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*p: e_tilda\r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: m.*incr: 2\r\n.*RuleName: nm", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: tranquille\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*rf: \r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*prevRegex: tranqu\$\r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*follRegEx: \^ll\r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*isFirstLetter: False\r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*p: i\r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: tranquille.*incr: 1\r\n.*RuleName: ill", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: ient_1\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*rf: Regle_ient\r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*prevRegex: \r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*follRegEx: \r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*isFirstLetter: False\r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*p: i\r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: ient_1.*incr: 1\r\n.*RuleName: ient_2", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

            r = new Regex(@"RuleName: i_voyelle\r\n");
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*rf: \r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*prevRegex: \r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*follRegEx: \^\[aäâeéèêëoôöuù]\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*isFirstLetter: False\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*p: j\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));
            r = new Regex(@"RuleName: i_voyelle.*incr: 1\r\n.*RuleName: \*", RegexOptions.Singleline);
            Assert.IsTrue(r.IsMatch(rTxt));

        }

        [TestMethod]
        public void TestFireRuleI ()
        {
            AutomLetter al;
            int pos;
            string tstAL;
            TheText tt;
            List<PhonWord> pws;
            int wordI;
            string rTxt;

            ColConfWin.Init();
            TheText.Init();

            tstAL = @"'i' : [['ing','n','m','nm','prec_2cons','lldeb','vill','mill','tranquille',
				'ill','@ill','@il','ll','ui','ient_1','ient_2','ie','i_voyelle', '*'],
				{'ing':[{'-':/[bcçdfghjklmnpqrstvwxz]/i,'+':/ng$/i},'i',1],
				'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
				'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
				'nm':[{'+':/[n|m]$/i},'e_tilda',2],
                'prec_2cons':[{'-':/[ptkcbdgfv][lr]/i, '+':/[aäâeéèêëoôöuù]/i},'i_j',1], // précédé de 2 consonnes (en position 3), doit apparaître comme [ij] PAE: rajouté les voyelles
				'lldeb':[{'-':/^/i,'+':/ll/i},'i',1],
				'vill':[{'-':/v/i,'+':/ll/i},'i',1],
				'mill':[{'-':/m/i,'+':/ll/i},'i',1],
				'tranquille':[{'-':/tranqu/i,'+':/ll/i},'i',1],
				'ill':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i',1], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
				'@ill':[{'-':/[aeo]/i,'+':/ll/i},'j',3], // par défaut précédé d'une voyelle et suivi de 'll' donne le son [j]
				'@il':[{'-':/[aeou]/i,'+':/l(s?)$/i},'j',2], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [j]
				'll':[{'+':/ll/i},'j',3], // par défaut avec ll donne le son [j]
				'ui':[{'-':/u/i,'+':/ent/i},'i',1], // essuient, appuient
				'ient_1':[this.Regle_ient,'i',1], // règle spécifique pour différencier les verbes du premier groupe 3ème pers pluriel
				'ient_2':[{'+':/ent(s)?$/i},'j',1], // si la règle précédente ne fonctionne pas
				'ie':[{'+':/e(s|nt)?$/i},'i',1], // mots terminés par -ie(s|nt)
				'i_voyelle':[{'+':/[aäâeéèêëoôöuù]/i},'j',1], // i suivi d'une voyelle donne [j]
				'*':[{},'i',1]}]";
            pos = 0;
            al = new AutomLetter(tstAL, ref pos);
            tt = TheText.NewTestTheText(@"briefing, berlingot, sapin, imbécile, limbe, afin, prier, ville, paille, triage,
                               appartient, amplifient, glorifient");
            pws = tt.GetPhonWords();

            foreach (PhonWord pw in pws)
                pw.ClearPhons();

            // briefing
            wordI = 0;
            pos = 5;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(6, pos);
            Assert.AreEqual("i", pws[wordI].Phonetique());

            // berlingot
            wordI = 1;
            pos = 4;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(6, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // sapin
            wordI = 2;
            pos = 3;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // imbécile
            wordI = 3;
            pos = 0;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(2, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // limbe
            wordI = 4;
            pos = 1;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(3, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // afin
            wordI = 5;
            pos = 2;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(4, pos);
            Assert.AreEqual("5", pws[wordI].Phonetique());

            // prier
            wordI = 6;
            pos = 2;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(3, pos);
            Assert.AreEqual("ij", pws[wordI].Phonetique());

            // ville
            wordI = 7;
            pos = 1;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(2, pos);
            Assert.AreEqual("i", pws[wordI].Phonetique());

            // paille
            wordI = 8;
            pos = 2;
            al.FireRule(pws[wordI], ref pos);
            rTxt = pws[wordI].AllStringInfo();
            StringAssert.Matches(rTxt, new Regex(@"Rule: @ill"));
            Assert.AreEqual(5, pos);
            Assert.AreEqual("j", pws[wordI].Phonetique());

            // triage
            wordI = 9;
            pos = 2;
            al.FireRule(pws[wordI], ref pos);
            rTxt = pws[wordI].AllStringInfo();
            StringAssert.Matches(rTxt, new Regex(@"Rule: prec_2cons"));
            Assert.AreEqual(3, pos);
            Assert.AreEqual("ij", pws[wordI].Phonetique());

            // appartient
            wordI = 10;
            pos = 6;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(7, pos);
            Assert.AreEqual("j", pws[wordI].Phonetique());

            // amplifient
            wordI = 11;
            pos = 6;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(7, pos);
            Assert.AreEqual("i", pws[wordI].Phonetique());

            // glorifient
            wordI = 12;
            pos = 4;
            al.FireRule(pws[wordI], ref pos);
            Assert.AreEqual(5, pos);
            Assert.AreEqual("i", pws[wordI].Phonetique());

        }
    }
}
