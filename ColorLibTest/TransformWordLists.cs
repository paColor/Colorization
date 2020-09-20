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
            string txt = "audience";
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
            string txt = @"

auburn







";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.PourExceptDictionary());
                Console.Write("            ");
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
agoyate
agoyates
alcoyle
alcoyles
arroyo
arroyos
benzoyle
benzoyles
boy
boys
boyard
boyards
boycott
boycotts
boycottage
boycottages
boycotta
boycottai
boycottaient
boycottais
boycottait
boycottant
boycottas
boycottasse
boycottassent
boycottasses
boycottassiez
boycottassions
boycotte
boycotte
boycotte
boycotte
boycottent
boycottent
boycotter
boycottera
boycotterai
boycotteraient
boycotterais
boycotterait
boycotteras
boycotterez
boycotteriez
boycotterions
boycotterons
boycotteront
boycottes
boycottes
boycottez
boycottez
boycottiez
boycottiez
boycottions
boycottions
boycottons
boycottons
boycottâmes
boycottât
boycottâtes
boycottèrent
boycottés
boycotté
boycottée
boycottées
boycotte
boycottais
boycotterais
broyon
cacaoyer
cacaoyers
cacaoyère
cacaoyères
caloyère
caloyères
caloyer
caloyers
coyau
coyaux
coyote
coyotes
goy
goys
goyau
goyaux
goyave
goyaves
goyavier
goyaviers
goyot
goyots
halloysite
halloysites
oyant
oyant
oyants
oyat
oyats
samoyède
samoyèdes
samoyède
samoyèdes
yoyo
yoyos
alcoylé
hoya
métahalloysite
sulfamoyle





        ";
    }
}
