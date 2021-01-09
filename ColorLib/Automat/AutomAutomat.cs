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

'a' : [['u','il','in','nc_ai_fin','ai_fin','fais','i','n','m','adam','nm','y_except','y_fin','yat',
		'taylor','y','ae_e','coach','*'],
		{'u':[{'+':/u/i},'o_comp',2],
		'il':[{'+':/il((s?)$|l)/i},'a',1],
		'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3], // toute succession 'ain' 'aim' suivie d'une consonne ou d'une fin de mot
		'nc_ai_fin':[this.Regle_nc_ai_final,'E_comp',2],
		'ai_fin':[{'+':/i$/i},'e_comp',2],
		'fais':[{'-':/f/i,'+':/is[aeiouy]/i},'q', 2], // (PAE - 30.04.20) faisais et toutes les variations
		'i':[{'+':/[iî]/i},'E_comp',2],
		'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'a_tilda',2],
		'm':[{'+':/m[bp]/i},'a_tilda',2], // règle du m devant b, p
		'adam':[{'-':/^ad/i,'+':/m(s?)$/i},'a_tilda',2],
		'nm':[{'+':/n(s?)$/i},'a_tilda',2],
		'y_except':[{'-':/(^b|cob|cip|^k|^m|^f|mal|bat|^bisc)/i,'+':/y/i},'a',1], // exception : baye, cobaye, kayac, maya, mayonnaise, fayot (PAE - 10.03.20)
		'y_fin':[{'+':/y(s?)$/i},'E_comp',2], // (PAE - 10.03.20) - 'pays' est une eexception traitée dans AutomDictionary.
		'yat':[{'+': /yat/i},'a',1], // (PAE - 10.03.20) ayatollah
		'taylor':[{'-':/t/i,'+':/ylor/i},'E_comp', 2],
		'y':[{'+':/y/i},'E_comp',1],
		'ae_e': [{'+':/e/i},'e',2],
		'coach':[{'-':/co/i,'+':/(ch|lt)/i},'_muet', 1],
		'*':[{},'a',1]}],
'â' : [['*'],
		{'*':[{},'a',1]}],
'à' : [['*'],
		{'*':[{},'a',1]}],
'b' : [['b','plomb', '*'],
		{'b':[{'+':/b/i},'b',2],
		'plomb':[{'-':/om/i,'+':/(s?)$/i},'_muet',1], // le ´b´ à la fin de plomb ne se prononce pas
		'*':[{},'b',1]}],
