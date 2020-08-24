using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.IO;

namespace ColorLibTest.ListesMots
{
    [TestClass]
    public class TrouveExceptions
    {

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        const string path = @"F:\Utilisateurs\Papa\Divers\Colorization\Morphalou\csv\";
        const string fullName = path + @"morphalou.csv";
        const string fullOutFName = path + @"morphalouFiltre.csv";
        const string fullFilteredOutFN = path + @"morphaplouExclus.csv";


        [TestMethod]
        public void TestMethod1()
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
                                || graphie.Contains(@"'"))
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
    }
}
