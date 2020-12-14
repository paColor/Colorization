using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ColorLib;

namespace ColorLibTest
{
    [TestClass]
    public class TheTextTest
    {
        [TestMethod]
        public void TestGetPhonWords()
        {
            List<PhonWord> pws;
            TheText tt;

            Config conf = new Config();
            tt = new TheText("Dans tes yeux les clartés trop brutales s’émoussent.");
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            pws = tt.GetPhonWordList(conf, false);
            Assert.AreEqual("Dans", pws[0].ToString());
            Assert.AreEqual("tes", pws[1].ToString());
            Assert.AreEqual("yeux", pws[2].ToString());
            Assert.AreEqual("les", pws[3].ToString());
            Assert.AreEqual("clartés", pws[4].ToString());
            Assert.AreEqual("trop", pws[5].ToString());
            Assert.AreEqual("brutales", pws[6].ToString());
            Assert.AreEqual("s’", pws[7].ToString());
            Assert.AreEqual("émoussent", pws[8].ToString());

            pws = tt.GetPhonWordList(conf, true);
            Assert.AreEqual(8, pws.Count);
            Assert.AreEqual("Dans", pws[0].ToString());
            Assert.AreEqual("tes", pws[1].ToString());
            Assert.AreEqual("yeux", pws[2].ToString());
            Assert.AreEqual("les", pws[3].ToString());
            Assert.AreEqual("clartés", pws[4].ToString());
            Assert.AreEqual("trop", pws[5].ToString());
            Assert.AreEqual("brutales", pws[6].ToString());
            Assert.AreEqual("s’émoussent", pws[7].ToString());

            tt = new TheText
                (
                @"
                France ! ô belle contrée, ô terre généreuse
                Que les dieux 'complaisants' forma'ient pour être heureuse,

                Je ne t’ai pas connu, je ne t’ai pas aimé,
                Je ne te connais point et je t’aime encor moins :
                Je me chargerais mal de ton nom diffamé,
                Et si j’ai quelque droit d'être entre tes témoins,

                C’est que, d’abord, et c’est qu’ailleurs, vers les Pieds joints
                D’abord par les clous froids, puis par l’élan pâmé
                Des femmes de péché – desquelles ô tant oints,
                Tant baisés, chrême fol et baiser affamé ! –
                "
                );
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            pws = tt.GetPhonWordList(conf, false);
            Assert.AreEqual("France", pws[0].ToString());
            Assert.AreEqual("ô", pws[1].ToString());
            Assert.AreEqual("belle", pws[2].ToString());
            Assert.AreEqual("les", pws[8].ToString());
            Assert.AreEqual("complaisants", pws[10].ToString());
            Assert.AreEqual("ient", pws[12].ToString());
            Assert.AreEqual("Je", pws[16].ToString());
            Assert.AreEqual("t’", pws[18].ToString());
            Assert.AreEqual("t’", pws[24].ToString());
            Assert.AreEqual("de", pws[43].ToString());
            Assert.AreEqual("j’", pws[49].ToString());
            Assert.AreEqual("d'", pws[53].ToString());
            Assert.AreEqual("C’", pws[58].ToString());
            Assert.AreEqual("qu’", pws[66].ToString());
            Assert.AreEqual("Pieds", pws[70].ToString());
            Assert.AreEqual("clous", pws[76].ToString());
            Assert.AreEqual("l’", pws[80].ToString());
            Assert.AreEqual("péché", pws[86].ToString());
            Assert.AreEqual("tant", pws[89].ToString());

            pws = tt.GetPhonWordList(conf, true);
            Assert.AreEqual("France", pws[0].ToString());
            Assert.AreEqual("ô", pws[1].ToString());
            Assert.AreEqual("belle", pws[2].ToString());
            Assert.AreEqual("les", pws[8].ToString());
            Assert.AreEqual("complaisants", pws[10].ToString());
            Assert.AreEqual("ient", pws[12].ToString());
            Assert.AreEqual("Je", pws[16].ToString());
            Assert.AreEqual("t’ai", pws[18].ToString());
            Assert.AreEqual("t’ai", pws[23].ToString());
            Assert.AreEqual("d'être", pws[49].ToString());
        }

        [TestMethod]
        public void TestGetPhonWords2()
        {
            List<PhonWord> pws;
            TheText tt;

            Config conf = new Config();
            tt = new TheText(@"Dans tes yeux les clartés trop brutales s’émoussent. J'");
            pws = tt.GetPhonWordList(conf, true);
            Assert.AreEqual("J'", pws[8].ToString());

        }

