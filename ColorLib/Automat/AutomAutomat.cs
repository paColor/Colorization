/********************************************************************************
 *  Copyright 2020, Pierre-Alain Etique                                         *
 *                                                                              *
 *  This file is part of Coloriƨation.                                          *
 *                                                                              *
 *  Coloriƨation is free software: you can redistribute it and/or modify        *
 *  it under the terms of the GNU General Public License as published by        *
 *  the Free Software Foundation, either version 3 of the License, or           *
 *  (at your option) any later version.                                         *
 *                                                                              *
 *  Coloriƨation is distributed in the hope that it will be useful,             *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *  GNU General Public License for more details.                                *
 *                                                                              *
 *  You should have received a copy of the GNU General Public License           *
 *  along with Coloriƨation.  If not, see <https://www.gnu.org/licenses/>.      *
 *                                                                              *
 ********************************************************************************/

// Merci à Marie-Pierre Brungard pour le fanatstique travail effectué pour LireCouleur
// L'automate ci-dessous est une copie légèrement adaptée du coeur de LireCouleur.
// http://lirecouleur.arkaline.fr/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ColorLib
{
    public class AutomAutomat : AutomElement
    {

		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		/*****************************************************************************************************************
		
		La syntaxe de l'automate est plagiée de LireCouleur. Le contenu est à 95% le même.. 
		Bien sûr, l’automate est en fait un emboitement de structures. 
		Au premier niveau, c’est une liste d’éléments {e1, e2, e3, …, en}
		Chaque élément ex correspond à une structure « key : value »
		value est composé d’une liste de deux éléments notés [él1, él2]
		él1 est une liste d’éléments
		él2 est une liste de « key : value »
		où cette fois value est un triplet (ou liste de trois éléments) composé
		•	d’une liste l
		•	d’un string
		•	d’un nombre entier
		la liste l, contient soit un identifiant de fonction, ou une liste de un ou deux « key : value »
		où value est une expression régulière et key la façon de l’utiliser dans le mot qu’on travaille.

		Il y a donc finalement trois éléments de base :
		1.	« key : value », où key est un nom entre guillemets simples et value n’importe quel objet
		2.	La liste {} qui contient des objets séparés par une virgule
		3.	La liste [] qui contient des objets séparés par une virgule
		Je ne sais pas quelle est la différence entre la liste [] et la liste {}. Il faudrait vérifier dans Javascript.
		L’automate est donc une liste de key-value pairs, où les values sont des listes et on descend récursivement 
		dans les possibilités. Syntaxiquement, c’est donc finalement assez simple. Comme nous cherchons la sémantique de 
		cet automate, nous avons considéré une syntaxe plus sophistiquée.
		*)

		alphaChar =         "a" | "b" | ... | "z";
		digit =             "0" | "1" | ... | "9";
		quoteMark =         "'"
		charWithoutSlash =  alphaChar | digit | charactersWOCapitalsOrSlash
								(* en fait n'importe quel caractère qui peut se retrouver dans un texte, sauf les majuscules *)
		identifier =        alphaChar, { alphaChar | digit | "_" };
		char =              charWithoutSlash | "/";
		letter =            quoteMark, char, quoteMark;
		name =              quoteMark, identifier, quoteMark;
		regexRule =         charWithoutSlash, {charWithoutSlash}; 
								(* on se simplifie la vie en définissant une règle regex comme une suite de caractères sans "/" *)
		regex =             "/", regexRule, "/"
		ruleFilterComp =    ((quoteMark, "+", quoteMark) | (quoteMark, "-", quoteMark)), ":", regex, "i";
		ruleFilter =        ("{", ruleFilterComp, [",", ruleFilterComp], "}") | identifier;
		rule =              name, ":", "[", ruleFilter, ",", name, ",", digit, [",", name], "]" ;
		ruleList =          "{", rule, {",", rule}, "}";
		ruleOrder =         "[", name, {",", name}, "]";
		letterAutomat =     char, ":", "[", ruleOrder, ",", ruleList, "]";
		automat =           "{", letterAutomat, {",", letterAutomat}, "}";

		(* 
		Par rapport à l’automate original, nous avons ajouté la possibilité de mettre un nom à la fin de la ligne rule. 
		Si ce nom est présent, il indique le nom d’un flag qui doit être mis pour que la règle soit prise en compte.
		*) 
		
		***************************************************************************************************************************/


		static private string theAutomat =
			@"{
				'a' : [['u','il','in','nc_ai_fin','ai_fin','fais','i','n','m','nm','y_except','y_fin','yat','y', '*'],
						{'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'a_tilda',2],
						'm':[{'+':/m[mbp]/i},'a_tilda',2], // règle du m devant m, b, p
						'nm':[{'+':/n(s?)$/i},'a_tilda',2],
						'y_except':[{'-':/(^b|cob|cip|^k|^m|^f|mal)/i,'+':/y/i},'a',1], // exception : baye, cobaye, kayac, maya, mayonnaise, fayot (PAE - 10.03.20)
						'y_fin':[{'+':/y$/i},'E_comp',2], // (PAE - 10.03.20)
						'yat':[{'+': /yat/i},'a',1], // (PAE - 10.03.20) ayatollah
						'y':[{'+':/y/i},'E_comp',1],
						'u':[{'+':/u/i},'o_comp',2],
						'il':[{'+':/il($|l)/i},'a',1],
						'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3], // toute succession 'ain' 'aim' suivie d'une consonne ou d'une fin de mot
						'nc_ai_fin':[this.Regle_nc_ai_final,'E_comp',2],
						'ai_fin':[{'+':/i$/i},'e_comp',2],
						'fais':[{'-':/f/i,'+':/is[aeiouy]/i},'q', 2], // (PAE - 30.04.20) faisais et toutes les variations
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
						'h','erc_orc','cisole','c_muet_fin','onc_donc','nc_muet_fin','_spect','_inct','cciey','cc','apostrophe', 'voy_c_fin','@','*'],
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
						'voy_c_fin':[{'-':/[aeiouïé]/i,'+':/(s?)$/i}, 'k', 1], // [PAE 20.02.20 ajouté la règle]
						'c_muet_fin':[{'-':/taba|accro|estoma|bro|capo|cro|escro|raccro|caoutchou/i,'+':/(s?)$/i},'_muet',1], // exceptions traitées : tabac, accroc [PAE 20.02.20 ajouté les autres]
						'onc_donc':[{'-':/^on|^don/i},'k',1], // non exceptions traitées : onc, donc
						'nc_muet_fin':[{'-':/n/i,'+':/(s?)$/i},'_muet',1], // exceptions traitées : tous les mots terminés par *nc
						'_spect':[{'-':/spe/i,'+':/t(s?)$/i},'_muet',1], // respect, suspect, aspect
						'_inct':[{'-':/in/i,'+':/t(s?)$/i},'_muet',1], // instinct, succinct, distinct
						'cciey':[{'+':/c[eiyéèêëîï]/i},'k',1], // accident, accepter, coccyx
						'cc':[{'+':/c/i},'k',2], // accorder, accompagner
						'apostrophe':[{'+':/('|’)/i},'s',2], // apostrophe
						'*':[{},'k',1], 
						'@':[{'+':/$/i},'_muet',1]}], // + tous les *nc sauf ´onc´ et ´donc´
				'ç' : [['*'],
						{'*':[{},'s',1]}],
				'd' : [['d','aujourdhui','disole','except','dmuet','apostrophe', '*'],
						{'d':[{'+':/d/i},'d',2],
						//'except':[{'-':/(aï|oue)/i, '+':/(s?)$/i},'d',1], // aïd, caïd, oued
						'except':[this.Regle_finD,'d',1], // aïd, caïd, oued
						'aujourdhui':[{'-':/aujour/i},'d',1], // aujourd'hui
						'disole':[{'+':/$/i,'-':/^/i},'d',1], // exemple : d'abord
						'dmuet':[{'+':/(s?)$/i},'_muet',1], // un d suivi éventuellement d'un s ex. : retards
						'apostrophe':[{'+':/('|’)/i},'d',2], // apostrophe
						'*':[{},'d',1]}],
				'e' : [['conj_v_ier','uient','ien_0','ien','ien_2','een','except_en','_ent','clef','hier','adv_emment_fin',
						'ment','imparfait','verbe_3_pluriel','au',
						'eu_final','avoir','monsieur','jeudi','jeu_','eux','eur','eu','eu_accent_circ','in','eil','y','iy','ennemi','enn_debut_mot','dessus_dessous',
						'et','cet','t_final','eclm_final','est','d_except','drz_final','n','adv_emment_a','femme','lemme','em_gene','nm','tclesmesdes',
						'que_isole','que_gue_final','jtcnslemede','jean','ge','eoi','ex','ef','reqquechose','except_evr','2consonnes','abbaye','e_muet','e_caduc','e_deb', '@', '*'],
						{'_ent':[this.Regle_mots_ent,'a_tilda',2], // quelques mots (adverbes ou noms) terminés par ent
						'adv_emment_fin':[{'-':/emm/i,'+':/nt/i},'a_tilda',2], // adverbe avec 'emment' => se termine par le son [a_tilda]
						'ment':[this.Regle_ment,'a_tilda',2], // on considère que les mots terminés par 'ment' se prononcent [a_tilda] sauf s'il s'agit d'un verbe
						'imparfait':[{'-':/ai/i,'+':/nt$/i},'verb_3p',3], // imparfait à la 3ème personne du pluriel
						'verbe_3_pluriel':[{'+':/nt$/i},'q_caduc',1], // normalement, pratiquement tout le temps verbe à la 3eme personne du pluriel
						'clef':[{'-':/cl/i,'+':/f/i},'e_comp',2], // une clef
						'hier':[this.Regle_er,'E_comp',1], // encore des exceptions avec les mots terminés par 'er' prononcés 'R'
						'n':[{'+':/n[bcdfghjklmpqrstvwxzç]/i},'a_tilda',2],
						'adv_emment_a':[{'+':/mment/i},'a',1], // adverbe avec 'emment' => son [a]
						'eclm_final':[{'+':/[clm](s?)$/i},'E_comp',1], // donne le son [E] et le l ou le c se prononcent (ex. : miel, sec)
						'femme':[{'-':/f/i,'+':/mm/i},'a',1], // femme et ses dérivés => son [a]
						'lemme':[{'-':/l/i,'+':/mm/i},'E_comp',1], // lemme et ses dérivés => son [E]
						'em_gene':[{'+':/m[bcçdfghjklmnpqrstvwxz]/i},'a_tilda',2], // 'em' cas général => son [a_tilda]
						'uient':[{'-':/ui/i,'+':/nt$/i},'_muet',3], // enfuient, appuient, fuient, ennuient, essuient
						'conj_v_ier':[this.Regle_ient,'_muet',3], // verbe du 1er groupe terminé par 'ier' conjugué à la 3ème pers du pluriel
						'except_en':[{'-':/(exam|mino|édu)/i,'+':/n(s?)$/i},'e_tilda',2], // exceptions des mots où le 'en' final se prononce [e_tilda] (héritage latin)
						'een':[{'-':/é/i,'+':/n(s?)$/i},'e_tilda',2], // les mots qui se terminent par 'éen'
						'ien_0':[{'-':/ni/i,'+':/nt(s?)$/i},'a_tilda',2], // incovénient
						'ien':[{'-':/[bcdlmnrstvh]i/i,'+':/n([bcçdfghjklpqrstvwxz]|$)/i},'e_tilda',2], // certains mots avec 'ien' => son [e_tilda]
						'ien_2':[{'-':/ï/i,'+':/n([bcçdfghjklpqrstvwxz]|$)/i},'e_tilda',2], // mots avec 'ïen' => son [e_tilda]
						'nm':[{'+':/[nm]$/i},'a_tilda',2],
						'd_except': [{'-':/(^bl|^ou|^damn)/i, '+':/d(s?)$/i},'E',1], // [PAE 22.02.20] pour covrir oued, bled, damned
						'drz_final':[{'+':/[drz](s?)$/i},'e_comp',2], // e suivi d'un d,r ou z en fin de mot done le son [e]
						'que_isole':[{'-':/^qu/i,'+':/$/i},'q',1], // que isolé
						'que_gue_final':[{'-':/[gq]u/i,'+':/(s?)$/i},'q_caduc',1], // que ou gue final
						'jtcnslemede':[{'-':/^[jtcnslmd]/i,'+':/$/i},'q',1], // je, te, me, le, se, de, ne
						'tclesmesdes':[{'-':/^[tcslmd]/i,'+':/s$/i},'e_comp', 2], // mes, tes, ces, ses, les
						'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3], // toute succession 'ein' 'eim' suivie d'une consonne ou d'une fin de mot
						'avoir':[this.Regle_avoir,'y',2],
						'monsieur':[{'-':/si/i,'+':/ur/i},'x2',2],
						'eu_final': [{'+':/u(s?)$/i},'x2',2], // [PAE 23.02.20] --> envisager de supprimer toutes les règles eu.
						'jeudi':[{'-':/j/i,'+':/udi/i},'x2',2], // jeudi
						'jeu_':[{'-':/j/i,'+':/u/i},'x2',2], // tous les ´jeu*´ sauf jeudi
						'eur':[{'+':/ur/i},'x2',2],
						'eux':[{'+':/ux/i},'x2',2], // [PAE 23.02.20]
						'eu':[{'+':/u/i},'x2',2],
						'eu_accent_circ':[{'+':/û/i},'x2',2],
						'est':[{'-':/^/i,'+':/st$/i},'E_comp',3],
						'et':[{'-':/^/i,'+':/t$/i},'e_comp',2],
						'eil':[{'+':/il/i},'E_comp',1],
						'y':[{'+':/y[aeiouéèêààäôâ]/i},'E_comp',1],
						'iy':[{'+':/[iy]/i},'E_comp',2],
						'cet':[{'-':/^c/i,'+':/[t]$/i},'E_comp',1], // 'cet'
						't_final':[{'+':/[t]$/i},'E_comp',2], // donne le son [E] et le t ne se prononce pas
						'au':[{'+':/au/i},'o_comp',3],
						'ennemi':[{'-':/^/i,'+':/nnemi/i},'E_comp',1], // ennemi est l'exception ou 'enn' en début de mot se prononce 'èn' (cf. enn_debut_mot)
						'enn_debut_mot':[{'-':/^/i,'+':/nn/i},'a_tilda',2], // 'enn' en début de mot se prononce 'en'
						'ex':[{'+':/x/i},'E_comp',1], // e suivi d'un x se prononce è
						'ef':[{'+':/[bf](s?)$/i},'E_comp',1], // e suivi d'un f ou d'un b en fin de mot se prononce è
						'reqquechose':[{'-':/r/i,'+':/[bcçdfghjklmnpqrstvwxz](h|l|r)/i},'q',1], // re-quelque chose : le e se prononce 'e'
						'dessus_dessous':[{'-':/d/i,'+':/ss(o?)us/i},'q',1], // dessus, dessous : 'e' = e
						'except_evr':[{'+':/vr/i},'q',1], // chevrier, chevron, chevreuil
						'2consonnes':[{'+':/[bcçdfghjklmnpqrstvwxz]{2}/i},'E_comp',1], // e suivi de 2 consonnes se prononce è
						'e_deb':[{'-':/^/i},'q',1], // par défaut, un 'e' en début de mot se prononce [q]
						'abbaye':[{'-':/abbay/i,'+':/(s?)$/i},'_muet',1], // ben oui...
						'e_muet':[{'-':/[aeiouéèêà]/i,'+':/(s?)$/i},'_muet',1], // un e suivi éventuellement d'un 's' et précédé d'une voyelle ou d'un 'g' ex. : pie, geai
						'jean':[{'-':/j/i,'+':/an/i},'_muet',1], // jean
						'ge':[{'-':/g/i,'+':/[aouàäôâ]/i},'_muet',1], // un e précédé d'un 'g' et suivi d'une voyelle ex. : cageot
						'eoi':[{'+':/oi/i},'_muet',1], // un e suivi de 'oi' ex. : asseoir
						'e_caduc':[{'-':/[bcçdfghjklmnpqrstvwxzy]/i,'+':/(s?)$/i},'q_caduc',1], // un e suivi éventuellement d'un 's' et précédé d'une consonne ex. : correctes
						'*':[{},'q',1],
						'@':[{'+':/$/i},'q_caduc',1]
						}],
					'é' : [['*'],
							{'*':[{},'e',1]}],
					'è' : [['*'],
							{'*':[{},'E',1]}],
					'ê' : [['*'],
							{'*':[{},'E',1]}],
					'ë' : [['*'],
							{'*':[{},'E',1]}],
					'f' : [['f','oeufs', '*'],
							{'f':[{'+':/f/i},'f',2],
							 'oeufs':[{'-':/(oeu|œu)/i,'+':/s/i},'_muet',1], // oeufs et boeufs
							 '*':[{},'f',1]}],
					'g' : [['g','ao','eiy','aiguille','u_consonne','u','n','vingt','g_muet_oin',
							'g_muet_our','g_muet_an','g_muet_fin', '*'],
							{'g':[{'+':/g/i},'g',2],
							'n':[{'+':/n/i},'N',2],
							'ao':[{'+':/(a|o)/i},'g',1],
							'eiy':[{'+':/[eéèêëïiy]/i},'Z',1], // un 'g' suivi de e,i,y se prononce [Z]
							'g_muet_oin':[{'-':/oi(n?)/i},'_muet',1], // un 'g' précédé de 'oin' ou de 'oi' ne se prononce pas ; ex. : poing, doigt
							'g_muet_our':[{'-':/ou(r)/i},'_muet',1], // un 'g' précédé de 'our' ou de 'ou(' ne se prononce pas ; ex. : bourg
							'g_muet_an':[{'-':/(s|^ét|^r)an/i,'+':/(s?)$/i},'_muet',1], // sang, rang, étang
							'g_muet_fin':[{'-':/lon|haren/i},'_muet',1], // pour traiter les exceptions : long, hareng
							'aiguille':[{'-':/ai/i,'+':/u/i},'g',1], // encore une exception : aiguille et ses dérivés
							'vingt':[{'-':/vin/i,'+':/t/i},'_muet',1], // vingt
							'u_consonne':[{'+':/u[bcçdfghjklmnpqrstvwxz]/i},'g',1], // gu suivi d'une consonne se prononce [g][y]
							'u':[{'+':/u/i},'g_u',2],
							'*':[{},'g',1]}],
					'h' : [['*'],
							{'*':[{},'_muet',1]}],
					'i' : [['ing','n','m','nm','prec_2cons','lldeb','vill','mill2','tranquille',
							'ill','except_ill','bacille','ill_Ceras', '@ill','@il','ll','@il_Ceras',
							'll_Ceras','ui','ient_1','ient_2','ie','i_voyelle', '*'],
							{'ing':[{'-':/[bcçdfghjklmnpqrstvwxz]/i,'+':/ng$/i},'i',1],
							'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
							'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
							'nm':[{'+':/[n|m]$/i},'e_tilda',2],
							'prec_2cons':[{'-':/[ptkcbdgfv][lr]/i, '+':/[aäâeéèêëoôöuù]/i},'i_j',1], // précédé de 2 consonnes (en position 3), doit apparaître comme [ij] [PAE 20.02.20: rajouté les voyelles]
							'lldeb':[{'-':/^/i,'+':/ll/i},'i',1],
							'vill':[{'-':/v/i,'+':/ll/i},'i',1,IllCeras],
							//'mill':[{'-':/m/i,'+':/ll[^(et)]/i},'i',1,IllLireCouleur],
							'mill2':[{'-':/^m/i,'+':/ll[^(et)]/i},'i',1,IllCeras],
							'tranquille' : [{'-':/tranqu/i,'+':/ll/i},'i',1,IllCeras],
							'ill':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i',1,IllLireCouleur], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
							'ill_Ceras':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i_j_ill',3,IllCeras], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
							'except_ill':[this.Regle_ill,'i',1], // PAE - 07.05.20
							'bacille':[{'-':/(bac|dist|inst)/i,'+':/ll/i},'i',1], // il y tant de mots contenant 'bacill'... et les verbes...
							'@ill':[{'-':/[aeoœ]/i,'+':/ll/i},'j',3,IllLireCouleur], // par défaut précédé d'une voyelle et suivi de 'll' donne le son [j]
							'@il':[{'-':/[aeouœ]/i,'+':/l(s?)$/i},'j',2,IllLireCouleur], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [j]
							'@il_Ceras':[{'-':/[aeouœ]/i,'+':/l(s?)$/i},'j_ill',2, IllCeras], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [ill]
							'll':[{'+':/ll/i},'j',3, IllLireCouleur], // par défaut avec ll donne le son [j]
							'll_Ceras':[{'+':/ll/i},'j_ill',3, IllCeras], // par défaut avec ll donne le son [ill]
							'ui':[{'-':/u/i,'+':/ent/i},'i',1], // essuient, appuient
							'ient_1':[this.Regle_ient,'i',1], // règle spécifique pour différencier les verbes du premier groupe 3ème pers pluriel
							'ient_2':[{'+':/ent(s)?$/i},'j',1], // si la règle précédente ne fonctionne pas
							'ie':[{'+':/e(s|nt)?$/i},'i',1], // mots terminés par -ie(s|nt)
							'i_voyelle':[{'+':/[aäâeéèêëoôöuù]/i},'ji',1], // i suivi d'une voyelle donne [j]
							'*':[{},'i',1]}],
					'ï' : [['thai', 'aie', '*'],
							{'thai':[{'-':/t(h?)a/i},'j',1], // taï, thaï et dérivés
							'aie':[{'-':/[ao]/i,'+':/e/i},'j',1], // païen et autres
							'*':[{},'i',1]}],
					'î' : [['*'],
							{'*':[{},'i',1]}],
					'j' : [['*'],
							{'*':[{},'Z',1]}],
					'k' : [['*'],
							{'*':[{},'k',1]}],
					'l' : [['vill','mill','tranquille','illdeb','except_ill_l','bacille','ill','eil','ll','excep_il', 'apostrophe','lisole', '*'],
							{'vill':[{'-':/^vi/i,'+':/l/i},'l',2], // ville, village etc. => son [l]
							//'mill':[{'-':/^mi/i,'+':/l[^(et)]/i},'l',2], // mille, million, etc. => son [l] mais pas 'millet'
							'mill':[{'-':/mi/i,'+':/l[^(et)]/i},'l',2], // mille, million, etc. => son [l] mais pas 'millet'
							'tranquille':[{'-':/tranqui/i,'+':/l/i},'l',2], // tranquille => son [l]
							'illdeb':[{'-':/^i/i,'+':/l/i},'l',2], // 'ill' en début de mot = son [l] ; exemple : illustration
							'except_ill_l':[this.Regle_ill,'l',2],
							'bacille':[{'-':/(baci|disti|insti)/i,'+':/l/i},'l',2],
							'lisole':[{'+':/$/i,'-':/^/i},'l',1], // exemple : l'animal
							'ill':[{'-':/.i/i,'+':/l/i},'j',2], // par défaut, 'ill' donne le son [j]
							'll':[{'+':/l/i},'l',2], // à défaut de l'application d'une autre règle, 'll' donne le son [l]
							'excep_il':[{'-':/fusi|outi|genti|sourci|persi/i,'+':/(s?)$/i},'_muet',1], // les exceptions trouvées ou le 'l' à la fin ne se prononce pas : fusil, gentil, outil
							'eil':[{'-':/e(u?)i/i},'j',1], // les mots terminés en 'eil' ou 'ueil' => son [j]
							'apostrophe':[{'+':/('|’)/i},'l',2], // apostrophe
							'*':[{},'l',1]}],
					'm' : [['m','tomn','damn','misole','apostrophe', '*'],
							{'m':[{'+':/m/i},'m',2],
							'damn':[{'-':/da/i,'+':/n/i},'_muet',1], // Regle spécifique pour 'damné' et ses dérivés
							'tomn':[{'-':/to/i,'+':/n/i},'_muet',1], // Regle spécifique pour 'automne' et ses dérivés
							'*':[{},'m',1],
							'misole':[{'+':/$/i,'-':/^/i},'m',1], // exemple : m'a
							'apostrophe':[{'+':/('|’)/i},'m',2] // apostrophe
							}],
					'n' : [['ing','n','ment','urent','irent','erent','ent','nisole','apostrophe', '*'],
							{'n':[{'+':/n/i},'n',2],
							'ment':[this.Regle_verbe_mer,'verb_3p',2], // on considère que les verbent terminés par 'ment' se prononcent [_muet]
							'urent':[{'-':/ure/i,'+':/t$/i},'verb_3p',2], // verbes avec terminaisons en -urent
							'irent':[{'-':/ire/i,'+':/t$/i},'verb_3p',2], // verbes avec terminaisons en -irent
							'erent':[{'-':/ère/i,'+':/t$/i},'verb_3p',2], // verbes avec terminaisons en -èrent
							'ent':[{'-':/e/i,'+':/t$/i},'verb_3p',2],
							'ing':[{'-':/i/i,'+':/g$/i},'J',2],
							'*':[{},'n',1],
							'nisole':[{'+':/$/i,'-':/^/i},'n',1], // exemple : n'a
							'apostrophe':[{'+':/('|’)/i},'n',2] // apostrophe
							}],
					'o' : [['in','oignon','i',
							'tomn','monsieur','n','m','nm','y1','u','o','oeu_defaut','oe_0','oe_1','oe_2', 'oe_3','voeux','oeufs','noeud','oe_4','oe_defaut', '*'],
							{'in':[{'+':/i[nm]([bcçdfghjklpqrstvwxz]|$)/i},'w_e_tilda',3],
							'oignon':[{'-':/^/i,'+':/ignon/i},'o',2],
							'i':[{'+':/(i|î|y)/i},'oi',2], // [PAE 26.02.20] introduction du phonème oi pour pouvoir le marquer dans la convention CERAS
							'u':[{'+':/[uwûù]/i},'u',2], // son [u] : clou, clown
							'tomn':[{'-':/t/i,'+':/mn/i},'o',1], // Regle spécifique pour 'automne' et ses dérivés
							'monsieur':[{'-':/m/i,'+':/nsieur/i},'q',2],
							'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'o_tilda',2],
							'm':[{'+':/m[bcçdfgjklpqrstvwxz]/i},'o_tilda',2], // toute consonne sauf le m
							'nm':[{'+':/[nm]$/i},'o_tilda',2],
							'y1':[{'+':/y$/i},'oi',2], // [PAE 26.02.2020] introduction de 'oi' par exemple pour roy
							'o':[{'+':/o/i},'o',2], // exemple : zoo
							'voeux':[{'+':/eux/i},'x2',3], // voeux
							'noeud':[{'+':/eud/i},'x2',3], // noeud
							'oeufs':[{'+':/eufs/i},'x2',3], // traite oeufs et boeufs
							'oeu_defaut':[{'+':/eu/i},'x2',3], // exemple : oeuf
							'oe_0':[{'+':/ê/i},'oi',2],  // exemple : poêle [PAE 26.02.2020] remplacé par 'oi'
							'oe_1':[{'-':/c/i,'+':/e/i},'o',1], // exemple : coefficient
							'oe_2':[{'-':/m/i,'+':/e/i},'oi',2], // exemple : moelle [PAE 26.02.2020] remplacé par 'oi'
							'oe_3':[{'-':/f/i,'+':/e/i},'e',2], // exemple : foetus
							'oe_4':[{'-':/(gastr|électr|inc|min)/i,'+':/e/i},'o',1], // [PAE 26.02.2020] électroencéphalogramme, minoen
							'oe_defaut':[{'+':/e/i},'x2',2], // exemple : oeil
							'*':[{},'o',1]
							}],
					'œ' : [['oeu', '*'],
							{'oeu':[{'+':/u/i},'x2',2], // voeux, ... [PAE 29.02.20]
							'*':[{},'x2',1]
							}],
					'ô' : [['*'],
							{'*':[{},'o',1]
								}],
					'ö' : [['*'],
							{'*':[{},'o',1]
								}],
					'p' : [['h','oup','drap','trop','sculpt','sirop','sgalop','rps','amp','compt','bapti','sept','p', '*'],
							{'p':[{'+':/p/i},'p',2],
							'oup':[{'-':/[cl]ou/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : loup, coup
							'amp':[{'-':/c(h?)am/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : camp, champ
							'drap':[{'-':/dra/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : drap
							'trop':[{'-':/tro/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : trop
							'sculpt':[{'-':/scul/i,'+':/t/i},'_muet',1], // les exceptions avec un p muet : sculpter et les mots de la même famille
							'sirop':[{'-':/siro/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : sirop
							'sept':[{'-':/^se/i,'+':/t(s?)$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : sept
							'sgalop':[{'-':/[gs]alo/i,'+':/$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : galop
							'rps':[{'-':/[rm]/i,'+':/s$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : corps, camp
							'compt':[{'-':/com/i,'+':/t/i},'_muet',1], // les exceptions avec un p muet : les mots en *compt*
							'bapti':[{'-':/ba/i,'+':/ti/i},'_muet',1], // les exceptions avec un p muet : les mots en *bapti*
							'h':[{'+':/h/i},'f_ph',2],
							'*':[{},'p',1]}],
					'q' : [['qu','k', '*'],
							{'qu':[{'+':/u[bcçdfgjklmnpqrstvwxz]/i},'k',1],
							'k':[{'+':/u/i},'k_qu',2],
							'*':[{},'k',1]}],
					'r' : [['monsieur','messieurs','gars','r', '*'],
							{'monsieur':[{'-':/monsieu/i},'_muet',1],
							'messieurs':[{'-':/messieu/i},'_muet',1],
							'r':[{'+':/r/i},'R',2],
							'gars':[{'+':/s/i,'-':/ga/i},'_muet',2], // gars
							'*':[{},'R',1]}],
					's' : [['sch','h','s_final','parasit','para','mars','s','z','sisole',
							// 'smuet',
							'apostrophe', '@', '*'],
							{'sch':[{'+':/ch/i},'S',3], // schlem
							'h':[{'+':/h/i},'S',2],
							's_final':[this.Regle_s_final,'s',1], // quelques mots terminés par -us, -is, -os, -as
							'z':[{'-':/[aeiyouéèàâüûùëöêîô]/i,'+':/[aeiyouéèàâüûùëöêîô]/i},'z_s',1], // un s entre 2 voyelles se prononce [z]
							'parasit':[{'-':/para/i,'+':/it/i},'z_s',1], // parasit*
							'para':[{'-':/para/i},'s',1], // para quelque chose (parasol, parasismique, ...)
							's':[{'+':/s/i},'s',2], // un s suivi d'un autre s se prononce [s]
							'sisole':[{'+':/$/i,'-':/^/i},'s',1], // exemple : s'approche
							'mars':[{'+':/$/i,'-':/mar/i},'s',1], // mars
							//'smuet':[{'-':/(e?)/i,'+':/$/i},'_muet',1], // un s en fin de mot éventuellement précédé d'un e ex. : correctes
							'apostrophe':[{'+':/('|’)/i},'s',2], // apostrophe
							'*':[{},'s',1],
							'@':[{'+':/$/i},'_muet',1]}],
					't' : [['t_deb','t','tisole','except_tien','_tien','ex_tie','tie','ex_tiot','tiaot','tiaos','vingt',
							'ourt','_inct','_spect','_ct','_est','t_final','tmuet','apostrophe', '@', '*'],
							{'t':[{'+':/t/i},'t',2],
							't_deb':[{'-':/^/i},'t',1],
							'except_tien':[this.Regle_tien,'t',1], // quelques mots où 'tien' se prononce [t]
							'_tien':[{'+':/ien/i},'s_t',1],
							'ex_tie':[{'-':/minu/i,'+':/ie(r|z)/i},'t',1],
							'tie':[{'-':/(ambi|albu|cra|lvi|^essen|idio|iner|ini|minu|ipé|oten|phé)/i,'+':/ie/i},'s_t',1],
							'ex_tiot':[{'-':/(cré|plé|jé)/i,'+':/i[ao]/i},'s_t',1],
							'tiaot':[{'-':/([eéèêës]|[sc]en|an|f(l?)[uû]|ar|(ch|^str|galim|fum)[aâ]|rb[io])/i,'+':/i[ao]/i},'t',1],
							'tiaos':[{'+':/i[ao]/i},'s_t',1],
							'vingt':[{'-':/ving/i,'+':/$/i},'t',1], // vingt mais pas vingts
							'tisole':[{'+':/$/i,'-':/^/i},'t',1], // exemple : demande-t-il
							'ourt':[{'-':/(a|h|g)our/i,'+':/$/i},'t',1], // exemple : yaourt, yoghourt, yogourt
							'_est':[{'-':/es/i,'+':/(s?)$/i},'t',1], // test, ouest, brest, west, zest, lest
							'_inct':[{'-':/inc/i,'+':/(s?)$/i},'_muet',1], // instinct, succinct, distinct
							'_spect':[{'-':/spec/i,'+':/(s?)$/i},'_muet',1], // respect, suspect, aspect
							'_ct':[{'-':/c/i,'+':/(s?)$/i},'t',1], // tous les autres mots terminés par 'ct'
							't_final':[this.Regle_t_final,'t',1], // quelques mots où le ´t´ final se prononce
							'tmuet':[{'+':/(s?)$/i},'_muet',1], // un t suivi éventuellement d'un s ex. : marrants
							'*':[{},'t',1],
							'apostrophe':[{'+':/('|’)/i},'t',2], // apostrophe
							'@':[{'+':/$/i},'_muet',1]}],
					'u' : [['um','n','nm','ueil', '*'],
							{'um':[{'-':/[^aefo]/i,'+':/m$/i},'o',1],
							'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'x_tilda',2],
							'nm':[{'+':/[nm]$/i},'x_tilda',2],
							'ueil':[{'+':/eil/i},'x2',2], // mots terminés en 'ueil' => son [x2]
							'*':[{},'y',1]}],
					'û' : [['*'],
							{'*':[{},'y',1]}],
					'ù' : [['*'],
							{'*':[{},'y',1]}],
					'ü' : [['*'],
							{'*':[{},'y',1]}], // pour les mots allemands [PAE 11.07.2020]
					'v' : [['*'],
							{'*':[{},'v',1]}],
					'w' : [['wurst', '*'],
							{'wurst':[{'+':/(ur|ag|isi|estp|ei)/i},'v',1], // saucisse [PAE 23.02.20] modifié pour couvrir tous les cas de Lexique. Une règle complexe vaut-elle mieux que cinq simples?????
							'*':[{},'w',1]}], 
					'x' : [['six_dix','gz_1','gz_2','gz_3','gz_4','gz_5','_aeox','fix','_ix', '@', '*'],
							{'six_dix':[{'-':/(s|d)i/i},'s_x',1],
							'gz_1':[{'-':/^/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un x suivi d'une voyelle
							'gz_2':[{'-':/^(h?)e/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'ex' ou 'hex' suivi d'une voyelle
							'gz_3':[{'-':/^coe/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'coex' suivi d'une voyelle
							'gz_4':[{'-':/^ine/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'inex' suivi d'une voyelle
							'gz_5':[{'-':/^(p?)rée/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'réex' ou 'préex' suivi d'une voyelle
							'_aeox':[{'-':/[aeo]/i},'ks',1],
							'fix':[{'-':/fi/i},'ks',1],
							'_ix':[{'-':/(remi|obéli|astéri|héli|phéni|féli)/i},'ks',1],
							'*':[{},'ks',1],
							'@':[{'+':/$/i},'_muet',1]}],
					'y' : [['m','n','nm','abbaye','y_voyelle', '*'],
							{'y_voyelle':[{'+':/[aeiouéèàüëöêîôûù]/i},'j',1], // y suivi d'une voyelle donne [j]
							'abbaye':[{'-':/abba/i,'+':/e/i},'i', 1], // abbaye... bien irrégulier
							'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
							'm':[{'+':/m[mpb]/i},'e_tilda',2],
							'nm':[{'+':/[n|m]$/i},'e_tilda',2],
							'*':[{},'i',1]}],
					'z' : [['riz', 'iz', 'gaz', '@', '*'],
							{'riz':[{'-':/i/i,'+':/$/i},'_muet',1], // y suivi d'une voyelle donne [j]
							'iz':[{'-':/i/i,'+':/$/i},'z',1],
							'gaz':[{'-':/a/i,'+':/$/i},'z',1],
							'*':[{},'z',1],
							'@':[{'+':/$/i},'_muet',1]}],
					'0' : [['*'],
							{'*':[{},'chiffre',1]}],
					'1' : [['*'],
							{'*':[{},'chiffre',1]}],
					'2' : [['*'],
							{'*':[{},'chiffre',1]}],
					'3' : [['*'],
							{'*':[{},'chiffre',1]}],
					'4' : [['*'],
							{'*':[{},'chiffre',1]}],
					'5' : [['*'],
							{'*':[{},'chiffre',1]}],
					'6' : [['*'],
							{'*':[{},'chiffre',1]}],
					'7' : [['*'],
							{'*':[{},'chiffre',1]}],
					'8' : [['*'],
							{'*':[{},'chiffre',1]}],
					'9' : [['*'],
							{'*':[{},'chiffre',1]}],
					''' : [['*'],
							{'*':[{},'_muet',1]}],
					'’' : [['*'],
							{'*':[{},'_muet',1]}],
					'*' : [['*'],
							{'*':[{},'_muet',1]}]
				}";

		static public AutomAutomat autom;

		static public void InitAutomat()
		{
			logger.ConditionalDebug("InitAutomat");
			AutomLetter.InitAutomat();
			autom = new AutomAutomat(theAutomat);
		}

		private Dictionary<char, AutomLetter> automLetters;

        public AutomAutomat(string s)
            :base(s, 0)
        {
			logger.ConditionalDebug("AutomAutomat");
			/*
             * An AutomAutomat has the syntax
             * {List of AutomLetter separated by ','}
             */

			// Let's skip possible leading spaces
			int pos = 0;
            pos = GetNextChar(pos);
            // The char at pos must be an {
            Debug.Assert(s[pos] == '{', String.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomAutomat, on attend un \'{{\' en début d'automate.",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // Let's load the list of AutomLetters
            automLetters = new Dictionary<char, AutomLetter>(44); // there are 44 letters in our automata
            pos = GetNextChar(pos + 1);
            while (s[pos] != '}')
            {
                AutomLetter al = new AutomLetter(s, ref pos);
                automLetters.Add(al.Letter, al);
                pos = GetNextChar(pos + 1);
                // it is either ',' or '}'
                Debug.Assert(((s[pos] == ',') || (s[pos] == '}')), string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomAutomat, on attend une \',\' ou un \'}}\' après une lettre.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                if (s[pos] == ',')
                    pos = GetNextChar(pos + 1);
            } // while
            end = pos;
        } // Constructor public AutomAutomat(string s, ref int pos)

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<char, AutomLetter> k in automLetters)
                sb.AppendLine(k.Value.ToString());
            return sb.ToString();
        }

		/// <summary>
		/// Cherche les phonèmes dans <c>pw</c> et les complète <c>pw</c> pour qu'il contienne l'information.
		/// </summary>
		/// <param name="pw">Le <see cref="PhonWord"/> à analyser et à compléter avec ses phonèmes.</param>
		/// <param name="conf">La <see cref="Config"/> à utiliser au cours de cette analyse.</param>
		public void FindPhons(PhonWord pw, Config conf)
		{
			logger.ConditionalTrace("FindPhons");
			Debug.Assert(pw != null);

			int pos = 0;
			string w = pw.GetWord();
			AutomLetter al;

			while (pos < w.Length)
			{
				if (automLetters.TryGetValue(w[pos], out al))
				{
					al.FireRule(pw, ref pos, conf);
				}
				else if (automLetters.TryGetValue('*', out al))
				{
					// strange character encountered --> handle it as letter '*'
					al.FireRule(pw, ref pos, conf);
				} 
				else
				{
					// this should not happen!!
					string message = String.Format(ConfigBase.cultF, "La règle générique n'existe pas et on en aurait besoin...");
					throw new KeyNotFoundException(message);
				}
			} // while
		} // FindPhons

		public void CountPhons(ref int[] usedPhons)
			// counts the number of times each phoneme is used in the automat
		{
			foreach (KeyValuePair<char, AutomLetter> k in automLetters)
				k.Value.CountPhons(ref usedPhons);
		}
	}
}
