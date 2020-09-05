using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Collections.Generic;
using System.Text;

namespace ColorLibTest
{
    [TestClass]
    public class PhonWordTest
    {
        string s = @"Heureux qui, comme Ulysse, a fait un beau voyage,
                        Ou comme cestuy la qui conquit la toison,
                        Et puis est retourné, plein d'usage et raison,
                        Vivre entre ses parents le reste de son age!

                        Quand revoiray-je, helas, de mon petit village
                        Fumer la cheminee, et en quelle saison,
                        Revoiray-je le clos de ma pauvre maison,
                        Qui m'est une province, et beaucoup d'avantage?

                        Plus me plaist le sejour qu'on basty mes ayeux,
                        Que des palais Romains le front audacieux,
                        Plus que le marbre dur me plaist l'ardoise fine:

                        Plus mon Loyre Gaulois, que le Tybre Latin,
                        Plus mon petit Lyré, que le mont Palatin,
                        Et plus que l'air marin la doulceur Angevine.";


        private void CheckTextVsSyls(string txt, string[] syls, bool std, SylConfig.Mode inMode, bool mergApostrophe = false)
        {
            Config conf = new Config();
            TheText tt = new TheText(txt);
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            conf.sylConf.DoubleConsStd = std;
            conf.sylConf.mode = inMode;
            List<PhonWord> pws = tt.GetPhonWordList(conf, mergApostrophe);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
                pw.ColorizeSyls(conf);
            }
               

            pws[0].ComputeSyls(); // doit résister à deux appels de la méthode
            pws[0].ColorizeSyls(conf);
            for (int i = 0; i < syls.Length; i++)
            {
                Console.WriteLine(pws[i].AllStringInfo());
                Assert.AreEqual(syls[i], pws[i].Syllabes());
            }
        }

        [TestMethod]
        public void TestSyllabes1()
        {
            TheText.Init();
            string txt = @"audacieux";
            string syllabe = "au-da-cieux";
            Config conf = new Config();
            TheText tt = new TheText(txt);
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
                pw.ColorizeSyls(conf);
            }
                
            Console.WriteLine(pws[0].AllStringInfo());
            Assert.AreEqual(syllabe, pws[0].Syllabes());