        [TestMethod]
        public void TestTexteVide()
        {
            // le but principal est qu'il n'y ait pas d'exceptions...
            TestTheText ttt = new TestTheText("");
            Config conf = new Config();
            ttt.ColorizePhons(conf, PhonConfType.phonemes);
            ttt.ColorizePhons(conf, PhonConfType.muettes);
            List<PhonWord> list = ttt.GetPhonWordList(conf);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 0);
            list = ttt.GetPhonWordList(conf, true);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 0);
            ttt.MarkDuo(conf);
            ttt.MarkLetters(conf);
            ttt.MarkLignes(conf);
            ttt.MarkMuettes(conf);
            ttt.MarkNoir(conf);
            conf.sylConf.mode = SylConfig.Mode.ecrit;
            ttt.MarkSyls(conf);
            conf.sylConf.mode = SylConfig.Mode.oral;
            ttt.MarkSyls(conf);
            conf.sylConf.mode = SylConfig.Mode.poesie;
            ttt.MarkSyls(conf);
            ttt.MarkVoyCons(conf);
            ttt.MarkWords(conf);
            _ = ttt.ToLowerString();
            _ = ttt.ToString();
        }

        private Config conf;

        /// <summary>
        /// Copie de l'original (<see cref="ColConfWin.CERASColor"/>)
        /// </summary>
        private enum tstCERASColor
        {
#pragma warning disable CA1707 // Identifiers should not contain underscores
            CERAS_oi, CERAS_o, CERAS_an, CERAS_5, CERAS_E, CERAS_e, CERAS_u,
            CERAS_on, CERAS_eu, CERAS_oin, CERAS_muet, CERAS_rosé, CERAS_ill, CERAS_1
#pragma warning restore CA1707 // Identifiers should not contain underscores
        }


        /// <summary>
        /// Copie du tableau <see cref="ColConfWin.predefinedColors"/> pour tester sur les valeurs
        /// absolues. Doit être adapté en cas de modification de l'original.
        /// </summary>
        private readonly static RGB[] tstPredefinedColors = new RGB[] {
            new RGB(000, 000, 000), // CERAS_oi     --> noir
            new RGB(240, 222, 000), // CERAS_o      --> jaune
            new RGB(237, 125, 049), // CERAS_an     --> orange
            new RGB(051, 153, 102), // CERAS_5      --> vert comme sapin
            new RGB(164, 020, 210), // CERAS_E      --> violet
            new RGB(000, 020, 208), // CERAS_e      --> (bleu) foncé
            new RGB(255, 000, 000), // CERAS_u      --> rouge
            new RGB(171, 121, 066), // CERAS_on     --> marron
            new RGB(071, 115, 255), // CERAS_eu     --> bleu
            new RGB(015, 201, 221), // CERAS_oin    --> turquoise
            new RGB(166, 166, 166), // CERAS_muet   --> gris
            new RGB(255, 100, 177), // CERAS_rosé   --> rose
            new RGB(127, 241, 000), // CERAS_ill    --> vert grenouille
            new RGB(222, 222, 222), // CERAS_1      --> gris // la couleur ne devrait jamais être utilisée.

            new RGB(221, 221, 221), // neutre       --> gris // il est important qu'il ne s'agisse pas d'une couleur de WdColorIndex
            new RGB(000, 000, 255), // bleuPur      --> bleu
            new RGB(091, 215, 255), // bleu clair   --> bleu clair
            new RGB(175, 000, 000), // rouge foncé  --> rouge foncé
            new RGB(255, 255, 255), // blanc        --> blanc
        };

        private static CharFormatting[] tstCerasCF { get; set; }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            // Il s'agit ici d'une copie de l'initialisation originale dans ColConfWin, pour éviter
            // d'utiliser cette dernière comme référence dans les tests.

            tstCerasCF = new CharFormatting[tstPredefinedColors.Length];
            for (int i = 0; i < tstPredefinedColors.Length; i++)
            {
                tstCerasCF[i] = new CharFormatting(tstPredefinedColors[i]);
            }
            tstCerasCF[(int)PredefCol.neutral] = CharFormatting.NeutralCF;

            tstCerasCF[(int)tstCERASColor.CERAS_ill] = new CharFormatting(false, true, false, false,
                true, tstPredefinedColors[(int)tstCERASColor.CERAS_ill],
                false, tstPredefinedColors[(int)PredefCol.neutral]);

            tstCerasCF[(int)tstCERASColor.CERAS_oi] = new CharFormatting(true, false, false, false,
                true, tstPredefinedColors[(int)tstCERASColor.CERAS_oi],
                false, tstPredefinedColors[(int)PredefCol.neutral]);

            tstCerasCF[(int)tstCERASColor.CERAS_1] = new CharFormatting(false, false, true);
        }


        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            TheText.Init();
            conf = new Config(); // default config at the beginning of the test.
        }

        [TestMethod]
        public void TestLettresIsolées()
        {
            string txt =
                "a b c d e f g h i j k l m n o p q r s t u v w x y z " +
                "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z " +
                "é à è ö ü ä ù ï ë É À È Ö Ü Ä Ù Ï Ë " +
                "0 1 2 3 4 5 6 7 8 9";

            TestTheText ttt = new TestTheText(txt);
            ttt.ColorizePhons(conf, PhonConfType.phonemes);

            //                           CharFormatting(bold,  itali, under, caps,  chCol,theColor       )
            CharFormatting blackCF = new CharFormatting(false, false, false, false, false, TestTheText.black,
                false, TestTheText.black);

            ttt.AssertCF(0, blackCF); // a
            ttt.AssertCF(2, blackCF); // b
            ttt.AssertCF(4, blackCF); // c
            ttt.AssertCF(6, blackCF); // d
            ttt.AssertCF(8, blackCF); // e
            ttt.AssertCF(10, blackCF); // f
            ttt.AssertCF(12, blackCF); // g
            ttt.AssertCF(14, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_muet]); // h
            ttt.AssertCF(16, blackCF); // i
            ttt.AssertCF(18, blackCF); // j
            ttt.AssertCF(20, blackCF); // k
            ttt.AssertCF(22, blackCF); // l
            ttt.AssertCF(24, blackCF); // m
            ttt.AssertCF(26, blackCF); // n
            ttt.AssertCF(28, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_o]); // o
            ttt.AssertCF(30, blackCF); // p
            ttt.AssertCF(32, blackCF); // q
            ttt.AssertCF(34, blackCF); // r
            ttt.AssertCF(36, blackCF); // s
            ttt.AssertCF(38, blackCF); // t
            ttt.AssertCF(40, blackCF); // u
            ttt.AssertCF(42, blackCF); // v
            ttt.AssertCF(44, blackCF); // w
            ttt.AssertCF(46, blackCF); // x
            ttt.AssertCF(48, blackCF); // y
            ttt.AssertCF(50, blackCF); // z

            ttt.AssertCF(52, blackCF); // A
            ttt.AssertCF(54, blackCF); // B
            ttt.AssertCF(56, blackCF); // C
            ttt.AssertCF(58, blackCF); // D
            ttt.AssertCF(60, blackCF); // E
            ttt.AssertCF(62, blackCF); // F
            ttt.AssertCF(64, blackCF); // G
            ttt.AssertCF(66, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_muet]); // H
            ttt.AssertCF(68, blackCF); // I
            ttt.AssertCF(70, blackCF); // J
            ttt.AssertCF(72, blackCF); // K
            ttt.AssertCF(74, blackCF); // L
            ttt.AssertCF(76, blackCF); // M
            ttt.AssertCF(78, blackCF); // N
            ttt.AssertCF(80, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_o]); // O
            ttt.AssertCF(82, blackCF); // P
            ttt.AssertCF(84, blackCF); // Q
            ttt.AssertCF(86, blackCF); // R
            ttt.AssertCF(88, blackCF); // S
            ttt.AssertCF(90, blackCF); // T
            ttt.AssertCF(92, blackCF); // U
            ttt.AssertCF(94, blackCF); // V
            ttt.AssertCF(96, blackCF); // W
            ttt.AssertCF(98, blackCF); // X
            ttt.AssertCF(100, blackCF); // Y
            ttt.AssertCF(102, blackCF); // Z

            ttt.AssertCF(104, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_rosé]); // é
            ttt.AssertCF(106, blackCF); // à
            ttt.AssertCF(108, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_E]); // è
            ttt.AssertCF(110, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_o]); // ö
            ttt.AssertCF(112, blackCF); // ü
            ttt.AssertCF(114, blackCF); // ä
            ttt.AssertCF(116, blackCF); // ù
            ttt.AssertCF(118, blackCF); // ï
            ttt.AssertCF(120, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_E]); // ë

            ttt.AssertCF(122, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_rosé]); // É
            ttt.AssertCF(124, blackCF); // À
            ttt.AssertCF(126, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_E]); // È
            ttt.AssertCF(128, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_o]); // Ö
            ttt.AssertCF(130, blackCF); // Ü
            ttt.AssertCF(132, blackCF); // Ä
            ttt.AssertCF(134, blackCF); // Ù
            ttt.AssertCF(136, blackCF); // Ï
            ttt.AssertCF(138, ColConfWin.coloredCF[(int)tstCERASColor.CERAS_E]); // Ë

            ttt.AssertCF(140, blackCF); // 0
            ttt.AssertCF(142, blackCF); // 1
            ttt.AssertCF(144, blackCF); // 2
            ttt.AssertCF(146, blackCF); // 3
            ttt.AssertCF(148, blackCF); // 4
            ttt.AssertCF(150, blackCF); // 5
            ttt.AssertCF(152, blackCF); // 6
            ttt.AssertCF(154, blackCF); // 7
            ttt.AssertCF(156, blackCF); // 8
            ttt.AssertCF(158, blackCF); // 9

        }
    }
}