'c' : [['eiy','choeur','psycho','brachio','schizo','tech','tachy','batra','chK','h',
		'cciey','cc','cisole','c_muet_fin','c_k_fin','@',
		'ct_fin','apostrophe','coe','seconde','*'],
		{'eiy':[{'+':/([eiyéèêëîï]|ae)/i},'s_c',1],
		'choeur':[{'+':/h(oe|œ|or|éo|r|estr|esti|irop|irom|lo|lam)/i},'k',2],
		'psycho':[{'-':/psy/i,'+':/h[oa]/i},'k',2], // tous les ´psycho´ quelque chose
		'brachio':[{'-':/bra/i,'+':/hio/i},'k',2], // brachiosaure, brachiocéphale
		'schizo':[{'-':/s/i,'+':/(hi[aoz]|hato)/i},'k',2], // schizo, eschatologie
		'tech':[{'-':/te/i,'+':/hn/i},'k',2], // technique et tous ses dérivés
		'tachy':[{'-':/ta/i,'+':/hy/i},'k',2],
		'batra':[{'-':/batra/i,'+':/h/i},'k',2],
		'chK':[this.Regle_ChK,'k',2], // pour les cas qui n'ont pas été reconnus par les règles précédentes
		'h':[{'+':/h/i},'S',2],
		'cciey':[{'+':/c[eiyéèêëîï]/i},'k',1], // accident, accepter, coccyx
		'cc':[{'+':/[ck]/i},'k',2], // accorder, accompagner
		'cisole':[{'+':/$/i,'-':/^/i},'s_c',1], // exemple : c'est
		'c_muet_fin':[{'-':/taba|accro|estoma|bro|capo|cro|escro|raccro|caoutchou|mar/i,'+':/(s?)$/i},'_muet',1], // exceptions traitées : tabac, accroc [PAE 20.02.20 ajouté les autres]
		'c_k_fin':[{'-':/([aeiouïé]|^on|don|ar|ur|s|l)/i,'+':/(s?)$/i}, 'k', 1], // [PAE 20.02.20 ajouté la règle]
		'@':[{'+':/(s?)$/i},'_muet',1],
		'ct_fin':[{'-':/(spe|in)/i,'+':/t(s?)$/i},'_muet',1], // respect, suspect, aspect
		'apostrophe':[{'+':/('|’)/i},'s_c',2], // il faut aussi cette règle car l'appostrophe n'est pas toujours filtrée.
		'coe':[{'+':/(œ)(l|n|c)/i},'s_c',1],
		'seconde':[{'-':/se/i,'+':/ond/i},'g',1],
		'*':[{},'k',1]}], 
'ç' : [['*'],
		{'*':[{},'s',1]}],
'd' : [['d','disole','except','dmuet','dt',
		'*'],
		{'d':[{'+':/d/i},'d',2],
		'except':[this.Regle_finD,'d',1], // aïd, caïd, oued
		'disole':[{'+':/$/i,'-':/^/i},'d',1], // exemple : d'abord
		'dmuet':[{'+':/(s?)$/i},'_muet',1], // un d suivi éventuellement d'un s ex. : retards
		'dt':[{'+':/t/i},'_muet',1], // un d suivi t ne se prononce pas ex: cronstadt
		'*':[{},'d',1]}],
'e' : [['conj_v_ier','uient','ien_0','scien','orient','ien','ien2',
		'examen','zen','_ent',
		'adv_emment_fin','ment','imparfait','verbe_3_pluriel','hier','au',
		'avoir','eu','in','orgueil','eil','y','iy',//'ennemi',
		'enn_debut_mot',
		't_final','eclm_final','d_except','drz_final','except_en2','n','adv_emment_a',
		'lemme','em_gene','nm','eno','tclesmesdes',
		'jtcnslemede','jean','ge','eoi','ex','ef','reqquechose','entre',
		'except_evr',
		'2consonnes','abbaye','que_gue_final','e_muet','e_deb',
		'@','ier_Conj','*'],
		{
        'conj_v_ier':[this.Regle_ient,'_muet',3], // verbe du 1er groupe terminé par 'ier' conjugué à la 3ème pers du pluriel
        'uient':[{'-':/ui/i,'+':/nt$/i},'_muet',3], // enfuient, appuient, fuient, ennuient, essuient
        'ien_0':[{'-':/(fic|n|quot)i/i,'+':/nt(s?)$/i},'a_tilda',2], // incovénient, coefficient,...
		'scien':[{'-':/((aud|sc|cl|^fa|([éf]fic)|pat|émoll|expé[dr]|^farn)[iï])/i,'+':/n/i},'a_tilda',2], // science, faïence...
		'orient':[{'-':/ori/i,'+':/nt/i},'a_tilda',2],
        'ien':[{'-':/([bcdégklmnrstvhz]i|ï)/i,'+':/n([bcçdfghjklpqrstvwxz]|(s?)$)/i},'e_tilda',2], // certains mots avec 'ien' => son [e_tilda]
		'ien2':[{'-':/pi/i,'+':/n(s?)$/i},'e_tilda',2], // carpien, olympien, ...
		'examen':[{'-':/(exam|mino|édu|apexi|^api|loqui|\wy|é|^b(r?))/i,'+':/n(s?)$/i},'e_tilda',2],
		'zen':[{'-':/([a-z]m|gold|poll|^[yz]|^av|bigoud|coh|^éd)/i,'+':/n(s?)$/i},'E_comp',1],

        //'zen':[{'-':/(abdom|dolm|gentlem|gold|poll|spécim|^z|^y|acum|album|^a(m|v)|lum|bigoud|cérum|coh|^culm|^cyclam|dictam|^éd)/i,
		//			'+':/n(s?)$/i},'E_comp',1], // pas sûr que gentlemen ait un sens ici, qui l'utilise en français?

		//'except_en':[{'-':/(exam|mino|édu|apexi|^api|loqui|y|é|^b(r?))/i,'+':/n(s?)$/i},'e_tilda',2], // exceptions des mots où le 'en' final se prononce [e_tilda] (héritage latin)
        '_ent':[this.Regle_mots_ent,'a_tilda',2], // quelques mots (adverbes ou noms) terminés par ent
        'adv_emment_fin':[{'-':/emm/i,'+':/nt/i},'a_tilda',2], // adverbe avec 'emment' => se termine par le son [a_tilda]
        'ment':[this.Regle_ment,'a_tilda',2], // on considère que les mots terminés par 'ment' se prononcent [a_tilda] sauf s'il s'agit d'un verbe
		'imparfait':[{'-':/ai/i,'+':/nt$/i},'verb_3p',3], // imparfait à la 3ème personne du pluriel
		'verbe_3_pluriel':[{'+':/nt$/i},'q_caduc',1], // normalement, pratiquement tout le temps verbe à la 3eme personne du pluriel
		'hier':[this.Regle_er,'E_comp',1], // encore des exceptions avec les mots terminés par 'er' prononcés 'E R'        
		'au':[{'+':/au/i},'o_comp',3],
        'avoir':[this.Regle_avoir,'y',2],
		'eu':[{'+':/(u|û)/i},'x2',2],
        'in':[{'+':/i[nm]([bcçdfghjklnmpqrstvwxz]|$)/i},'e_tilda',3], // toute succession 'ein' 'eim' suivie d'une consonne ou d'une fin de mot
        'orgueil':[{'-':/gu/i,'+':/il/i},'x2',1], // enorgueilli
		'eil':[{'+':/il/i},'E_comp',1],
		'y':[{'+':/y[aeiouéèêààäôâ]/i},'E_comp',1],
 		'iy':[{'+':/[iy]/i},'E_comp',2],
        // 'ennemi':[{'-':/^/i,'+':/nnemi/i},'E_comp',1], // ennemi est l'exception ou 'enn' en début de mot se prononce 'èn' (cf. enn_debut_mot)
        'enn_debut_mot':[{'-':/(^|dés)/i,'+':/nn[^ié]/i},'a_tilda',2], // 'enn' en début de mot se prononce 'en'
		't_final':[{'+':/[t]$/i},'E_comp',2], // donne le son [E] et le t ne se prononce pas
		'eclm_final':[{'+':/[clm](s?)$/i},'E_comp',1], // donne le son [E] et le l ou le c se prononcent (ex. : miel, sec)
 		'd_except': [{'-':/(^bl|^ou|^damn)/i, '+':/d(s?)$/i},'E_comp',1], // [PAE 22.02.20] pour covrir oued, bled, damned   
        'drz_final':[{'+':/[drz](s?)$/i},'e_comp',2], // e suivi d'un d,r ou z en fin de mot done le son [e] 
        'except_en2':[this.RegleMotsEn5,'e_tilda',2], // mots dont le en se prononce [5]
        'n':[{'+':/n[bcdfghjklmpqrstvwxzç]/i},'a_tilda',2],
        'adv_emment_a':[{'+':/mment/i},'a',1], // adverbe avec 'emment' => son [a]
		'lemme':[{'-':/([ltg]|^p|^syn|^systr)/i,'+':/mm/i},'E_comp',1], // lemme et ses dérivés => son [E]
		'em_gene':[{'+':/m[bcçdfghjklmpqrstvwxz]/i},'a_tilda',2], // 'em' cas général => son [a_tilda]
		'nm':[{'+':/[nm]$/i},'a_tilda',2], // en fin de mot...
		'eno':[{'-':/(^|dés)/i,'+':/n[aio]/i},'a_tilda',1], // 'enivrer' --> le 'n' se prononce également
        'tclesmesdes':[{'-':/^[tcslmd]/i,'+':/s$/i},'e_comp', 2], // mes, tes, ces, ses, les
		'que_gue_final':[{'-':/[gq]u/i,'+':/(s?)$/i},'q_caduc',1], // que ou gue final
		'jtcnslemede':[{'-':/^[jtcnslmd]/i,'+':/$/i},'q',1], // je, te, me, le, se, de, ne
		'jean':[{'-':/j/i,'+':/an/i},'_muet',1], // jean
		'ge':[{'-':/g/i,'+':/[aouàäôâ]/i},'_muet',1], // un e précédé d'un 'g' et suivi d'une voyelle ex. : cageot
		'eoi':[{'+':/oi/i},'_muet',1], // un e suivi de 'oi' ex. : asseoir
		'ex':[{'+':/x/i},'E_comp',1], // e suivi d'un x se prononce è
		'ef':[{'+':/[bf](s?)$/i},'E_comp',1], // e suivi d'un f ou d'un b en fin de mot se prononce è
		'reqquechose':[this.RegleMotsRe,'q',1], // re-quelque chose : le e se prononce 'e'
		'entre':[{'-':/(^((ré)?)entr|^contr|^autor|^maugr)/i},'q',1],
        'except_evr':[{'+':/vr/i},'q',1], // chevrier, chevron, chevreuil
		'2consonnes':[{'+':/[bcçdfghjklmnpqrstvwxz]{2}/i},'E_comp',1], // e suivi de 2 consonnes se prononce è
        'abbaye':[{'-':/abbay/i,'+':/(s?)$/i},'_muet',1], // ben oui...
		'e_muet':[{'-':/[aeiouéèêà]/i,'+':/(s?)$/i},'_muet',1], // un e suivi éventuellement d'un 's' et précédé d'une voyelle ou d'un 'g' ex. : pie, geai
		'e_deb':[{'-':/^/i},'q',1], // par défaut, un 'e' en début de mot se prononce [q]
        '@':[{'+':/(s?)$/i},'q_caduc',1],
		'ier_Conj':[this.Regle_ierConjE,'_muet',1], // verbes en ier conjugués au futur ou au conditionnel
		'*':[{},'q',1],
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
'g' : [['sugg','g','ao','eiy','aiguille','u_consonne','ngui','u','except_n','n','vingt','g_muet_oin',
		'g_muet_our','g_muet_an',//'g_muet_fin', 
		'*'],
		{'sugg':[{'-':/su/i,'+':/g(e|é)/i},'g',1], // suggérer et sa famille
		'g':[{'+':/g/i},'g',2],
		'ao':[{'+':/(a|o)/i},'g',1],
		'eiy':[{'+':/[eéèêëïiîy]/i},'Z',1], // un 'g' suivi de e,i,y se prononce [Z]
		'aiguille':[{'-':/ai/i,'+':/(u(ill|iér|ï|ité|(s?)$))/i},'g',1], // encore une exception : aiguille, aigu et quelques mots bizarres comme aiguité
		'u_consonne':[{'+':/u[bcçdfghjklmnpqrstvwxz]/i},'g',1], // gu suivi d'une consonne se prononce [g][y]
		'ngui':[{'-':/n/i,'+':/ui(st|sm|fè|cu)/i},'g',1], // linguiste, inguinal, unguifère, onguiculé...
		'u':[{'+':/u/i},'g_u',2],
		'except_n':[this.RegleMotsGnGN,'g',1],
		'n':[{'+':/n/i},'N',2],
		'vingt':[{'-':/vin/i,'+':/t/i},'_muet',1], // vingt
		'g_muet_oin':[{'-':/oi(n?)/i},'_muet',1], // un 'g' précédé de 'oin' ou de 'oi' ne se prononce pas ; ex. : poing, doigt
		'g_muet_our':[{'-':/ou(r)/i},'_muet',1], // un 'g' précédé de 'our' ou de 'ou(' ne se prononce pas ; ex. : bourg
		'g_muet_an':[{'-':/((s|^ét|^r|^harf|^il)an|lon|haren|ein)/i,'+':/(s?)$/i},'_muet',1], // sang, rang, étang, long, hareng
		//'g_muet_fin':[{'-':/(lon|haren)/i,'+':/(s?)$/i},'_muet',1], // pour traiter les exceptions : long, hareng
		'*':[{},'g',1]}],
'h' : [['*'],
		{'*':[{},'_muet',1]}],
'i' : [['ing','inh','n','m','nm','prec_2cons','lldeb','vill','mill2','tranquille',
		'ill','except_ill','bacille','ill_Ceras', '@ill','@il','ll','@il_Ceras',
		'll_Ceras','ui','ient_1','ient_2','ie','ier_Conj','i_voyelle','flirt','*'],
		{'ing':[{'-':/[bcçdfghjklmnpqrstvwxz]/i,'+':/ng(s?)$/i},'i',1],
		'inh':[{'-':/^/i,'+':/nh/i},'i',1],
		'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
		'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
		'nm':[{'+':/[n|m]$/i},'e_tilda',2],
		'prec_2cons':[{'-':/[ptkcbdgfv][lr]/i, '+':/[aäâeéèêëoôöuù]/i},'i_j',1], // précédé de 2 consonnes (en position 3), doit apparaître comme [ij] [PAE 20.02.20: rajouté les voyelles]
		'lldeb':[{'-':/^/i,'+':/ll/i},'i',1],
		'vill':[{'-':/v/i,'+':/ll/i},'i',1,IllCeras],
		'mill2':[{'-':/^m/i,'+':/ll[^(et)]/i},'i',1,IllCeras],
		'tranquille' : [{'-':/(ach|tranqu)/i,'+':/ll/i},'i',1,IllCeras],
		'ill':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i',1,IllLireCouleur], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
		'except_ill':[this.Regle_ill,'i',1], // PAE - 07.05.20
		'bacille':[{'-':/(bac|dist|inst)/i,'+':/ll/i},'i',1], // il y tant de mots contenant 'bacill'... et les verbes...
		'ill_Ceras':[{'+':/ll/i,'-':/[bcçdfghjklmnpqrstvwxz](u?)/i},'i_j_ill',3,IllCeras], // précédé éventuellement d'un u et d'une consonne, donne le son [i]
		'@ill':[{'-':/[aeoœ]/i,'+':/ll/i},'j',3,IllLireCouleur], // par défaut précédé d'une voyelle et suivi de 'll' donne le son [j]
		'@il':[{'-':/[aeouœ]/i,'+':/l(s?)$/i},'j',2,IllLireCouleur], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [j]
		'll':[{'+':/ll/i},'j',3, IllLireCouleur], // par défaut avec ll donne le son [j]
		'@il_Ceras':[{'-':/[aeouœ]/i,'+':/l(s?)$/i},'j_ill',2, IllCeras], // par défaut précédé d'une voyelle et suivi de 'l' donne le son [ill]
		'll_Ceras':[{'+':/ll/i},'j_ill',3, IllCeras], // par défaut avec ll donne le son [ill]
		'ui':[{'-':/u/i,'+':/ent/i},'i',1], // essuient, appuient
		'ient_1':[this.Regle_ient,'i',1], // règle spécifique pour différencier les verbes du premier groupe 3ème pers pluriel
		'ient_2':[{'+':/ent(s?)$/i},'j',1], // si la règle précédente ne fonctionne pas
		'ie':[{'+':/e(s?)$/i},'i',1], // mots terminés par -ie(s)
		'ier_Conj':[this.Regle_ierConjI,'i',1], // verbes en ier conjugués au futur ou au conditionnel
		'i_voyelle':[{'+':/[aäâeéèêëoôöuù]/i},'ji',1], // i suivi d'une voyelle donne [j]
		'flirt':[{'-':/^fl/i,'+':/rt/i},'x2',1],
		'*':[{},'i',1]}],
'ï' : [['thai', 'aie', 'n','m','nm','*'],
		{'thai':[{'-':/t(h?)a/i},'j',1], // taï, thaï et dérivés
		'aie':[{'-':/[ao]/i,'+':/e/i},'j',1], // païen et autres
		'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
		'm':[{'+':/m[bcçdfghjklnpqrstvwxz]/i},'e_tilda',2],
		'nm':[{'+':/[n|m]$/i},'e_tilda',2],
		'*':[{},'i',1]}],
'î' : [['n','*'],
		{'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
		'*':[{},'i',1]}],
'j' : [['*'],
		{'*':[{},'Z',1]}],
'k' : [['*'],
		{'*':[{},'k',1]}],
'l' : [['vill','mill','tranquille','illdeb','except_ill_l','bacille','ill','eil','ll','excep_il', 
		// 'lisole', 
		'*'],
		{'vill':[{'-':/(^v|vaudev|banv|^ov|bougainv|interv|cav)i/i,'+':/l/i},'l',2], // ville, village etc. => son [l]
		'mill':[{'-':/mi/i,'+':/l[^(et)]/i},'l',2], // mille, million, etc. => son [l] mais pas 'millet'
		'tranquille':[{'-':/(achi|tranqui)/i,'+':/l/i},'l',2], // tranquille => son [l]
		'illdeb':[{'-':/^i/i,'+':/l/i},'l',2], // 'ill' en début de mot = son [l] ; exemple : illustration
		'except_ill_l':[this.Regle_ill,'l',2],
		'bacille':[{'-':/(baci|disti|insti)/i,'+':/l/i},'l',2],
		'ill':[{'-':/.i/i,'+':/l/i},'j',2], // par défaut, 'ill' donne le son [j]
		'eil':[{'-':/e(u?)i/i},'j',1], // les mots terminés en 'eil' ou 'ueil' => son [j]
		'll':[{'+':/l/i},'l',2], // à défaut de l'application d'une autre règle, 'll' donne le son [l]
		'excep_il':[{'-':/(fusi|outi|genti|sourci|persi)/i,'+':/(s?)$/i},'_muet',1], // les exceptions trouvées où le 'l' à la fin ne se prononce pas : fusil, gentil, outil
		//'lisole':[{'+':/$/i,'-':/^/i},'l',1], // exemple : l'animal
		'*':[{},'l',1]}],
'm' : [['m','tomn','damn','*'],
		{'m':[{'+':/m/i},'m',2],
		'damn':[{'-':/da/i,'+':/n/i},'_muet',1], // Regle spécifique pour 'damné' et ses dérivés
		'tomn':[{'-':/to/i,'+':/n/i},'_muet',1], // Regle spécifique pour 'automne' et ses dérivés
		'*':[{},'m',1],
		}],
'n' : [['n','ent','ing','*'],
		{'n':[{'+':/n/i},'n',2],
		'ent':[{'-':/e/i,'+':/t$/i},'verb_3p',2],
		'ing':[{'-':/i/i,'+':/g(s?)$/i},'J',2],
		'*':[{},'n',1],
		}],
'o' : [['in','except_y','i','tomn','faonner',
		'n','m','nm','u','boo','zoom','alcool','oeu_defaut','oe_0','oe_2', 'oe_3',
		'oe_4','oe_defaut','toast','*'],
		{'in':[{'+':/i[nm]([bcçdfghjklpqrstvwxz]|$)/i},'w_e_tilda',3],
		'except_y':[this.RegleMotsOYoj,'o',1],
		'i':[{'+':/(i|î|y)/i},'oi',2], // [PAE 26.02.20] introduction du phonème oi pour pouvoir le marquer dans la convention CERAS
		'tomn':[{'-':/t/i,'+':/mn/i},'o',1], // Regle spécifique pour 'automne' et ses dérivés
		'faonner':[{'-':/^fa/i,'+':/nn/i},'_muet',1],
		'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'o_tilda',2],
		'm':[{'+':/m[bcçdfgjkpqrstvwxz]/i},'o_tilda',2], // toute consonne sauf le l et le m
		'nm':[{'+':/[nm]$/i},'o_tilda',2],
		'u':[{'+':/[uwûù]/i},'u',2], // son [u] : clou, clown
		'boo':[{'-':/(al|b|bl|baz|f|gl|gr|lm|pr|^r|sc|sh|sl|w)/i,'+':/o/i},'u',2], // exemple : booléen, boom, sloop, ...
		'zoom':[{'-':/z/i,'+':/om/i},'u',2],
		'alcool':[{'-':/(alc|hyper|waterl|witl)/i,'+':/o/i},'o',2],
		'oeu_defaut':[{'+':/eu/i},'x2',3], // exemple : oeuf
		'oe_0':[{'+':/ê/i},'oi',2],  // exemple : poêle [PAE 26.02.2020] remplacé par 'oi'
		'oe_2':[{'-':/m/i,'+':/e/i},'oi',2], // exemple : moelle [PAE 26.02.2020] remplacé par 'oi'
		'oe_3':[{'-':/f/i,'+':/e/i},'e',2], // exemple : foetus
		'oe_4':[{'-':/(gastr|électr|inc|min|c|aér|angi|benz)/i,'+':/e/i},'o',1], // [PAE 26.02.2020] électroencéphalogramme, minoen, coefficient
		'oe_defaut':[{'+':/e/i},'x2',2], // exemple : oeil
		'toast':[{'-':/t/i,'+':/ast/i},'o',2], // toast, toaster et toutes ses formes
		'*':[{},'o',1]
		}],
'œ' : [['oeu','coe','oe_e','*'],
		{'oeu':[{'+':/u/i},'x2',2], // voeux, ... [PAE 29.02.20]
		'coe':[{'-':/c/i,'+':/n/i},'e',1],
		'oe_e':[{'+':/(l|c|b|t)/i},'e',1],
		'*':[{},'x2',1]
		}],
'ô' : [['*'],
		{'*':[{},'o',1]
		}],
'ö' : [['*'],
		{'*':[{},'o',1]
		}],
'p' : [['p','h','oup','sculpt','*'],
		{'p':[{'+':/p/i},'p',2],
		'h':[{'+':/h/i},'f_ph',2],
		'oup':[{'-':/([cl]ou|dra|[ti]ro|alo|[rm])/i,'+':/(s?)$/i},'_muet',1], // les exceptions avec un p muet en fin de mot : loup, coup, galop, sirop
		'sculpt':[{'-':/(scul|ba|com|corrom)/i,'+':/t/i},'_muet',1], // les exceptions avec un p muet : sculpter, baptême, compter et les mots de la même famille
		'*':[{},'p',1]}],
'q' : [['qua_w','qu','k', '*'],
		{'qua_w':[this.RegleMotsQUkw,'k',1], 
		'qu':[{'+':/u[bcçdfgjklmnpqrstvwxz]/i},'k',1],
		'k':[{'+':/u/i},'k_qu',2],
		'*':[{},'k',1]}],
'r' : [[
		//'monsieur','messieurs','gars',
		'r', '*'],
		{
		//'monsieur':[{'-':/monsieu/i},'_muet',1],
		//'messieurs':[{'-':/messieu/i},'_muet',1],

		'r':[{'+':/r/i},'R',2],
		//'gars':[{'+':/s/i,'-':/ga/i},'_muet',2], // gars
		'*':[{},'R',1]}],
's' : [['schizo','sch','transs','s','s_final','@','parasit','balsa','subside','asept','pasZ','pasZ2','déss','z','dész','h','fasci',
		'*'],
		{'schizo':[{'+':/(chi[aoz]|chato)/i},'s',1],
		'sch':[{'+':/ch/i},'S',3], // schlem
		'transs':[{'-':/tran/i, '+':/s/i},'s',1],
		's':[{'+':/s/i},'s',2], // un s suivi d'un autre s se prononce [s]
		's_final':[this.Regle_s_final,'s',1], // quelques mots terminés par -us, -is, -os, -as, -es
		'@':[{'+':/$/i},'_muet',1],
		'parasit':[{'-':/para/i,'+':/it/i},'z_s',1], // parasit*
		'balsa':[{'-':/(tran|bal)/i,'+':/(i|hum|a)/i},'z_s',1], // transhumance, transit, balsa,...
		'subside':[{'-':/sub/i,'+':/i/i},'z_s',1], // subsidiaire
		'asept':[{'-':/a/i,'+':/(ep(s|t)i|ex|ocia|y(m|n|s))/i},'s',1],
		'pasZ':[{'-':/
			(^para|^contre|^mono|^vrai|^vivi|^uni|^ultra|^alcoo|^antidy|^anti|^auto|batracho|^bio|^su|^carbo|^chéno|^ortho|^déca|^co|^soubre|^crypto|^cupro|^cyno|^deuto|^dodéca|^écho|(^[ée]qui))
			/i},'s',1],
		'pasZ2':[{'-':/
			(^énnéa|^entre)
			/i},'s',1],		
		'déss':[{'-':/^dé/i,'+':/(acra|ensibi|olida)/i},'s',1], // désacraliser
		'z':[{'-':/[aeiyouéèàâüûùëöêîôïœ]/i,'+':/[aeiyouéèàâüûùëöêîôïœ]/i},'z_s',1], // un s entre 2 voyelles se prononce [z]
		'dész':[{'-':/(^dé|^di|^dy|^e|^phy|^tran)/i,'+':/[aiyouéèàâüûùëöêîôïh]/i},'z_s',1], // déshonneur, esherbeur (si si), transhumance...
		'h':[{'+':/h/i},'S',2],
		'fasci':[{'-':/fa/i,'+':/cis/i},'S',2], // fasciste
		'*':[{},'s',1]}],
't' : [['t_deb','t','tisole','except_tien','_tien','ex_tiot','verb_tions','ex_tie','tie','tiaot',
		'tiaos','vingt',
		'ourt','_inct','_spect','_ct','_est','t_final','tmuet',
		'ex_tiel','_tiel','courtci','@','*'],
		{'t_deb':[{'-':/^/i},'t',1],
		't':[{'+':/t/i},'t',2],
		'tisole':[{'+':/$/i,'-':/^/i},'t',1], // exemple : demande-t-il
		'except_tien':[this.Regle_tien,'t',1], // quelques mots où 'tien' se prononce [t]
		'_tien':[{'+':/ien/i},'s_t',1],
		'ex_tie':[{'-':/minu/i,'+':/ie(r|z)/i},'t',1],
		'tie':[{'-':/(ambi|albu|cra|lvi|[^r]essen|idio|iner|ini|minu|ipé|oten|phé|oba|iaba|argu|automa|balbu|^cani|cap|tan|conten|dévo|féren|ploma|facé|^fac)/i,
			'+':/i(e|é|èr)/i},'s_t',1],
		'ex_tiot':[{'-':/(cré|plé|jé|([^r]|^)essen|^dui)/i,'+':/i[ao]/i},'s_t',1],
		'tiaot':[{'-':/([eéèêës]|[sc]en|(^|h|n)an|f(l?)[uû]|ar|(ch|^str|galim|fum)[aâ]|rb[io]|^ca|^tri)/i,'+':/i[aâou]/i},'t',1],
		'verb_tions':[this.Regle_VerbesTer,'t',1], // verbes en ter à l'imparfait - nous
		'tiaos':[{'+':/i[aâou]/i},'s_t',1],
		'vingt':[{'-':/ving/i,'+':/$/i},'t',1], // vingt mais pas vingts
		'ourt':[{'-':/(a|h|g)our/i,'+':/$/i},'t',1], // exemple : yaourt, yoghourt, yogourt
		'_inct':[{'-':/inc/i,'+':/(s?)$/i},'_muet',1], // instinct, succinct, distinct
		'_spect':[{'-':/spec/i,'+':/(s?)$/i},'_muet',1], // respect, suspect, aspect
		'_ct':[{'-':/c/i,'+':/(s?)$/i},'t',1], // tous les autres mots terminés par 'ct'
		'_est':[{'-':/es/i,'+':/(s?)$/i},'t',1], // test, ouest, brest, west, zest, lest
		't_final':[this.Regle_t_final,'t',1], // quelques mots où le ´t´ final se prononce
		'tmuet':[{'+':/(s?)$/i},'_muet',1], // un t suivi éventuellement d'un s ex. : marrants
		'ex_tiel':[{'-':/céles/i},'t',1],
		'_tiel':[{'+':/iel((le)?)(s?)/i},'s_t',1],
		'courtci':[{'-':/^cour/i,'+':/circ/i},'_muet',1], // une règle pour courtcircuiter...
		'*':[{},'t',1],							
		'@':[{'+':/$/i},'_muet',1]}],
'u' : [['um','circum','n_on','n','nm','ueil','trust','bluff','qua_w','umb','*'],
		{'um':[this.Regle_MotsUM,'o',1],
		'circum':[{'-':/(circ|^cent)/i,'+':/m/i},'o',1],
		'n_on':[this.Regle_MotsUN_ON,'o_tilda',2],
		'n':[{'+':/n[bcçdfgjklmpqrstvwxz]/i},'x_tilda',2],
		'nm':[{'+':/[nm]$/i},'x_tilda',2],
		'ueil':[{'+':/eil/i},'x2',2], // mots terminés en 'ueil' => son [x2]
		'trust':[{'-':/tr/i,'+':/st/i},'x2',1],
		'bluff':[{'-':/bl/i,'+':/ff/i},'x2',1],
		'qua_w':[this.RegleMotsQUkw,'w',1],
		'umb':[{'-':/(l|rh|^)/i,'+':/mb([aio]|ra|(s?)$)/i},'o_tilda',2],
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
		{'wurst':[{'+':/((u|ü)r|ag(o|n|uin)|rr|lk|isi|e(stp|rn|l(t|che)|i)|arrant|yando|orm|olfram|ill(é|e)|alky)/i},'v',1], 
		// [PAE 23.02.20] modifié pour couvrir tous les cas de Lexique. Une règle complexe vaut-elle mieux que cinq simples?????
		'*':[{},'w',1]}], 
'x' : [['six_dix','dixième','gz_1','gz_2','gz_3','gz_4','gz_5','_aeox','fix','xisole','x_final', '@', '*'],
		{'six_dix':[{'-':/(s|d)i/i,'+':/$/i},'s_x',1],
		'dixième':[{'-':/(s|d)i/i,'+':/iè/i},'z',1],
		'gz_1':[{'-':/^/i,'+':/[aeuéèàüëêûù]/i},'gz',1], // mots qui commencent par un x suivi d'une voyelle (sauf 'i' ou 'o')
		'gz_2':[{'-':/^(h?)e/i,'+':/(h?)[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'ex' ou 'hex' suivi d'une voyelle
		'gz_3':[{'-':/^coe/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'coex' suivi d'une voyelle
		'gz_4':[{'-':/^ine/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'inex' suivi d'une voyelle
		'gz_5':[{'-':/^(p?)rée/i,'+':/[aeiouéèàüëöêîôûù]/i},'gz',1], // mots qui commencent par un 'réex' ou 'préex' suivi d'une voyelle
		'_aeox':[{'-':/[aeo]/i},'ks',1],
		'fix':[{'-':/fi/i},'ks',1],
		'xisole':[{'-':/^/i,'+':/$/i},'ks',1],
		'x_final':[this.Regle_X_Final,'ks',1],
		'*':[{},'ks',1],
		'@':[{'+':/$/i},'_muet',1]}],
'y' : [['m','n','nm','abbaye','y_voyelle', '*'],
		{'m':[{'+':/m[mpb]/i},'e_tilda',2],
		'n':[{'+':/n[bcçdfghjklmpqrstvwxz]/i},'e_tilda',2],
		'nm':[{'+':/[n|m]$/i},'e_tilda',2],
		'abbaye':[{'-':/abba/i,'+':/e/i},'i', 1], // abbaye... bien irrégulier
		'y_voyelle':[{'+':/[aâeiouéèàüëöêîôûù]/i},'j',1], // y suivi d'une voyelle donne [j]
		'*':[{},'i',1]}],
'z' : [['riz', 'aio_z','razzia','zsch','tz','zisole','@', '*'],
		{'riz':[{'-':/^r(i|a)/i,'+':/$/i},'_muet',1], 
		'aio_z':[{'-':/(a|i|o)/i,'+':/$/i},'z',1],
		//'gaz':[{'-':/a/i,'+':/$/i},'z',1],
		'razzia':[{'+':/z/i},'d',1],
		'zsch':[{'+':/sch/i},'S',4], // nietzschéen...
		'tz':[{'-':/t/i},'s',1],
		'zisole':[{'-':/^/i,'+':/$/i},'z',1],
		'@':[{'+':/$/i},'_muet',1],
		'*':[{},'z',1]}],
'æ' : [['*'],
		{'*':[{},'e',1]}], // les autres cas sont traités dans les exceptions. [ae] n'est cependant pas possible...
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
		{'*':[{},'chiffre',1]}], // 20.11.2020 mis à 'chiffre' plutôt que '_muet'. ça se discute...
'’' : [['*'],
		{'*':[{},'chiffre',1]}], // 20.11.2020 mis à 'chiffre' plutôt que '_muet'. ça se discute...
'*' : [['*'],
		{'*':[{},'chiffre',1]}]  // 20.11.2020 mis à 'chiffre' plutôt que '_muet'. ça se discute...

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
		/// Cherche les phonèmes dans <c>pw</c> et complète <c>pw</c> pour qu'il contienne l'information.
		/// </summary>
		/// <param name="pw">Le <see cref="PhonWord"/> à analyser et à compléter avec ses phonèmes.</param>
		/// <param name="conf">La <see cref="Config"/> à utiliser au cours de cette analyse.</param>
		public void FindPhons(PhonWord pw, Config conf)
		{
			logger.ConditionalTrace("FindPhons");
			Debug.Assert(pw != null);

			if (!AutomDictionary.FindPhons(pw, conf))
			{
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
			} // if
		} // FindPhons

		public void CountPhons(ref int[] usedPhons)
			// counts the number of times each phoneme is used in the automat
		{
			foreach (KeyValuePair<char, AutomLetter> k in automLetters)
				k.Value.CountPhons(ref usedPhons);
		}
	}
}
