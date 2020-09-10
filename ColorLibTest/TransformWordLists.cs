using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    [TestClass]
    public class TransformWordLists
    {
        private static int lPos = 12;

        private static void WriteBlock (string s)
        {
            Console.Write('"');
            Console.Write(s);
            Console.Write("\", ");
            lPos += s.Length + 4;
            if (lPos > 93)
            {
                Console.WriteLine();
                Console.Write("            ");
                lPos = 12;
            }
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
            lPos = 12;
        }

        //[TestMethod]
        public void CheckIsSubset()
        {
            //foreach (string m in AutomRuleFilter.mots_t_final)
            //{
            //    if (!AutomRuleFilter.mots_t_finalMorph.Contains(m))
            //    {
            //        WriteBlock(m);
            //    }
            //}

            //Assert.IsTrue(AutomRuleFilter.mots_t_final.IsSubsetOf(AutomRuleFilter.mots_t_finalMorph));
        }


        //[TestMethod]
        public void MotsSansAccents()
        {
            //foreach (string v in AutomRuleFilter.mots_t_finalMorph)
            //{
            //    string sansAcc = AutomRuleFilter.ChaineSansAccents(v);
            //    if (sansAcc != v && !AutomRuleFilter.mots_t_finalMorph.Contains(sansAcc))
            //    {
            //        WriteBlock(sansAcc);
            //    }
            //}
        }

        //[TestMethod]
        public void FinalPasS()
        {
            //foreach (string v in AutomRuleFilter.mots_s_final)
            //{
            //    if (v[v.Length - 1] != 's')
            //    {
            //        WriteBlock(v);
            //    }
            //}
        }


        [TestMethod]
        public void PrintConstruct()
        {
            string txt = "trustent";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.AllStringInfo());
            }
        }

        [TestMethod]
        public void PrintExceptFormat()
        {
            string txt = "actaea";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.PourExceptDictionary());
            }
        }

        [TestMethod]
        public void WriteWords()
        {
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(words);
            foreach (Match m in matches)
            {
                WriteBlock(m.Value);
            }
        }
            
            
        string words =
        @"
           acupuncteur
acupuncteurs
acupuncture
acupunctures
avunculaire
avunculairement
avunculaires
avunculat
avunculats
bécabunga
bécabungas
carborundum
carborundums
compound
conjungo
conjungos
contrapuntique
contrapuntiques
contrapuntiste
contrapuntistes
fungicide
homuncule
homuncules
infundibulum
infundibulums
latifundium
latifundiums
negundo
négundo
negundos
négundos
nuncupatif
nuncupatifs
nuncupation
nuncupations
nuncupative
nuncupatives
nundinal
nundinale
nundinales
nundinaux
nundines
opuntia
opuntias
pacfung
pacfungs
punctiforme
punctiformes
punctum
punctums
puntarelle
puntarelles
secundo
skungs
skunks
unciforme
unciformes
uncinaire
unciné
uncinée
uncinées
uncinés
uncinule
uncipenne
uncirostre
undécennal
undécennale
undécennales
undécennaux
unguéal
unguéale
unguéales
unguéaux
unguifère
unguifères
unguis
vasopuncture
vérécundie



        ";
    }
}