            txt = @"colorƨation";
            syllabe = "co-lo-rƨa-tion";
            tt = new TheText(txt);
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
                pw.ColorizeSyls(conf);
            }
            Console.WriteLine(pws[0].AllStringInfo());
            Assert.AreEqual(syllabe, pws[0].Syllabes());
        }



        [TestMethod]
        public void TestSyllabesDuBellay1()
        {
            TheText.Init();

            string[] sylsEcritStd = new string[]
            {
                "Heu-reux", "qui", "com-me", "U-lys-se", "a", "fait", "un", "beau", "voy-a-ge",
                "Ou", "com-me", "ces-tuy", "la", "qui", "con-quit", "la", "toi-son",
                "Et", "puis", "est", "re-tour-né", "plein", "d'", "u-sa-ge", "et", "rai-son",
                "Vi-vre", "en-tre", "ses", "pa-rents", "le", "res-te", "de", "son", "a-ge",

                "Quand", "re-voi-ray", "je", "he-las", "de", "mon", "pe-tit", "vil-la-ge",
                "Fu-mer", "la", "che-mi-nee", "et", "en", "quel-le", "sai-son",
                "Re-voi-ray", "je", "le", "clos", "de", "ma", "pau-vre", "mai-son",
                "Qui", "m'", "est", "u-ne", "pro-vin-ce", "et", "beau-coup", "d'", "a-van-ta-ge",

                "Plus", "me", "plaist", "le", "se-jour", "qu'", "on", "bas-ty", "mes", "a-yeux",
                "Que", "des", "pa-lais", "Ro-mains", "le", "front", "au-da-cieux",
                "Plus", "que", "le", "mar-bre", "dur", "me", "plaist", "l'", "ar-doi-se", "fi-ne",

                "Plus", "mon", "Loy-re", "Gau-lois", "que", "le", "Ty-bre", "La-tin",
                "Plus", "mon", "pe-tit", "Ly-ré", "que", "le", "mont", "Pa-la-tin",
                "Et", "plus", "que", "l'", "air", "ma-rin", "la", "doul-ceur", "An-ge-vi-ne"
            };

            string[] sylsEcritNonDouble = new string[]
            {
                "Heu-reux", "qui", "co-mme", "U-ly-sse", "a", "fait", "un", "beau", "voy-a-ge",
                "Ou", "co-mme", "ces-tuy", "la", "qui", "con-quit", "la", "toi-son",
                "Et", "puis", "est", "re-tour-né", "plein", "d'", "u-sa-ge", "et", "rai-son",
                "Vi-vre", "en-tre", "ses", "pa-rents", "le", "res-te", "de", "son", "a-ge",

                "Quand", "re-voi-ray", "je", "he-las", "de", "mon", "pe-tit", "vi-lla-ge",
                "Fu-mer", "la", "che-mi-nee", "et", "en", "que-lle", "sai-son",
                "Re-voi-ray", "je", "le", "clos", "de", "ma", "pau-vre", "mai-son",
                "Qui", "m'", "est", "u-ne", "pro-vin-ce", "et", "beau-coup", "d'", "a-van-ta-ge",

                "Plus", "me", "plaist", "le", "se-jour", "qu'", "on", "bas-ty", "mes", "a-yeux",
                "Que", "des", "pa-lais", "Ro-mains", "le", "front", "au-da-cieux",
                "Plus", "que", "le", "mar-bre", "dur", "me", "plaist", "l'", "ar-doi-se", "fi-ne",

                "Plus", "mon", "Loy-re", "Gau-lois", "que", "le", "Ty-bre", "La-tin",
                "Plus", "mon", "pe-tit", "Ly-ré", "que", "le", "mont", "Pa-la-tin",
                "Et", "plus", "que", "l'", "air", "ma-rin", "la", "doul-ceur", "An-ge-vi-ne"
            };

            string[] sylsOralStd = new string[]
            {
                "Heu-reux", "qui", "comme", "U-lysse", "a", "fait", "un", "beau", "voy-age",
                "Ou", "comme", "ces-tuy", "la", "qui", "con-quit", "la", "toi-son",
                "Et", "puis", "est", "re-tour-né", "plein", "d'", "u-sage", "et", "rai-son",
                "Vivre", "entre", "ses", "pa-rents", "le", "reste", "de", "son", "age",

                "Quand", "re-voi-ray", "je", "he-las", "de", "mon", "pe-tit", "vil-lage",
                "Fu-mer", "la", "che-mi-nee", "et", "en", "quelle", "sai-son",
                "Re-voi-ray", "je", "le", "clos", "de", "ma", "pauvre", "mai-son",
                "Qui", "m'", "est", "une", "pro-vince", "et", "beau-coup", "d'", "a-van-tage",

                "Plus", "me", "plaist", "le", "se-jour", "qu'", "on", "bas-ty", "mes", "a-yeux",
                "Que", "des", "pa-lais", "Ro-mains", "le", "front", "au-da-cieux",
                "Plus", "que", "le", "marbre", "dur", "me", "plaist", "l'", "ar-doise", "fine",

                "Plus", "mon", "Loyre", "Gau-lois", "que", "le", "Tybre", "La-tin",
                "Plus", "mon", "pe-tit", "Ly-ré", "que", "le", "mont", "Pa-la-tin",
                "Et", "plus", "que", "l'", "air", "ma-rin", "la", "doul-ceur", "An-ge-vine"
            };

            string[] sylsEcritNonDoubleApostrophe = new string[]
            {
                "Heu-reux", "qui", "co-mme", "U-ly-sse", "a", "fait", "un", "beau", "voy-a-ge",
                "Ou", "co-mme", "ces-tuy", "la", "qui", "con-quit", "la", "toi-son",
                "Et", "puis", "est", "re-tour-né", "plein", "d'u-sa-ge", "et", "rai-son",
                "Vi-vre", "en-tre", "ses", "pa-rents", "le", "res-te", "de", "son", "a-ge",

                "Quand", "re-voi-ray", "je", "he-las", "de", "mon", "pe-tit", "vi-lla-ge",
                "Fu-mer", "la", "che-mi-nee", "et", "en", "que-lle", "sai-son",
                "Re-voi-ray", "je", "le", "clos", "de", "ma", "pau-vre", "mai-son",
                "Qui", "m'est", "u-ne", "pro-vin-ce", "et", "beau-coup", "d'a-van-ta-ge",

                "Plus", "me", "plaist", "le", "se-jour", "qu'on", "bas-ty", "mes", "a-yeux",
                "Que", "des", "pa-lais", "Ro-mains", "le", "front", "au-da-cieux",
                "Plus", "que", "le", "mar-bre", "dur", "me", "plaist", "l'ar-doi-se", "fi-ne",

                "Plus", "mon", "Loy-re", "Gau-lois", "que", "le", "Ty-bre", "La-tin",
                "Plus", "mon", "pe-tit", "Ly-ré", "que", "le", "mont", "Pa-la-tin",
                "Et", "plus", "que", "l'air", "ma-rin", "la", "doul-ceur", "An-ge-vi-ne"
            };

            CheckTextVsSyls(s, sylsEcritStd, true, SylConfig.Mode.ecrit);
            CheckTextVsSyls(s, sylsEcritNonDouble, false, SylConfig.Mode.ecrit);
            CheckTextVsSyls(s, sylsOralStd, true, SylConfig.Mode.oral);
            CheckTextVsSyls(s, sylsEcritNonDoubleApostrophe, false, SylConfig.Mode.ecrit, true);
        }

        public string TransformTextToPhons(string txt)
        {
            const int wordsPerLine = 10;

            StringBuilder sb = new StringBuilder();
            TheText.Init();
            Config conf = new Config();
            TheText tt = new TheText(txt);
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            int i = 0;
            int nrLines = pws.Count / wordsPerLine;
            for (int line = 0; line < nrLines; line++)
            {
                for (int wil = 0; wil < wordsPerLine - 1; wil++)
                {
                    sb.Append("\"");
                    sb.Append(pws[i].Phonetique());
                    sb.Append("\", ");
                    i++;
                }
                sb.Append("\"");
                sb.Append(pws[i].Phonetique());
                sb.AppendLine("\", ");
                i++;
            }
            int remainingWords = (pws.Count % wordsPerLine);
            for (int wil = 0; wil < remainingWords; wil++)
            {
                sb.Append("\"");
                sb.Append(pws[i].Phonetique());
                sb.Append("\", ");
                i++;
            }

            return sb.ToString();
        }

        private void CheckTextVsPhons(string txt, string[] phons)
        {
            Config conf = new Config();
            TheText tt = new TheText(txt);
            conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            for (int i = 0; i < phons.Length; i++)
            {
                Console.WriteLine(pws[i].AllStringInfo());
                Assert.AreEqual(phons[i], pws[i].Phonetique());
            }
        }

        string victor1 =
            @"Ami, poète, esprit, tu fuis notre nuit noire.
            Tu sors de nos rumeurs pour entrer dans la gloire;
            Et désormais ton nom rayonne aux purs sommets.
            Moi qui t’ai connu jeune et beau, moi qui t’aimais,
            Moi qui, plus d’une fois, dans nos altiers coups d’aile,
            Éperdu, m’appuyais sur ton âme fidèle,
            Moi, blanchi par les jours sur ma tête neigeant,
            Je me souviens des temps écoulés, et songeant
            A ce jeune passé qui vit nos deux aurores,
            A la lutte, à l’orage, aux arènes sonores,
            A l’art nouveau qui s’offre, au peuple criant oui,
            J’écoute ce grand vent sublime évanoui.

            Fils de la Grèce antique et de la jeune France,
            Ton fier respect des morts fut rempli d’espérance;
            Jamais tu ne fermas les yeux à l’avenir.
            Mage à Thèbes, druide au pied du noir menhir,
            Flamine aux bords du Tibre et brahme aux bords du Gange,
            Mettant sur l’arc du dieu la flèche de l’archange,
            D’Achille et de Roland hantant les deux chevets,
            Forgeur mystérieux et puissant, tu savais
            Tordre tous les rayons dans une seule flamme;
            Le couchant rencontrait l’aurore dans ton âme;
            Hier croisait demain dans ton fécond cerveau;
            Tu sacrais le vieil art aïeul de l’art nouveau;
            Tu comprenais qu’il faut, lorsqu’une âme inconnue
            Parle au peuple, envolée en éclairs dans la nue,
            L’écouter, l’accepter; l’aimer, ouvrir les coeurs;
            Calme, tu dédaignais l’effort vil des moqueurs
            Écumant sur Eschyle et bavant sur Shakespeare;
            Tu savais que ce siècle a son air qu’il respire,
            Et que, l’art ne marchant qu’en se transfigurant,
            C’est embellir le beau que d’y joindre le grand.
            Et l’on t’a vu pousser d’illustres cris de joie
            Quand le Drame a saisi Paris comme une proie,
            Quand l’antique hiver fut chassé par Floréal,
            Quand l’astre inattendu du moderne idéal
            Est venu tout à coup, dans le ciel qui s’embrase
            Luire, et quand l’Hippogriffe a relayé Pégase!

            Je te salue au seuil sévère du tombeau.
            Va chercher le vrai, toi qui sus trouver le beau.
            Monte l’âpre escalier. Du haut des sombres marches,
            Du noir pont de l’abîme on entrevoit les arches;
            Va! meurs! la dernière heure est le dernier degré.
            Pars, aigle, tu vas voir des gouffres à ton gré;
            Tu vas voir l’absolu, le réel, le sublime.
            Tu vas sentir le vent sinistre de la cime
            Et l’éblouissement du prodige éternel.
            Ton olympe, tu vas le voir du haut du ciel,
            Tu vas du haut du vrai voir l’humaine chimère,
            Même celle de Job, même celle d’Homère,
            Ame, et du haut de Dieu tu vas voir Jéhovah.
            Monte, esprit! Grandis, plane, ouvre tes ailes, va!

            Lorsqu’un vivant nous quitte, ému, je le contemple;
            Car entrer dans la mort, c’est entrer dans le temple
            Et quand un homme meurt, je vois distinctement
            Dans son ascension mon propre avènement.
            Ami, je sens du sort la sombre plénitude;
            J’ai commencé la mort par de la solitude,
            Je vois mon profond soir vaguement s’étoiler;
            Voici l’heure où je vais, aussi moi, m’en aller.
            Mon fil trop long frissonne et touche presque au glaive;
            Le vent qui t’emporta doucement me soulève,
            Et je vais suivre ceux qui m’aimaient, moi, banni.
            Leur œil fixe m’attire au fond de l’infini.
            J’y cours. Ne fermez pas la porte funéraire.

            Passons; car c’est la loi; nul ne peut s’y soustraire;
            Tout penche; et ce grand siècle avec tous ses rayons
            Entre en cette ombre immense où pâles nous fuyons.
            Oh! quel farouche bruit font dans le crépuscule
            Les chênes qu’on abat pour le bûcher d’Hercule!
            Les chevaux de la mort se mettent à hennir,
            Et sont joyeux, car l’âge éclatant va finir;
            Ce siècle altier qui sut dompter le vent contraire,
            Expire ô Gautier! toi, leur égal et leur frère,
            Tu pars après Dumas, Lamartine et Musset.
            L’onde antique est tarie où l’on rajeunissait;
            Comme il n’est plus de Styx il n’est plus de Jouvence.
            Le dur faucheur avec sa large lame avance
            Pensif et pas à pas vers le reste du blé;
            C’est mon tour; et la nuit emplit mon oeil troublé
            Qui, devinant, hélas, l’avenir des colombes,
            Pleure sur des berceaux et sourit à des tombes.";

        string[] phonVictor1 = new string[]
            {
                "ami", "pOEt", "EspRi", "ty", "fyi", "nOtR", "nyi", "nwaR", "ty", "sOR",
                "d°", "nO", "Rym2R", "puR", "@tRe", "d@", "la", "glwaR", "e", "dezORmE",
                "t§", "n§", "REjOn", "o", "pyR", "sOmE", "mwa", "ki", "t", "e",
                "kOny", "Z2n", "e", "bo", "mwa", "ki", "t", "EmE", "mwa", "ki",
                "plys", "d", "yn", "fwa", "d@", "nO", "altje", "kup", "d", "El",
                "epERdy", "m", "apyjE", "syR", "t§", "am", "fidEl", "mwa", "bl@Si", "paR",
                "le", "ZuR", "syR", "ma", "tEt", "nEZ@", "Z°", "m°", "suvj5", "de",
                "t@", "ekule", "e", "s§Z@", "a", "s°", "Z2n", "pase", "ki", "vi",
                "nO", "d2", "oROR", "a", "la", "lyt", "a", "l", "ORaZ", "o",
                "aREn", "sOnOR", "a", "l", "aR", "nuvo", "ki", "s", "OfR", "o",
                "p2pl", "kRij@", "ui", "Z", "ekut", "s°", "gR@", "v@", "syblim", "evanui",
                "fil", "d°", "la", "gREs", "@tik", "e", "d°", "la", "Z2n", "fR@s",
                "t§", "fje", "REspE", "de", "mOR", "fy", "R@pli", "d", "EspeR@s", "ZamE",
                "ty", "n°", "fERma", "le", "j2", "a", "l", "av°niR", "maZ", "a",
                "tEb", "dRyid", "o", "pje", "dy", "nwaR", "m@iR", "flamin", "o", "bOR",
                "dy", "tibR", "e", "bRam", "o", "bOR", "dy", "g@Z", "mEt@", "syR",
                "l", "aR", "dy", "dj2", "la", "flES", "d°", "l", "aRS@Z", "d",
                "aSil", "e", "d°", "ROl@", "@t@", "le", "d2", "S°vE", "fORZ2R", "misteRj2",
                "e", "pyis@", "ty", "savE", "tORdR", "tus", "le", "REj§", "d@", "yn",
                "s2l", "flam", "l°", "kuS@", "R@k§tRE", "l", "oROR", "d@", "t§", "am",
                "jER", "kRwazE", "d°m5", "d@", "t§", "fek§", "sERvo", "ty", "sakRE", "l°",
                "vjEj", "aR", "aj2l", "d°", "l", "aR", "nuvo", "ty", "k§pR°nE", "k",
                "il", "fo", "lORsk", "yn", "am", "5kOny", "paRl", "o", "p2pl", "@vOle",
                "@", "eklER", "d@", "la", "ny", "l", "ekute", "l", "aksEpte", "l",
                "Eme", "uvRiR", "le", "k2R", "kalm", "ty", "dedENE", "l", "EfOR", "vil",
                "de", "mOk2R", "ekym@", "syR", "ESil", "e", "bav@", "syR", "SakEsp°aR", "ty",
                "savE", "k°", "s°", "sjEkl", "a", "s§", "ER", "k", "il", "REspiR",
                "e", "k°", "l", "aR", "n°", "maRS@", "k", "@", "s°", "tR@sfigyR@",
                "s", "E", "@bEliR", "l°", "bo", "k°", "d", "i", "Zw5dR", "l°",
                "gR@", "e", "l", "§", "t", "a", "vy", "puse", "d", "ilystR",
                "kRi", "d°", "Zwa", "k@", "l°", "dRam", "a", "sEzi", "paRi", "kOm",
                "yn", "pRwa", "k@", "l", "@tik", "ive", "fy", "Sase", "paR", "flOReal",
                "k@", "l", "astR", "inat@dy", "dy", "mOdERn", "ideal", "E", "v°ny", "tu",
                "a", "ku", "d@", "l°", "sjEl", "ki", "s", "@bRaz", "lyiR", "e",
                "k@", "l", "ipOgRif", "a", "R°lEje", "pegaz", "Z°", "t°", "saly", "o",
                "s2j", "sevER", "dy", "t§bo", "va", "SERSe", "l°", "vRE", "twa", "ki",
                "sy", "tRuve", "l°", "bo", "m§t", "l", "apR", "Eskalje", "dy", "o",
                "de", "s§bR", "maRS", "dy", "nwaR", "p§", "d°", "l", "abim", "§",
                "@tR°vwa", "le", "aRS", "va", "m2R", "la", "dERnjER", "2R", "E", "l°",
                "dERnje", "dEgRe", "paR", "Egl", "ty", "va", "vwaR", "de", "gufR", "a",
                "t§", "gRe", "ty", "va", "vwaR", "l", "absOly", "l°", "ReEl", "l°",
                "syblim", "ty", "va", "s@tiR", "l°", "v@", "sinistR", "d°", "la", "sim",
                "e", "l", "ebluis°m@", "dy", "pROdiZ", "etERnEl", "t§", "Ol5p", "ty", "va",
                "l°", "vwaR", "dy", "o", "dy", "sjEl", "ty", "va", "dy", "o",
                "dy", "vRE", "vwaR", "l", "ymEn", "SimER", "mEm", "sEl", "d°", "ZOb",
                "mEm", "sEl", "d", "OmER", "am", "e", "dy", "o", "d°", "dj2",
                "ty", "va", "vwaR", "ZeOva", "m§t", "EspRi", "gR@di", "plan", "uvR", "te",
                "El", "va", "lORsk", "1", "viv@", "nu", "kit", "emy", "Z°", "l°",
                "k§t@pl", "kaR", "@tRe", "d@", "la", "mOR", "s", "E", "@tRe", "d@",
                "l°", "t@pl", "e", "k@", "1", "Om", "m2R", "Z°", "vwa", "dist5kt°m@",
                "d@", "s§", "ass@sj§", "m§", "pROpR", "avEn°m@", "ami", "Z°", "s@", "dy",
                "sOR", "la", "s§bR", "plenityd", "Z", "e", "kOm@se", "la", "mOR", "paR",
                "d°", "la", "sOlityd", "Z°", "vwa", "m§", "pROf§", "swaR", "vag°m@", "s",
                "etwale", "vwasi", "l", "2R", "u", "Z°", "vE", "osi", "mwa", "m",
                "@", "ale", "m§", "fil", "tRO", "l§", "fRisOn", "e", "tuS", "pREsk",
                "o", "glEv", "l°", "v@", "ki", "t", "@pORta", "dus°m@", "m°", "sulEv",
                "e", "Z°", "vE", "syivR", "s2", "ki", "m", "EmE", "mwa", "bani",
                "l2R", "2j", "fiks", "m", "atiR", "o", "f§", "d°", "l", "5fini",
                "Z", "i", "kuR", "n°", "fERme", "pa", "la", "pORt", "fyneRER", "pas§",
                "kaR", "s", "E", "la", "lwa", "nyl", "n°", "p2", "s", "i",
                "sustRER", "tu", "p@S", "e", "s°", "gR@", "sjEkl", "avEk", "tus", "se",
                "REj§", "@tR", "@", "sEt", "§bR", "im@s", "u", "pal", "nu", "fyj§",
                "O", "kEl", "faRuS", "bRyi", "f§", "d@", "l°", "kRepyskyl", "le", "SEn",
                "k", "§", "aba", "puR", "l°", "bySe", "d", "ERkyl", "le", "S°vo",
                "d°", "la", "mOR", "s°", "mEt", "a", "EniR", "e", "s§", "Zwa2",
                "kaR", "l", "aZ", "eklat@", "va", "finiR", "s°", "sjEkl", "altje", "ki",
                "sy", "d§pte", "l°", "v@", "k§tRER", "EkspiR", "O", "gotje", "twa", "l2R",
                "egal", "e", "l2R", "fRER", "ty", "paR", "apRE", "dyma", "lamaRtin", "e",
                "mysE", "l", "§d", "@tik", "E", "taRi", "u", "l", "§", "RaZ2nisE",
                "kOm", "il", "n", "E", "plys", "d°", "sti", "il", "n", "E",
                "plys", "d°", "Zuv@s", "l°", "dyR", "foS2R", "avEk", "sa", "laRZ", "lam",
                "av@s", "p@sif", "e", "pa", "a", "pa", "vER", "l°", "REst", "dy",
                "ble", "s", "E", "m§", "tuR", "e", "la", "nyi", "@pli", "m§",
                "2j", "tRuble", "ki", "d°vin@", "elas", "l", "av°niR", "de", "kOl§b", "pl2R",
                "syR", "de", "bERso", "e", "suRi", "a", "de", "t§b"
            };

        [TestMethod]
        public void TestPhons()
        {
            TheText.Init();
            CheckTextVsPhons(victor1, phonVictor1);
        }
    }
}
