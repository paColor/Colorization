using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Threading.Tasks;

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
            string txt = "cœur";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.ceras;
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.AllStringInfo());
            }
        }

        [TestMethod]
        public void PrintConstructLireCoul()
        {
            string txt = "camomille";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
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


pétioles
pétiolé
pétiolés
pétiolée
pétiolées

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

        //[TestMethod]
        //public void CheckReWords()
        //{
        //    HashSet<string> wordSet = new HashSet<string>();
        //    Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
        //    MatchCollection matches = rx.Matches(words);
        //    const int nrLetters = 6;
        //    foreach (Match m in matches)
        //    {
        //        string w = m.Value;
        //        string ws;
        //        if (w.Length <= nrLetters)
        //        {
        //            ws = w;
        //        }
        //        else
        //        {
        //            ws = w.Substring(0, nrLetters);
        //        }
        //        if (AutomRuleFilter.motsRe6.Contains(ws))
        //        {
        //            WriteBlock(w);
        //        }
        //    }
        //}


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

        public class LineOfCSV
        {
            public string l; // the complete line
            public string w; // the word
            public string p; // phonetic representation
        }


        [TestMethod]
        public void AddPhonToCSVJoachim()
        {
            const string path = @"F:\Utilisateurs\Papa\Prog\ClavierAPI\dictionaries\dictionaries\";
            // const string path = @"F:\Utilisateurs\Papa\Prog\ClavierAPI\tmp\";
            const string fullInName = path + @"wiktionnaire_clean.csv";
            const string fullOutName = path + @"wiktionnaire_clean_phon.csv";

            List<LineOfCSV> linesOfCSV = new List<LineOfCSV>();

            if (File.Exists(fullOutName))
            {
                File.Delete(fullOutName);
            }

            if (File.Exists(fullInName))
            {
                using (TextFieldParser csvParser = new TextFieldParser(fullInName))
                {
                    using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(fullOutName))
                    {
                        string headerLine = csvParser.ReadLine();
                        headerLine = headerLine.Trim();
                        headerLine = headerLine + ";phon";
                        outFile.WriteLine(headerLine);

                        // csvParser.SetDelimiters(new string[] { ";" });
                        // csvParser.HasFieldsEnclosedInQuotes = false;

                        while (!csvParser.EndOfData)
                        {
                            string line = csvParser.ReadLine();
                            string[] elements = line.Split(new char[] { ';' }, 3);
                            if (elements.Length == 3)
                            {
                                LineOfCSV lineOfCSV = new LineOfCSV();
                                lineOfCSV.l = line;
                                lineOfCSV.w = elements[1];
                                linesOfCSV.Add(lineOfCSV);
                            }
                        }
                        
                        Config conf = new Config();

                        Parallel.ForEach(linesOfCSV, (l) =>
                        {
                            TheText tt = new TheText(l.w);
                            List<PhonWord> pws = tt.GetPhonWordList(conf);
                            l.p = "";
                            foreach (PhonWord pw in pws)
                            {
                                if (l.p.Length > 0)
                                {
                                    l.p = l.p + " ";
                                }
                                l.p = l.p + pw.ToColSE();
                            }
                          
                        });

                        foreach (LineOfCSV l in linesOfCSV)
                        {
                            outFile.Write(l.l);
                            outFile.Write(";");
                            outFile.WriteLine(l.p);
                        }
                    }
                    
                }
            }
        }


    }
}
