using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorLib;

namespace ConsoleApp___Test
{
    class Program
    {
        static private string[] tstRFS = {
            @"{ '+':/$/ i, '-':/ ^/ i }",
            @"{ '-':/ ^/ i, '+':/ st$/ i }",
            // "this.regle_nc_ai_final,",
            @"{'+':/(\'|\')/i}"
        };

        static private string[] tstRS =
        {
            @"'chr':[{'+':/hr/i},'k',2] // de chrétien à synchronisé",
            @"'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3] // toute succession 'ein' 'eim' suivie d'une consonne ou d'une fin de mot"
        };

        static private string[] validRuleNames = { "chr", "in" };

        static private string[] tstLet =
        {
            @"'h' : [['*'],
				{'*':[{},'_muet',1]}]",
            @"'f' : [['f','oeufs', '*'],
				{'f':[{'+':/f/i},'f',2],
				 'oeufs':[{'-':/(oeu|œu)/i,'+':/s/i},'_muet',1], // oeufs et boeufs
				 '*':[{},'f',1]}]",
            @"'i' : [['ing','n','m','nm','prec_2cons','lldeb','vill','mill','tranquille',
				'ill','@ill','@il','ll','ui','ient_1','ient_2','ie','i_voyelle', '*'],
				{'ing':[{'-':/[bcçdfghjklmnpqrstvwxz]/i,'+':/ng$/i},'i',1],
				'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
				'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
				'nm':[{'+':/[n|m]$/i},'e_tilda',2],
                'prec_2cons':[{'-':/[ptkcbdgfv][lr]/i},'i',1], // précédé de 2 consonnes (en position 3), doit apparaître comme [ij]
				'lldeb':[{'-':/^/i,'+':/ll/i},'i',1],
				'vill':[{'-':/v/i,'+':/ll/i},'i',1],
				'mill':[{'-':/m/i,'+':/ll/i},'i',1],
				'tranquille':[{'-':/tranqu/i,'+':/ll/i},'i',1],
				'ill':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i',1], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
				'@ill':[{'-':/[aeo]/i,'+':/ll/i},'j',3], // par défaut précédé d'une voyelle et suivi de 'll' donne le son [j]
				'@il':[{'-':/[aeou]/i,'+':/l(s?)$/i},'j',2], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [j]
				'll':[{'+':/ll/i},'j',3], // par défaut avec ll donne le son [j]
				'ui':[{'-':/u/i,'+':/ent/i},'i',1], // essuient, appuient
				//'ient_1':[this.regle_ient,'i',1], // règle spécifique pour différencier les verbes du premier groupe 3ème pers pluriel
				'ient_2':[{'+':/ent(s)?$/i},'j',1], // si la règle précédente ne fonctionne pas
				'ie':[{'+':/e(s|nt)?$/i},'i',1], // mots terminés par -ie(s|nt)
				'i_voyelle':[{'+':/[aäâeéèêëoôöuù]/i},'j',1], // i suivi d'une voyelle donne [j]
				'*':[{},'i',1]}]"
        };

        static private string[] tstAut =
        {
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
				//'nc_ai_fin':[this.regle_nc_ai_final,'E_comp',2],
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
				'@':[{},'_muet',1]}], // + tous les *nc sauf ´onc´ et ´donc´
			}"
		};

		static void FindUsedPhons()
		{
			int[] usedPhons = new int[(int)Phonemes.lastPhon];
			for (int i = 0; i < (int)Phonemes.lastPhon; i++)
			{
				usedPhons[i] = 0;
			}
			AutomAutomat.autom.CountPhons(ref usedPhons);
			for (Phonemes p = Phonemes.firstPhon; p < Phonemes.lastPhon; p++)
			{
				Console.WriteLine(p.ToString() + ": " + usedPhons[(int)p]);
				_ = Console.ReadLine();
			}
		}


        static void Main(string[] args)
        {

			// FindUsedPhons();


			/* Obsolete tests
			for (var i = 0; i < tstRFS.Length; i++)
			{
				AutomRuleFilter arf;
				int pos = 0;
				arf = new AutomRuleFilter(tstRFS[i], ref pos);
				Console.WriteLine(arf.ToString());
				_ = Console.ReadLine();
			}

			List<string> vRN = new List<string>(validRuleNames);
			vRN.Sort();
			for (var i = 0; i < tstRS.Length; i++)
			{
				AutomRule ar;
				int pos = 0;
				ar = new AutomRule(tstRS[i], ref pos, vRN);
				Console.WriteLine(ar.ToString());
				_ = Console.ReadLine();
			}

			for (var i = 0; i < tstLet.Length; i++)
			{
				AutomLetter al;
				int pos = 0;
				al = new AutomLetter(tstLet[i], ref pos);
				Console.WriteLine(al.ToString());
				_ = Console.ReadLine();
			}

			foreach (var s in tstAut)
			{
				AutomAutomat aa = new AutomAutomat(s);
				Console.WriteLine(aa.ToString());
				_ = Console.ReadLine();
			}
			*/
		}
    }
}
