using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Linq;
using System.Collections.Generic;

namespace ColorLibTest
{

    public class TextElTest : TextEl
    {

        public TextElTest()
            : base(null, 0, 0)
        {}

        public TextElTest(TheText inT, int inFirst, int inLast)
            : base(inT, inFirst, inLast)
        {}

        public void TestEstConsonne()
        {
            Assert.IsTrue(EstConsonne('c'));
            Assert.IsTrue(EstConsonne('k'));
            Assert.IsTrue(EstConsonne('f'));
            Assert.IsTrue(EstConsonne('ç'));
            Assert.IsFalse(EstConsonne('a'));
            Assert.IsFalse(EstConsonne('e'));
            Assert.IsFalse(EstConsonne('i'));
            Assert.IsFalse(EstConsonne('o'));
            Assert.IsFalse(EstConsonne('u'));
            Assert.IsFalse(EstConsonne('y'));
        }

        
    }

    [TestClass]
    public class TextElTestExecution
    {

        [TestMethod]
        public void TestSetCharFormat()
        {
            string s = @"Heureux qui, comme Ulysse, a fait un beau voyage,
                        Ou comme cestuy la qui conquit la toison,
                        Et puis est retourné, plein d'usage et raison,
                        Vivre entre ses parents le reste de son age!";
            TheText tt = new TheText(s);
            TextElTest tet = new TextElTest(tt, 19, 24); // Ulysse
            Assert.AreEqual("Ulysse", tet.ToString());

            tet.TestEstConsonne(); // comme ça c'est fait aussi...
        }

        [TestMethod]
        public void TestInvalidTextEl()
        {
            TestTheText ttt = new TestTheText("Bonjour");
            Assert.ThrowsException<ArgumentNullException>(() => _ = new TextElTest());
            Assert.ThrowsException<ArgumentNullException>(() => _ = new TextEl(null, 17, 18));
            Assert.ThrowsException<ArgumentException>(() => _ = new TextEl(ttt, 17, 18));
            Assert.ThrowsException<ArgumentException>(() => _ = new TextEl(ttt, -10, 18));
            Assert.ThrowsException<ArgumentException>(() => _ = new TextEl(ttt, -10, -2));

            ttt = new TestTheText("");
            Assert.ThrowsException<ArgumentException>(() => _ = new TextEl(ttt, 0, 0));
        }
    }
}
