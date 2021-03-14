using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ColorLib;

namespace ColorLibTest
{
    /// <summary>
    /// Tests pour la liste de mots à ne pas coloriser.
    /// </summary>
    [TestClass]
    public class ExcMotsTest
    {
        Config conf;
        const string txt1 = @"Il neigeait, il neigeait toujours ! la froide bise Sifflait ; sur le
            verglas, dans des lieux inconnus, On n'avait pas de pain et l'on allait pieds nus. Ce
            n'étaient plus des cœurs vivants, des gens de guerre ; C'était un rêve errant dans la 
            brume, un mystère, Une procession d'ombres sur le ciel noir. La solitude, vaste, 
            épouvantable à voir, Partout apparaissait, muette vengeresse. Le ciel faisait sans bruit
            avec la neige épaisse Pour cette immense armée un immense linceul ; Et, chacun se
            sentant mourir, on était seul. - Sortira-t-on jamais de ce funèbre empire ? Deux ennemis ! 
            le Tzar, le Nord. Le Nord est pire. On jetait les canons pour brûler les affûts. Qui se
            couchait, mourait. Groupe morne et confus, Ils fuyaient ; le désert dévorait le cortège.
            On pouvait, à des plis qui soulevaient la neige,";

        const string excTxt = @"On n'avait pas de pain et Sortira-t-on il d'ombres";

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            conf = TestTheText.GetSpecialConfig();
            ExceptionMots excm = new ExceptionMots(excTxt);
            excm.syllabes = true;
            excm.mots = true;
            excm.arcs = true;
            excm.phonemes = true;
            conf.sylConf.ExcMots = excm;
        }

        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestPhonemes()
        {
            TestTheText ttt = new TestTheText(txt1);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            ttt.AssertColor(0, TestTheText.black);
            ttt.AssertColor(1, TestTheText.black);
            ttt.AssertNotColor(3, TestTheText.black);
            int index = ttt.S.IndexOf("Sifflait");
            ttt.AssertNotColor(index, 8, TestTheText.black);
            index = ttt.S.IndexOf("On n'avait pas de pain");
            ttt.AssertColor(index, 22, TestTheText.black);
            index = ttt.S.IndexOf("on était seul");
            ttt.AssertColor(index, 2, TestTheText.black);
            ttt.AssertNotColor(index + 3, 5, TestTheText.black);
            index = ttt.S.IndexOf("d'ombres");
            ttt.AssertColor(index, 8, TestTheText.black);
        }

        [TestMethod]
        public void TestArcs()
        {
            TestTheText ttt = new TestTheText(txt1);
            ttt.MarkArcs(conf);
            ttt.AssertDrawArc(0, 2, false);
            ttt.AssertDrawArc(3, 8, true);
            int index = ttt.S.IndexOf("Sifflait");
            ttt.AssertDrawArc(index, 8, true);
            index = ttt.S.IndexOf("On n'avait pas de pain");
            ttt.AssertDrawArc(index, 2, false);
            ttt.AssertDrawArc(index + 3, 7, false);
            index = ttt.S.IndexOf("on était seul");
            ttt.AssertDrawArc(index, 2, false);
            ttt.AssertDrawArc(index + 3, 5, true);
            index = ttt.S.IndexOf("d'ombres");
            ttt.AssertDrawArc(index, 8, false);
        }

        [TestMethod]
        public void TestSyls()
        {
            TestTheText ttt = new TestTheText(txt1);
            ttt.MarkSyls(conf);
            ttt.AssertColor(0, TestTheText.black);
            ttt.AssertColor(1, TestTheText.black);
            ttt.AssertNotColor(3, TestTheText.black);
            int index = ttt.S.IndexOf("Sifflait");
            ttt.AssertNotColor(index, 8, TestTheText.black);
            index = ttt.S.IndexOf("On n'avait pas de pain");
            ttt.AssertColor(index, 22, TestTheText.black);
            index = ttt.S.IndexOf("on était seul");
            ttt.AssertColor(index, 2, TestTheText.black);
            ttt.AssertNotColor(index + 3, 5, TestTheText.black);
            index = ttt.S.IndexOf("d'ombres");
            ttt.AssertColor(index, 8, TestTheText.black);
        }

        [TestMethod]
        public void TestMots()
        {
            TestTheText ttt = new TestTheText(txt1);
            ttt.MarkWords(conf);
            ttt.AssertColor(0, TestTheText.black);
            ttt.AssertColor(1, TestTheText.black);
            ttt.AssertNotColor(3, TestTheText.black);
            int index = ttt.S.IndexOf("Sifflait");
            ttt.AssertNotColor(index, 8, TestTheText.black);
            index = ttt.S.IndexOf("On n'avait pas de pain");
            ttt.AssertColor(index, 22, TestTheText.black);
            index = ttt.S.IndexOf("on était seul");
            ttt.AssertColor(index, 2, TestTheText.black);
            ttt.AssertNotColor(index + 3, 5, TestTheText.black);
            index = ttt.S.IndexOf("d'ombres");
            ttt.AssertColor(index, 8, TestTheText.black);
        }
    }
}
