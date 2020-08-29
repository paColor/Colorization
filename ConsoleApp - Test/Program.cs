using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorLib;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ConsoleApp___Test
{

    class Program
    {
        const string path = @"F:\Utilisateurs\Papa\Divers\Colorization\Morphalou\csv\";
        const string fullName = path + @"morphalou.csv";
        const string fullOutFName = path + @"morphalouFiltre.csv";
        const string fullFilteredOutFN = path + @"morphaplouExclus.csv";

        private static void ExclutMotsComposes()
        {
            using (TextFieldParser csvParser = new TextFieldParser(fullName))
            {
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                string headerLine = csvParser.ReadLine();

                if (File.Exists(fullOutFName))
                {
                    File.Delete(fullOutFName);
                }
                using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(fullOutFName))
                {
                    if (File.Exists(fullFilteredOutFN))
                    {
                        File.Delete(fullFilteredOutFN);
                    }
                    using (System.IO.StreamWriter excludedFile
                        = new System.IO.StreamWriter(fullFilteredOutFN))
                    {
                        outFile.WriteLine(headerLine);
                        excludedFile.WriteLine(headerLine);

                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            string[] fields = csvParser.ReadFields();
                            string graphie = fields[0];
                            string id = fields[1];
                            string phonetique = fields[2];

                            System.IO.StreamWriter sw;

                            if (graphie.Contains(" ")
                                || graphie.Contains("-")
                                || graphie.Contains(@"'")
                                || graphie.Contains(@".")
                                )
                            {
                                sw = excludedFile;
                            }
                            else
                            {
                                sw = outFile;
                            }
                            sw.WriteLine("{0};{1};{2}", graphie, id, phonetique);
                        }
                    }
                }
            }
        }

        private static void ExclutMatchesFromMorphalou()
        {
            using (TextFieldParser csvParser = new TextFieldParser(fullName))
            {
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                string headerLine = csvParser.ReadLine();

                if (File.Exists(fullOutFName))
                {
                    File.Delete(fullOutFName);
                }
                using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(fullOutFName))
                {
                    if (File.Exists(fullFilteredOutFN))
                    {
                        File.Delete(fullFilteredOutFN);
                    }
                    using (System.IO.StreamWriter excludedFile
                        = new System.IO.StreamWriter(fullFilteredOutFN))
                    {
                        headerLine = "GRAPHIE;ID;MORPHALOU1;MORPHALOU2;COLORIZATION";
                        outFile.WriteLine(headerLine);
                        excludedFile.WriteLine(headerLine);

                        TheText.Init();
                        Config conf = new Config();
                        int count = 0;

                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            string[] fields = csvParser.ReadFields();
                            string graphie = fields[0];
                            string id = fields[1];
                            string phonetique = fields[2];
                            string phon1, phon2;

                            System.IO.StreamWriter sw;

                            int posOU = phonetique.IndexOf("OU");
                            if (posOU > 0)
                            {
                                phon1 = phonetique.Substring(0, posOU);
                                phon2 = phonetique.Substring(posOU + 3, phonetique.Length - (posOU + 3));
                            }
                            else
                            {
                                phon1 = phonetique;
                                phon2 = "";
                            }

                            TheText tt = new TheText(graphie);
                            List<PhonWord> pws = tt.GetPhonWordList(conf);
                            string colPhon = NotationsPhon.C2CS(pws[0].Phonetique());

                            if (colPhon == NotationsPhon.S2CS(phon1)
                                ||
                               colPhon == NotationsPhon.S2CS(phon2))
                            {
                                sw = excludedFile;
                            }
                            else
                            {
                                sw = outFile;
                            }
                            sw.WriteLine("{0};{1};{2};{3};{4}", graphie, id, phon1, phon2, colPhon);
                            count++;
                            if (count % 1000 == 0)
                            {
                                Console.WriteLine(count);
                            }
                        }
                    }
                }
            }
        }

        private static void ExclutMatches2()
        {
            using (TextFieldParser csvParser = new TextFieldParser(fullName))
            {
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                string headerLine = csvParser.ReadLine();

                if (File.Exists(fullOutFName))
                {
                    File.Delete(fullOutFName);
                }
                using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(fullOutFName))
                {
                    if (File.Exists(fullFilteredOutFN))
                    {
                        File.Delete(fullFilteredOutFN);
                    }
                    using (System.IO.StreamWriter excludedFile
                        = new System.IO.StreamWriter(fullFilteredOutFN))
                    {
                        headerLine = "GRAPHIE;ID;MORPHALOU1;MORPHALOU2;COLORIZATION";
                        outFile.WriteLine(headerLine);
                        excludedFile.WriteLine(headerLine);

                        TheText.Init();
                        Config conf = new Config();
                        int count = 0;

                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            string[] fields = csvParser.ReadFields();
                            string graphie = fields[0];
                            string id = fields[1];
                            string phon1 = fields[2];
                            string phon2 = fields[3];
                            string colPhon = fields[4];

                            System.IO.StreamWriter sw;

                            string phon1CS = NotationsPhon.S2CS(phon1);
                            if (phon1CS.Length > 0 && phon1CS[phon1CS.Length - 1] == '°')
                            {
                                phon1CS = phon1CS.Substring(0, phon1CS.Length - 1);
                            }
                            string phon2CS = NotationsPhon.S2CS(phon2);
                            if (phon2CS.Length > 0 && phon2CS[phon2CS.Length - 1] == '°')
                            {
                                phon2CS = phon2CS.Substring(0, phon2CS.Length - 1);
                            }

                            if (colPhon == phon1CS
                                ||
                               colPhon == phon2CS)
                            {
                                sw = excludedFile;
                            }
                            else
                            {
                                sw = outFile;
                            }
                            sw.WriteLine("{0};{1};{2};{3};{4}", graphie, id, phon1, phon2, colPhon);
                            count++;
                            if (count % 1000 == 0)
                            {
                                Console.WriteLine(count);
                            }
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            using (TextFieldParser csvParser = new TextFieldParser(fullName))
            {
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                string headerLine = csvParser.ReadLine();

                if (File.Exists(fullOutFName))
                {
                    File.Delete(fullOutFName);
                }
                using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(fullOutFName))
                {
                    if (File.Exists(fullFilteredOutFN))
                    {
                        File.Delete(fullFilteredOutFN);
                    }
                    using (System.IO.StreamWriter excludedFile
                        = new System.IO.StreamWriter(fullFilteredOutFN))
                    {
                        bool recompute = args.Length > 0 && args[0] == "recompute";
                        Mot.Init();
                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            Mot m = new Mot(csvParser.ReadFields());
                        }
                        Config conf = new Config();
                        Mot.EnsureCompleteness(conf, recompute);
                        Mot.DumpMotsFiltered(excludedFile, outFile);
                    }
                }
            }
        }
    }
}
