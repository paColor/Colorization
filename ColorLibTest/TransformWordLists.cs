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
            string txt = "noyau";
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

highlandais
highlander
highlanders
highlifer
highlifers
highlifeur
highlifeurs
hindi
hindis
hinterland
hinterlands
homespun
homespuns
homoionique
hotus


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
        public void CheckReWords()
        {
            HashSet<string> wordSet = new HashSet<string>();
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(words);
            const int nrLetters = 6;
            foreach (Match m in matches)
            {
                string w = m.Value;
                string ws;
                if (w.Length <= nrLetters)
                {
                    ws = w;
                }
                else
                {
                    ws = w.Substring(0, nrLetters);
                }
                if (AutomRuleFilter.motsRe6.Contains(ws))
                {
                    WriteBlock(w);
                }
            }
        }


        [TestMethod]
        public void WriteWords()
        {
            HashSet<string> wordSet = new HashSet<string>();
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(words);
            foreach (Match m in matches)
            {
                wordSet.Add(m.Value); // un doublon n'est ajouté qu'une fois
            }
            List<string> wordList = new List<string>(wordSet);
            wordList.Sort();
            foreach (string s in wordList)
            {
                WriteBlock(s);
            }
        }


        string words =
        @"
fritillaire
gille
gillotage
gillotages
grills
imbécillifié
imbécilliser
intermaxillaire
intermaxillaires
lapilli
lapillis
mamillaire
mamillaires
maxille
maxilles
maxillifère
maxilliforme
mille
millefeuille
millefeuilles
millepertuis
milleraies
millerandage
millerandages
millerole
milleroles
millerolle
millerolles
milles
multimillénaire
oscilla
oscillai
oscillaient
oscillais
oscillait
oscillâmes
oscillant
oscillante
oscillantes
oscillants
oscillas
oscillasse
oscillassent
oscillasses
oscillassiez
oscillassions
oscillât
oscillâtes
oscillateur
oscillateurs
oscillation
oscillations
oscillatoire
oscillatoires
oscillatrice
oscillatrices
oscille
oscillé
oscillée
oscillées
oscillement
oscillent
osciller
oscillera
oscillerai
oscilleraient
oscillerais
oscillerait
oscilleras
oscillèrent
oscillerez
oscilleriez
oscillerions
oscillerons
oscilleront
oscilles
oscillés
oscillez
oscilliez
oscillions
oscillogramme
oscillogrammes
oscillographe
oscillographes
oscillographie
oscillographique
oscillomètre
oscillomètres
oscillons
oscilloscope
oscilloscopes
papillite
papillites
papillomateuse
papillomateuses
papillomateux
papillome
papillomes
pénicille
pénicillé
pénicillée
pénicillées
pénicilles
pénicillés
pénicillinase
pénicillinases
pénicillium
pénicilliums
phosphovanillique
précapillaire
prémaxillaire
quadrillion
quadrillions
quintillion
réilluminer
saxillaire
saxillaires
scillarène
scille
scilles
scillitique
scillitiques
scintillatrice
scintillatrices
septillion
sigillographe
sigillographes
sigillographie
sigillographies
sigillographique
sigillographiques
spirillose
spirilloses
spongille
spongilles
sugillation
sugillations
tefillin
téfillin
téfillins
thriller
thrillers
tillodontes
trillionnaire
twills
vanilline
vanillines
vanillisme
vanillismes



        ";
    }
}
