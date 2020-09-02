using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ColorLibTest
{
    [TestClass]
    public class AutomAutomatTest
    {
        [TestMethod]
        public void TestAutomAutomat()
        {
            AutomAutomat aa;
            int pos;
            string tstAA;
            string rTxt;
            Regex r;

            // ************************************************ TEST 1 *******************************************************
            tstAA = 
				@"{
					'a' : [['u','il','in','nc_ai_fin','ai_fin','i','n','m','nm','y_except','y', '*'],
							{'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'a_tilda',2],
							'm':[{'+':/m[mbp]/i},'a_tilda',2], // règle du m devant m, b, p
							'nm':[{'+':/n(s?)$/i},'a_tilda',2],
							'y_except':[{'-':/(^b|cob|cip)/i,'+':/y/i},'a',1], // exception : baye, cobaye
							'y':[{'+':/y/i},'E_comp',1],
							'u':[{'+':/u/i},'o_comp',2],
							'il':[{'+':/il($|l)/i},'a',1],
							'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3], // toute succession 'ain' 'aim' suivie d'une consonne ou d'une fin de mot
							'nc_ai_fin':[this.Regle_nc_ai_final,'E_comp',2],
							'ai_fin':[{'+':/i$/i},'e_comp',2],
							'i':[{'+':/[iî]/i},'E_comp',2],
							'*':[{},'a',1]}],
					'â' : [['*'],
							{'*':[{},'a',1]}],
					'à' : [['*'],
							{'*':[{},'a',1]}],
					'b' : [['b','plomb', '*'],
							{'b':[{'+':/b/i},'b',2],
							'plomb':[{'-':/plom/i,'+':/(s?)$/i},'_muet',1], // le ´b´ à la fin de plomb ne se prononce pas
							'*':[{},'b',1]}],
					'c' : [['eiy','choeur_1','choeur_2','chor','psycho','brachio','cheo','chest','chiro','chlo_chlam','chr',
							'h','erc_orc','cisole','c_muet_fin','onc_donc','nc_muet_fin','_spect','_inct','cciey','cc','apostrophe', '@','*'],
							{'choeur_1':[{'+':/hoe/i},'k',2],
							'choeur_2':[{'+':/hœ/i},'k',2],
							'chor':[{'+':/hor/i},'k',2], // tous les ´choral, choriste´... exceptions non traitées : chorizo, maillechort
							'psycho':[{'-':/psy/i,'+':/ho/i},'k',2], // tous les ´psycho´ quelque chose
							'brachio':[{'-':/bra/i,'+':/hio/i},'k',2], // brachiosaure, brachiocéphale
							'cheo':[{'+':/héo/i},'k',2], // archéo..., trachéo...
							'chest':[{'+':/hest/i},'k',2], // orchestre et les mots de la même famille
							'chiro':[{'+':/hiro[p|m]/i},'k',2], // chiroptère, chiromancie
							'chlo_chlam':[{'+':/hl(o|am)/i},'k',2], // chlorure, chlamyde
							'chr':[{'+':/hr/i},'k',2], // de chrétien à synchronisé
							'h':[{'+':/h/i},'S',2],
							'eiy':[{'+':/[eiyéèêëîï]/i},'s_c',1],
							'cisole':[{'+':/$/i,'-':/^/i},'s_c',1], // exemple : c'est
							'erc_orc':[{'-':/[e|o]r/i,'+':/(s?)$/i},'_muet',1], // clerc, porc,
							'c_muet_fin':[{'-':/taba|accro/i,'+':/(s?)$/i},'_muet',1], // exceptions traitées : tabac, accroc
							'onc_donc':[{'-':/^on|^don/i},'k',1], // non exceptions traitées : onc, donc
							'nc_muet_fin':[{'-':/n/i,'+':/(s?)$/i},'_muet',1], // exceptions traitées : tous les mots terminés par *nc
							'_spect':[{'-':/spe/i,'+':/t(s?)$/i},'_muet',1], // respect, suspect, aspect
							'_inct':[{'-':/in/i,'+':/t(s?)$/i},'_muet',1], // instinct, succinct, distinct
							'cciey':[{'+':/c[eiyéèêëîï]/i},'k',1], // accident, accepter, coccyx
							'cc':[{'+':/c/i},'k',2], // accorder, accompagner
							'apostrophe':[{'+':/(\'|\’)/i},'s',2], // apostrophe
							'*':[{},'k',1], 
							'@':[{},'_muet',1]}] // + tous les *nc sauf ´onc´ et ´donc´
				}";
            pos = 0;
            aa = new AutomAutomat(tstAA);
            rTxt = aa.ToString();
			r = new Regex(@"letter: a\r\n");
			Assert.IsTrue(r.IsMatch(rTxt));
			r = new Regex(@"letter: â\r\n");
			Assert.IsTrue(r.IsMatch(rTxt));
			r = new Regex(@"letter: à\r\n");
			Assert.IsTrue(r.IsMatch(rTxt));
			r = new Regex(@"letter: b\r\n");
			Assert.IsTrue(r.IsMatch(rTxt));
			r = new Regex(@"letter: c\r\n");
			Assert.IsTrue(r.IsMatch(rTxt));

		}

		private void CheckPhons(List<PhonWord> pws, int wordI, string mot, string phonetique)
		{
			Console.WriteLine(pws[wordI].AllStringInfo());
			if (mot != "")
				Assert.AreEqual(mot, pws[wordI].GetWord());
			Assert.AreEqual(phonetique, pws[wordI].Phonetique());
		}

		[TestMethod]
		public void TestFindPhons1()
		{
			TheText tt;
			List<PhonWord> pws;
			int wordI;
			string rTxt;

			TheText.Init();

			Config conf = new Config();
			tt = new TheText(@"briefing, berlingot, sapin, imbécile, limbe, afin, prier, ville, paille, triage,
                               appartient, amplifient, glorifient, avec, franc, spray, abbaye");

			conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;

			pws = tt.GetPhonWordList(conf);

			CheckPhons(pws, 0, "briefing", "bRij°fiG");
			CheckPhons(pws, 1, "berlingot", "bERl5gO");
			CheckPhons(pws, 2, "sapin", "sap5");
			CheckPhons(pws, 3, "imbécile", "5besil");
			CheckPhons(pws, 4, "limbe", "l5b");
			CheckPhons(pws, 5, "afin", "af5");
			CheckPhons(pws, 6, "prier", "pRije");
			CheckPhons(pws, 7, "ville", "vil");
			CheckPhons(pws, 8, "paille", "paj");
			CheckPhons(pws, 9, "triage", "tRijaZ");
			CheckPhons(pws, 10, "appartient", "apaRtj5");
			CheckPhons(pws, 11, "amplifient", "@plifi");
			CheckPhons(pws, 12, "glorifient", "glORifi");
			CheckPhons(pws, 13, "avec", "avEk");
			CheckPhons(pws, 14, "franc", "fR@");
			CheckPhons(pws, 15, "spray", "spRE");
			CheckPhons(pws, 16, "abbaye", "abEi");

		}

		[TestMethod]
		public void TestFindPhons2()
		{
			TheText tt;
			List<PhonWord> pws;

			TheText.Init();
			Config conf = new Config();
			tt = new TheText(@"bredouilla, ouest, bled, blé, caïd, vieux. oeuf, wapiti, kiwi, weimar, wurst, noix, royal,
							   poissons, australopithèque");

			conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.lirecouleur;

			pws = tt.GetPhonWordList(conf);

			CheckPhons(pws, 0, "bredouilla", "bR°duja");
			CheckPhons(pws, 1, "ouest", "uEst");
			CheckPhons(pws, 2, "bled", "blEd");
			CheckPhons(pws, 3, "blé", "ble");
			CheckPhons(pws, 4, "caïd", "kaid");
			CheckPhons(pws, 5, "vieux", "vj2");
			CheckPhons(pws, 6, "oeuf", "2f");
			CheckPhons(pws, 7, "wapiti", "wapiti");
			CheckPhons(pws, 8, "kiwi", "kiwi");
			CheckPhons(pws, 9, "weimar", "vEmaR");
			CheckPhons(pws, 10, "wurst", "vyRs");
			CheckPhons(pws, 11, "noix", "nwa");
			CheckPhons(pws, 12, "royal", "Rwaal");
			CheckPhons(pws, 13, "poissons", "pwas§");
			CheckPhons(pws, 14, "australopithèque", "ostRalOpitEk");
		}

		private void CheckTextVsPhonetique(string txt, string[] phons, ColConfWin.IllRule ill)
		{
			Config conf = new Config();
			TheText tt = new TheText(txt);
			conf.colors[PhonConfType.phonemes].IllRuleToUse = ill;
			List<PhonWord> pws = tt.GetPhonWordList(conf);
			for (int i = 0; i < phons.Length; i++)
			{
				Console.WriteLine(pws[i].AllStringInfo());
				Assert.AreEqual(phons[i], pws[i].Phonetique());
			}
		}

		[TestMethod]
		public void TestFindPhons3()
		{
			TheText.Init();
			string txt =
				@"ayons, balaya, ayatollah, kayac, tokay, mayonnaise, fayot, maya, himalaya, crayeux, paresse, abesses,
				  dilemme, impeccable, chevrier, caramels, bedonnant, faisons, affaisseraient, refaisaient, tranquille,
				  illégalement, lilliputien, millimétré, distillerait, tranquillises, tranquillités, tranquillisantes,
				  tranquillos, désillusionné, distiller, illogisme, illustraient, illégalement, illumineront, imbécillité,
				  instillassiez, millésime, millionnaire, multimilliardaires, multimillionnaire, villégiature, villageoises,
				  villa,
				  examen, minoen, gastroentérologue, électroencéphalographie, acclamation, abdomens";

			string[] phonetique = new string[]
			{
				"Ej§", "balEja", "ajatOla", "kajak", "tOkE", "majOnEz", "fajO", "maja", "imalaja", "kREj2", "paREs", "abEs",
				"dilEm", "5pEkabl", "S°vRije", "kaRamEl", "b°dOn@", "f°z§", "afEs°RE", "R°f°zE", "tR@kil", "ilegal°m@",
				"lilipysj5", "milimetRe", "distil°RE", "tR@kiliz", "tR@kilite", "tR@kiliz@t", "tR@kilOs", "dezilyzjOne",
				"distile", "ilOZism", "ilystRE", "ilegal°m@", "ilymin°R§", "5besilite", "5stilasje", "milezim", "miljOnER",
				"myltimiljaRdER", "myltimiljOnER", "vileZjatyR", "vilaZwaz", "vila",
				"Egzam5", "minO5", "gastRO@teROlOg", "elEktRO@sefalOgRafi", "aklamasj§", "abdOmEn"
			};
			CheckTextVsPhonetique(txt, phonetique, ColConfWin.IllRule.lirecouleur);
		}

		[TestMethod]
		public void TestFindPhons4()
		{
			TheText tt;
			List<PhonWord> pws;

			TheText.Init();

			Config conf = new Config();
			tt = new TheText(
				@"ayons, balaya, ayatollah, kayac, tokay, mayonnaise, fayot, maya, himalaya, crayeux, paresse, abesses,
				  dilemme, impeccable, chevrier, caramels, bedonnant, faisons, affaisseraient, refaisaient, tranquille,
				  illégalement, lilliputien, millimétré, distillerait, tranquillises, tranquillités, tranquillisantes,
				  tranquillos, désillusionné, distiller, illogisme, illustraient, illégalement, illumineront, imbécillité,
				  instillassiez, millésime, millionnaire, multimilliardaires, multimillionnaire, villégiature, villageoises,
				  villa,
				  examen, minoen, gastroentérologue, électroencéphalographie"
			);

			conf.colors[PhonConfType.phonemes].IllRuleToUse = ColConfWin.IllRule.ceras;

			string[] phonetique = new string[]
			{
				"Ej§", "balEja", "ajatOla", "kajak", "tOkE", "majOnEz", "fajO", "maja", "imalaja", "kREj2", "paREs", "abEs",
				"dilEm", "5pEkabl", "S°vRije", "kaRamEl", "b°dOn@", "f°z§", "afEs°RE", "R°f°zE", "tR@kil", "ilegal°m@",
				"lilipysj5", "milimetRe", "distil°RE", "tR@kiliz", "tR@kilite", "tR@kiliz@t", "tR@kilOs", "dezilyzjOne",
				"distile", "ilOZism", "ilystRE", "ilegal°m@", "ilymin°R§", "5besilite", "5stilasje", "milezim", "miljOnER",
				"myltimiljaRdER", "myltimiljOnER", "vileZjatyR", "vilaZwaz", "vila",
				"Egzam5", "minO5", "gastRO@teROlOg", "elEktRO@sefalOgRafi"
			};

			pws = tt.GetPhonWordList(conf);
			for (int i = 0; i < phonetique.Length; i++)
			{
				Console.WriteLine(pws[i].AllStringInfo());
				Assert.AreEqual(phonetique[i], pws[i].Phonetique());
			}
		}

		[TestMethod]
		public void TestFindPhons5()
		{
			TheText.Init();
			string txt =
				@"abeille, accueillant, vrai";

			string[] phonetique = new string[]
			{
				"abEj", "ak2j@", "vRE"
			};

			CheckTextVsPhonetique(txt, phonetique, ColConfWin.IllRule.ceras);
		}


	} // class
} // namespace
