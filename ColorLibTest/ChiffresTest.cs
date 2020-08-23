using System;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    [TestClass]
    public class ChiffresTest
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


        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {

        }

        const string txt1 = @"Tu approuves 1867 donc mon projet ? dit M. de Rênal2, reme35rciant sa
                              23femme, p45ar un sourire, de l’excellente31 idée qu’elle venait 
                              d’avoir.
                              Allons, voilà qui est déc0123456789idé.";


        [TestMethod]
        public void TestMethod1()
        {
            TestTheText ttt = new TestTheText(txt1);
            Config conf = new Config();

            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            int index = ttt.S.IndexOf("1867");
            ttt.AssertColor(index, 4, TestTheText.black);

            index = ttt.S.IndexOf("Rênal");
            ttt.AssertColor(index + 5, TestTheText.black);

            index = ttt.S.IndexOf("reme35rciant");
            ttt.AssertColor(index + 4, 2, TestTheText.black);

            index = ttt.S.IndexOf("23femme");
            ttt.AssertColor(index, 2, TestTheText.black);

            index = ttt.S.IndexOf("p45ar");
            ttt.AssertColor(index + 1, 2, TestTheText.black);

            index = ttt.S.IndexOf("déc0123456789idé");
            ttt.AssertColor(index + 3, 10, TestTheText.black);
        }

        
    }
}
