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
            string txt = "rescindais";
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

reflex







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



        ";
    }
}
