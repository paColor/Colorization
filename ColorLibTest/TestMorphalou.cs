using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using ColorLib.Morphalou;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace ColorLibTest
{
    [TestClass]
    public class TestMorphalou
    {

        const string path = @"F:\Utilisateurs\Papa\Divers\Colorization\Morphalou\csv\Tests\";
        const string fullName = path + @"morphalou_VRAIS.csv";

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        [TestMethod]
        public void TestMethod1()
        {
            if (File.Exists(fullName))
            {
                using (TextFieldParser csvParser = new TextFieldParser(fullName))
                {
                    csvParser.SetDelimiters(new string[] { ";" });
                    csvParser.HasFieldsEnclosedInQuotes = false;

                    // Skip the row with the column names
                    string headerLine = csvParser.ReadLine();
                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        Mot m = new Mot(csvParser.ReadFields());
                    }
                    Config conf = new Config();
                    Mot.EnsureCompleteness(conf, true);

                    foreach (Mot m in Mot.mots)
                    {
                        bool success = m.matchSet && m.match;
                        if (!success)
                        {
                            Console.WriteLine(m.ToString());
                        }
                        Assert.IsTrue(success);
                    }
                }
            }
        }
    }
}
