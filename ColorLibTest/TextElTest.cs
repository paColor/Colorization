using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Linq;
using System.Collections.Generic;

namespace ColorLibTest
{
    [TestClass]
    public class TextElTest : TextEl
    {

        public TextElTest()
            : base(null, 0, 0)
        {}

        public TextElTest(TheText inT, int inFirst, int inLast)
            : base(inT, inFirst, inLast)
        {}

        [TestMethod]
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
        }
    }
}
