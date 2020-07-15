﻿using System;
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
            TheText.Init();

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
            TheText.Init();

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
            TheText.Init();

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
    }
}
