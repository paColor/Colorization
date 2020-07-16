using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using ColorLib.Dierese;
using System.Diagnostics;
using System.Collections.Generic;

namespace ColorLibTest.Dierese
{
    [TestClass]
    public class VersTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        string expiation27 =
            @"Puis, à pas lents, musique en tête, sans fureur,
            Tranquille, souriant à la mitraille anglaise,
            La garde impériale entra dans la fournaise.
            Hélas ! Napoléon, sur sa garde penché,
            Regardait, et, sitôt qu'ils avaient débouché
            Sous les sombres canons crachant des jets de soufre,
            Voyait, l'un après l'autre, en cet horrible gouffre,
            Fondre ces régiments de granit et d'acier
            Comme fond une cire au souffle d'un brasier.";

        [TestMethod]
        public void TestExceptionsConstructeur()
        {
            TestTheText ttt = new TestTheText(expiation27);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            List<PhonWord> pws = ttt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }

            Vers v;
            List<PhonWord> emptyPws = new List<PhonWord>();
            Assert.ThrowsException<ArgumentNullException>(() => { v = new Vers(null, 0, pws); });
            Assert.ThrowsException<ArgumentNullException>(() => { v = new Vers(ttt, 0, null); });
            Assert.ThrowsException<ArgumentException>(() => { v = new Vers(ttt, 1000, pws); });
            Assert.ThrowsException<ArgumentException>(() => { v = new Vers(ttt, 0, emptyPws); });
        }

        [TestMethod]
        public void TestConstructeur()
        {
            TestTheText ttt = new TestTheText(expiation27);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            List<PhonWord> pws = ttt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }
            Vers v = new Vers(ttt, 0, pws);
            Assert.AreEqual("Puis, à pas lents, musique en tête, sans fureur,", v.ToString());
            Assert.AreEqual("Puis à pas lents mu-sique en tê-te sans fu-reur", v.Syllabes());

            Vers v2 = new Vers(ttt, 50, pws);
            Assert.AreEqual("            Tranquille, souriant à la mitraille anglaise,", v2.ToString());
            Assert.AreEqual("Tran-quil-le sou-riant à la mi-traille an-glaise", v2.Syllabes());
        }


        [TestMethod]
        public void TestDierese()
        {
            TestTheText ttt = new TestTheText(expiation27);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            List<PhonWord> pws = ttt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }
            Vers v = new Vers(ttt, 0, pws);
            Assert.AreEqual("Puis, à pas lents, musique en tête, sans fureur,", v.ToString());
            Assert.AreEqual("Puis à pas lents mu-sique en tê-te sans fu-reur", v.Syllabes());
            v.ChercheDierese(12);
            Assert.AreEqual("Puis à pas lents mu-sique en tê-te sans fu-reur", v.Syllabes());

            Vers v2 = new Vers(ttt, 50, pws);
            Assert.AreEqual("            Tranquille, souriant à la mitraille anglaise,", v2.ToString());
            Assert.AreEqual("Tran-quil-le sou-riant à la mi-traille an-glaise", v2.Syllabes());
            v2.ChercheDierese(12);
            Assert.AreEqual("Tran-quil-le sou-ri-ant à la mi-traille an-glaise", v2.Syllabes());

            Vers v3 = new Vers(ttt, 121, pws);
            Assert.AreEqual("La garde impériale entra dans la fournaise.", v3.ToString());
            Assert.AreEqual("La garde im-pé-riale en-tra dans la four-naise", v3.Syllabes());
            v3.ChercheDierese(12);
            Assert.AreEqual("La garde im-pé-ri-ale en-tra dans la four-naise", v3.Syllabes());
        }
    }
}
