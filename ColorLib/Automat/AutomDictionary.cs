﻿/********************************************************************************
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

using ColorLib.Morphalou;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ColorLib
{

    /// <summary>
    /// Gestion du dictionnaire des exceptions.
    /// </summary>
    public static class AutomDictionary
    {
        /// <summary>
        /// Cherche si les phonèmes de <paramref name="pw"/> peuvent être trouvés grâche aux
        /// exceptions. Si oui, complète <paramref name="pw"/> et retourne <c>true</c>, sinon 
        /// retourne <c>false</c>
        /// </summary>
        /// <param name="pw">Le <see cref="PhonWord"/> à analyser et à compléter avec ses phonèmes.</param>
        /// <param name="conf">La <see cref="Config"/> à utiliser au cours de cette analyse.</param>
        /// <returns><c>true</c>: le mot a été trouvé dans les exceptions, les phonèmes ont été ajoutés à 
        /// <paramref name="pw"/>. <c>false</c>: le mot n'est pas dans les listes. <paramref name="pw"/> 
        /// est inchangé.</returns>
        public static bool FindPhons(PhonWord pw, Config conf)
        {
            bool toReturn = false;
            string sons;
            if (except.TryGetValue(pw.GetWord(), out sons)) 
            {
                toReturn = true;
                int l = 0; // indice dans le mot
                int i = 0; // indice dans son.
                while (i < sons.Length)
                {
                    int j = i;
                    while (j < sons.Length
                        && sons[j] != '-'
                        && sons[j] != ';')
                    {
                        j++;
                    }
                    int len = j - i; // j doit être > i, donc len > 0;
                    string lettres = sons.Substring(i, len);
                    lettres = lettres.Trim();
                    len = lettres.Length;
                    char son;
                    if (j < sons.Length && sons[j] == '-')
                    {
                        i = j + 1;
                        while (j < sons.Length && sons[j] != ';')
                        {
                            j++;
                        }
                        string strSon = sons.Substring(i, j - i);
                        strSon = strSon.Trim();
                        son = strSon[0]; // par définition un seul charactère.
                        PhonInW piw = new PhonInW(pw, l, l + len - 1, son, "exception");
                        l += len;
                    }
                    else
                    {
                        for (int k = 0; k < len; k++)
                        {
                            son = lettres[k];
                            PhonInW piw = new PhonInW(pw, l, l, son, "exception");
                            l++;
                        }
                    }
                    i = j + 1;
                }
                Debug.Assert(l == pw.GetWord().Length, string.Format(ConfigBase.cultF,
                    "AutomDictionary.FindPhons: le nombre de lettres du mot n'est pas consistant. {0} {1}", pw.GetWord(), l));
            }
            return toReturn;
        }

        /// <summary>
        /// Le dictionnaire contient d'un côté le mot comme clé, de l'autre la représentation
        /// en chaîne de caractères de sa forme en phonèmes.
        /// Le deuxième string (le contenu) a le format {lettre {lettre} "-" son ";"}. L'ensemble
        /// des lettres doit être équivalent au mot, cad à la clé. 
        /// son est la représentation en code colorization du son, où un symbole rerpésente un son.
        /// </summary>
        /// <remarks>
        /// J'ai parfois volontairement remplacé des règles dans <see cref="AutomAutomat"/> par une
        /// entrée dans le dico suivant l'argument qu'il est dommage de vérifier chaque fois qu'il y
        /// une lettre 'e' dans un texte, s'il s'agit du mot "et". Un autre example concerne
        /// la série de mots autour de "shampooing". Une règle aurait très bien intercepté toute
        /// la famille. Néanmoins je trouvais dommage de tester chaque occurence de la lettre 'o'
        /// pour vérifier s'il ne s'agissait pas de "shampoo" quelquechose...
        /// Cette approche n'est pas du tout systématique mais dépend plus de l'humeur du moment.
        /// Objectivement, un dico va beaucoup plus vite qu'une série de règles, de l'autre côté,
        /// à part pour un texte vraiment très long comme dans les tests de non régression où près
        /// de 400 000 mots sont testés, le temps pris par l'automate est négligeable par rapport
        ///  au temps de colorisation du texte. C'est donc un peu (beaucoup) du pinaillage :-). 
        /// </remarks>
        private static Dictionary<string, string> except = new Dictionary<string, string>()
        {
            // les mots suivants proviennent de la base de données Morphalou.
            // Je me demande s'ils existent vraiment tous dans notre belle langue...
            { "aber", "a-a;b-b;e-E;r-R" },
            { "abers", "a-a;b-b;e-E;r-R;s-#" },
            { "abies", "a-a;b-b;i-j;e-E;s-s" },
            { "abigaïl", "a-a;b-b;i-i;g-g;a-a;ï-j;l-l" },
            { "abigaïls", "a-a;b-b;i-i;g-g;a-a;ï-j;l-l;s-#" },
            { "abrasax", "ab;r-R;asax" },
            { "acanthophœnix", "a;c-k;an-@;t;h-#;o;ph-f;œ-e;ni;x" },
            { "trachélobranches", "t;r-R;a;ch-k;é-e;lob;r-R;an-@;ch-S;e-°;s-#" },
            { "trachélobranche", "t;r-R;a;ch-k;é-e;lob;r-R;an-@;ch-S;e-°" },
            { "schnorchel", "sch-S;no;r-R;ch-k;e-E;l" },
            { "schnorchels", "sch-S;no;r-R;ch-k;e-E;l;s-#" },
            { "cholédochoclyse", "ch-k;ol;é-e;do;ch-S;o;c-k;l;y-i;s-z;e-°" },
            { "cholédochoclyses", "ch-k;ol;é-e;do;ch-S;o;c-k;l;y-i;s-z;e-°;s-#" },
            { "chénomychon", "cg-k;é-e;nom;y-i;ch-S;on-§" },
            { "chénomychons", "cg-k;é-e;nom;y-i;ch-S;on-§;s-#" },
            { "accelerando", "a;cc-x;e-e;l;e-e;r-R;an-@;do" },
            { "ace", "a-E;c-s;e-ç" },
            { "aces", "a-E;c-s;e-ç;s-#" },
            { "actaea", "a-a;c-k;t-t;ae-e;a-a" },
            { "actaeas", "a-a;c-k;t-t;ae-e;a-a;s-#" },
            { "agnus", "a-a;gn-N;u-u;s-s" },
            { "aérosol", "a-a;é-e;r-R;o-o;s-s;o-o;l-l" },
            { "aérosols", "a-a;é-e;r-R;o-o;s-s;o-o;l-l;s-#" },
            { "clef", "c-k;l;ef-e" }, // évite un règle pour un seul mot
            { "clefs", "c-k;l;ef-e;s-#" },
            { "et", "et-e" }, // évite un règle pour un seul mot
            { "est", "est-E" }, // évite un règle pour un seul mot
            { "que", "qu-k;e-°" }, // évite un règle pour un seul mot
            { "mars", "m-m;a-a;r-R;s-s" },
            { "encoigniez", "en-@;c-k;oi-o;gn-N;i-j;ez-e" },
            { "opisthocoelien", "o-o;p-p;i-i;s-s;t-t;h-#;o-o;c-s;oe-2;l-l;i-i;en-5" },
            { "opisthocoeliens", "o-o;p-p;i-i;s-s;t-t;h-#;o-o;c-s;oe-2;l-l;i-i;en-5;s-#" },
            { "opisthocoelienne", "o-o;p-p;i-i;s-s;t-t;h-#;o-o;c-s;oe-2;l-l;i-i;e-E;n-n;n-n;e-ç" },
            { "opisthocoeliennes", "o-o;p-p;i-i;s-s;t-t;h-#;o-o;c-s;oe-2;l-l;i-i;e-E;n-n;n-n;e-ç;s-#" },
            { "grizzly", "g-g;r-R;i-i;zz-z;l-l;y-i" }, // évite un règle pour un seul mot
            { "grizzlys", "g-g;r-R;i-i;zz-z;l-l;y-i;s-#" }, // évite un règle pour un seul mot
            { "monsieur", "m-m;on-°;s-s;i-j;eu-2;r-#" },
            { "messieurs", "m-m;e-E;ss-s;i-j;eu-2;r-#;s-#" },
            { "gars", "g-g;a-a;rs-#" },
            { "aeschne", "ae-e;s;ch-k;n-n;e-e" },
            { "aeschnes", "ae-e;s;ch-k;n-n;e-e;s-#" },
            { "elaeis", "el;ae-e;is" },
            { "élaeis", "el;ae-e;is" },
            { "maelstrom", "m-m;ae-a;l-l;s-s;t-t;r-R;om" },
            { "maelstroms", "m-m;ae-a;l-l;s-s;t-t;r-R;om;s-#" },
            { "maestoso", "m-m;a-a;e-E;s-s;t-t;o-o;s-z;o-o" },
            { "maestria", "m-m;a-a;e-E;s-s;t-t;r-R;i-/;a-a" },
            { "maestrias", "m-m;a-a;e-E;s-s;t-t;r-R;i-/;a-a;s-#" },
            { "maestro", "m-m;a-a;e-E;s-s;t-t;r-R;o-o" },
            { "maestros", "m-m;a-a;e-E;s-s;t-t;r-R;o-o;s-#" },
            { "paella", "p-p;a-a;e-E;ll-l;a-a" },
            { "paellas", "p-p;a-a;e-E;ll-l;a-a;s-#" },
            { "tael", "t-t;a-a;e-E;l-l" },
            { "taels", "t-t;a-a;e-E;l-l;s-#" },
            { "ægilopinée", "æ-E;g-Z;i-i;l-l;o-o;p-p;i-i;n-n;é-e;e-#" },
            { "ægilopinées", "æ-E;g-Z;i-i;l-l;o-o;p-p;i-i;n-n;é-e;e-#;s-#" },
            { "affettuoso", "a-a;ff-f;e-e;tt-t;u;o-o;s-z;o-o" },
            { "afrikander", "a-a;f-f;r-R;i-i;k-k;an-@;d-d;e-E;r-R" },
            { "afrikanders", "a-a;f-f;r-R;i-i;k-k;an-@;d-d;e-E;r-R;s-#" },
            { "lignosité", "li;gn-N;o;s-z;it;é-e" },
            { "hargnosité", "h-#;a-a;r-R;gn-N;o-o;s-z;i-i;t-t;é-e" },
            { "ageratum", "a-a;g-Z;e;r-R;a-a;t-t;u-o;m-m" },
            { "ageratums", "a-a;g-Z;e;r-R;a-a;t-t;u-o;m-m;s-#" },
            { "lignomètre", "l-l;i-i;gn-N;o-o;m-m;è-E;t-t;r-R;e-ç" },
            { "lignomètres", "l-l;i-i;gn-N;o-o;m-m;è-E;t-t;r-R;e-ç;s-#" },
            { "magnan", "m-m;a-a;gn-N;an-@" }, // Nom propre dans un poème de Victor Hugo
            { "bisexe", "b-b;i-i;s-s;e-E;x-x;e-ç" },
            { "bisexes", "b-b;i-i;s-s;e-E;x-x;e-ç;s-#" },
            { "bisexualité", "b-b;i-i;s-s;e-E;x-x;u-y;a-a;l-l;i-i;t-t;é-e" },
            { "bisexualités", "b-b;i-i;s-s;e-E;x-x;u-y;a-a;l-l;i-i;t-t;é-e;s-#" },
            { "bisexuel", "b-b;i-i;s-s;e-E;x-x;u-y;e-E;l-l" },
            { "bisexuels", "b-b;i-i;s-s;e-E;x-x;u-y;e-E;l-l;s-#" },
            { "bisexuelle", "b-b;i-i;s-s;e-E;x-x;u-y;e-E;ll-l;e-ç" },
            { "bisexuelles", "b-b;i-i;s-s;e-E;x-x;u-y;e-E;ll-l;e-ç;s-#" },
            { "bisexué", "b-b;i-i;s-s;e-E;x-x;u-y;é-e" },
            { "bisexués", "b-b;i-i;s-s;e-E;x-x;u-y;é-e;s-#" },
            { "bisexuée", "b-b;i-i;s-s;e-E;x-x;u-y;é-e;e-#" },
            { "bisexuées", "b-b;i-i;s-s;e-E;x-x;u-y;é-e;e-#;s-#" },
            { "trisoc", "t-t;r-R;i-i;s-s;o-o;c-k" },
            { "trisymptôme", "t-t;r-R;i-i;s-s;ym-5;p-p;t-t;ô-o;m-m;e-ç" },
            { "trisymptômes", "t-t;r-R;i-i;s-s;ym-5;p-p;t-t;ô-o;m-m;e-ç;s-#" },
            { "trisyndrome", "t-t;r-R;i-i;s-s;yn-5;d-d;r-R;o-o;m-m;e-ç" },
            { "trisyndromes", "t-t;r-R;i-i;s-s;yn-5;d-d;r-R;o-o;m-m;e-ç;s-#" },
            { "trisubstitué", "t-t;r-R;i-i;s-s;u-y;b-b;s-s;t-t;i-i;t-t;u-y;é-e" },
            { "trisubstituée", "t-t;r-R;i-i;s-s;u-y;b-b;s-s;t-t;i-i;t-t;u-y;é-e;e-#" },
            { "trisubstituées", "t-t;r-R;i-i;s-s;u-y;b-b;s-s;t-t;i-i;t-t;u-y;é-e;e-#;s-#" },
            { "trisubstitués", "t-t;r-R;i-i;s-s;u-y;b-b;s-s;t-t;i-i;t-t;u-y;é-e;s-#" },
            { "trisulfure", "t-t;r-R;i-i;s-s;u-y;l-l;f-f;u-y;r-R;e-ç" },
            { "trisulfures", "t-t;r-R;i-i;s-s;u-y;l-l;f-f;u-y;r-R;e-ç;s-#" },
            { "trisyllabes", "t-t;r-R;i-i;s-s;y-i;ll-l;a-a;b-b;e-ç;s-#" },
            { "trisyllabe", "t-t;r-R;i-i;s-s;y-i;ll-l;a-a;b-b;e-ç" },
            { "trisecteur", "t-t;r-R;i-i;s-s;e-E;c-k;t-t;eu-2;r-R" },
            { "trisecteurs", "t-t;r-R;i-i;s-s;e-E;c-k;t-t;eu-2;r-R;s-#" },
            { "trisection", "t-t;r-R;i-i;s-s;e-E;c-k;t-s;i-j;on-§" },
            { "trisections", "t-t;r-R;i-i;s-s;e-E;c-k;t-s;i-j;on-§;s-#" },
            { "trisectrice", "t-t;r-R;i-i;s-s;e-E;c-k;t-t;r-R;i-i;c-s;e-ç" },
            { "trisectrices", "t-t;r-R;i-i;s-s;e-E;c-k;t-t;r-R;i-i;c-s;e-ç;s-#" },
            { "trisyllabique", "t-t;r-R;i-i;s-s;y-i;ll-l;a-a;b-b;i-i;qu-k;e-ç" },
            { "trisyllabiques", "t-t;r-R;i-i;s-s;y-i;ll-l;a-a;b-b;i-i;qu-k;e-ç;s-#" },
            { "oignon", "oi-o;gn-N;on-§" },
            { "oignons", "oi-o;gn-N;on-§;s-#" },
            { "oignonet", "oi-o;gn-N;o-o;n-n;et-E" },
            { "oignonets", "oi-o;gn-N;o-o;n-n;e-E;t-#;s-#" },
            { "oignonière", "oi-o;gn-N;o-o;n-n;i-j;è-E;r-R;e-ç" },
            { "oignonières", "oi-o;gn-N;o-o;n-n;i-j;è-E;r-R;e-ç;s-#" },
            { "oignonnade", "oi-o;gn-N;o-o;n-n;n-n;a-a;d-d;e-ç" },
            { "oignonnades", "oi-o;gn-N;o-o;n-n;n-n;a-a;d-d;e-ç;s-#" },
            { "aiguë", "ai-E;g;u-y;ë-ç" },
            { "aiguës", "ai-E;g;u-y;ë-ç;s-#" },
            { "ailhaut", "a;il-j;h-#;au-o;t-#" },
            { "ailhauts", "a;il-j;h-#;au-o;t-#;s-#" },
            { "alastrim", "a-a;l-l;a-a;s-s;t-t;r-R;im" },
            { "alastrims", "a-a;l-l;a-a;s-s;t-t;r-R;im;s-#" },
            { "albedo", "a-a;l-l;b-b;e;d-d;o-o" },
            { "albedos", "a-a;l-l;b-b;e;d-d;o-o;s-#" },
            { "cocktails", "c-k;o-o;ck-k;t-t;ai-E;l;s-#" },
            { "cocktail", "c-k;o-o;ck-k;t-t;ai-E;l" },
            { "aguacate", "a-a;g;u-w;a-a;c-k;a-a;t-t;e" },
            { "aguacates", "a-a;g;u-w;a-a;c-k;a-a;t-t;e;s-#" },
            { "alguazil", "a-a;l-l;g;u-w;a-a;z-z;i-i;l-l" },
            { "alguazils", "a-a;l-l;g;u-w;a-a;z-z;i-i;l-l;s-#" },
            { "braguard", "b-b;r-R;a-a;g;u-w;a-a;r-R;d-#" },
            { "braguarde", "b-b;r-R;a-a;g;u-w;a-a;r-R;d-d;e-ç" },
            { "braguardes", "b-b;r-R;a-a;g;u-w;a-a;r-R;d-d;e-ç;s-#" },
            { "braguards", "b-b;r-R;a-a;g;u-w;a-a;r-R;d-#;s-#" },
            { "guanaco", "g;u-w;a-a;n-n;a-a;c-k;o-o" },
            { "guanacos", "g;u-w;a-a;n-n;a-a;c-k;o-o;s-#" },
            { "guaneux", "g;u-w;a-a;n-n;eu-2;x-#" },
            { "guanine", "g;u-w;a-a;n-n;i-i;n-n;e-ç" },
            { "guanines", "g;u-w;a-a;n-n;i-i;n-n;e-ç;s-#" },
            { "guano", "g;u-w;a-a;n-n;o-o" },
            { "guanos", "g;u-w;a-a;n-n;o-o;s-#" },
            { "guarana", "g;u-w;a-a;r-R;a-a;n-n;a-a" },
            { "guaranas", "g;u-w;a-a;r-R;a-a;n-n;a-a;s-#" },
            { "guarani", "g;u-w;a-a;r-R;a-a;n-n;i-i" },
            { "guaranis", "g;u-w;a-a;r-R;a-a;n-n;i-i;s-#" },
            { "iguane", "i-i;g;u-w;a-a;n-n;e-ç" },
            { "iguanes", "i-i;g;u-w;a-a;n-n;e-ç;s-#" },
            { "iguanodon", "i-i;g;u-w;a-a;n-n;o-o;d-d;on-§" },
            { "iguanodons", "i-i;g;u-w;a-a;n-n;o-o;d-d;on-§;s-#" },
            { "intralingual", "in-5;t-t;r-R;a-a;l-l;in-5;g;u-w;a-a;l-l" },
            { "jaguar", "j-Z;a-a;g;u-w;a-a;r-R" },
            { "jaguars", "j-Z;a-a;g;u-w;a-a;r-R;s-#" },
            { "lingual", "l-l;in-5;g;u-w;a-a;l-l" },
            { "linguale", "l-l;in-5;g;u-w;a-a;l-l;e-ç" },
            { "linguales", "l-l;in-5;g;u-w;a-a;l-l;e-ç;s-#" },
            { "linguatule", "l-l;in-5;g;u-w;a-a;t-t;u-y;l-l;e-ç" },
            { "linguatules", "l-l;in-5;g;u-w;a-a;t-t;u-y;l-l;e-ç;s-#" },
            { "linguaux", "l-l;in-5;g;u-w;au-o;x-#" },
            { "nagualisme", "n-n;a-a;g;u-w;a-a;l-l;i-i;s-s;m-m;e-ç" },
            { "nagualismes", "n-n;a-a;g;u-w;a-a;l-l;i-i;s-s;m-m;e-ç;s-#" },
            { "paraguante", "p-p;a-a;r-R;a-a;g;u-w;an-@;t-t;e-ç" },
            { "paraguantes", "p-p;a-a;r-R;a-a;g;u-w;an-@;t-t;e-ç;s-#" },
            { "perlingual", "p-p;e-E;r-R;l-l;in-5;g;u-w;a-a;l-l" },
            { "perlinguale", "p-p;e-E;r-R;l-l;in-5;g;u-w;a-a;l-l;e-ç" },
            { "perlinguales", "p-p;e-E;r-R;l-l;in-5;g;u-w;a-a;l-l;e-ç;s-#" },
            { "perlinguaux", "p-p;e-E;r-R;l-l;in-5;g;u-w;au-o;x-#" },
            { "sublingual", "s-s;u-y;b-b;l-l;in-5;g;u-w;a-a;l-l" },
            { "sublinguale", "s-s;u-y;b-b;l-l;in-5;g;u-w;a-a;l-l;e-ç" },
            { "sublinguales", "s-s;u-y;b-b;l-l;in-5;g;u-w;a-a;l-l;e-ç;s-#" },
            { "sublinguaux", "s-s;u-y;b-b;l-l;in-5;g;u-w;au-o;x-#" },
            { "alderman", "a-a;l-l;d-d;e-E;r-R;m-m;an" },
            { "aldermans", "a-a;l-l;d-d;e-E;r-R;m-m;an;s-#" },
            { "ale", "a-E;l-l;e-ç" },
            // { "ales", "a-E;l-l;e-ç;s-#" }, au profit de "ales" prononcé 'à la française" mais
            // dont j'ignore également le sens :-) Vive Morphalou!
            { "aleatico", "a-a;l-l;e;a-a;t-t;i-i;c-k;o-o" },
            { "aleaticos", "a-a;l-l;e;a-a;t-t;i-i;c-k;o-o;s-#" },
            { "algumim", "a-a;l-l;g-g;u-y;m-m;im" },
            { "algumims", "a-a;l-l;g-g;u-y;m-m;im;s-#" },
            { "alléluia", "a-a;ll-l;é-e;l-l;u;i-j;a-a" },
            { "alléluias", "a-a;ll-l;é-e;l-l;u;i-j;a-a;s-#" },
            { "almanach", "a-a;l-l;m-m;a-a;n-n;a-a;ch-#" }, // il y a visiblement une grande discussion 
            // sur la prononciation du ch. Nous suivons morphalou et la règle "ch se prononce k et 
            // ne se fait sentir qu'en liaison avec une voyelle : un almanak ancien" 
            // pour plus de détails voir https://cnrtl.fr/definition/almanach
            { "almanachs", "a-a;l-l;m-m;a-a;n-n;a-a;ch-#;s-#" },
            { "alpenstock", "a-a;l-l;p-p;e-E;n;s-s;t-t;o-o;c-k;k-k" },
            { "alpenstocks", "a-a;l-l;p-p;e-E;n;s-s;t-t;o-o;c-k;k-k;s-#" },
            { "alsacien", "a-a;l-l;s-z;a-a;c-s;i-j;en-5" },
            { "alsaciens", "a-a;l-l;s-z;a-a;c-s;i-j;en-5;s-#" },
            { "alsacienne", "a-a;l-l;s-z;a-a;c-s;i-j;e-E;n-n;n-n;e-ç" },
            { "alsaciennes", "a-a;l-l;s-z;a-a;c-s;i-j;e-E;n-n;n-n;e-ç;s-#" },
            { "alsace", "a-a;l-l;s-z;a-a;c-s;e-ç" },
            { "amabile", "a-a;m-m;a-a;b-b;i-i;l-l;e" },
            { "ambigu", "am-@;b-b;i-i;g;u-y" },
            { "ambiguë", "am-@;b-b;i-i;g;u-y;ë-ç" },
            { "ambiguës", "am-@;b-b;i-i;g;u-y;ë-ç;s-#" },
            { "bégu", "b-b;é-e;g;u-y" },
            { "béguë", "b-b;é-e;g;u-y;ë-ç" },
            { "béguës", "b-b;é-e;g;u-y;ë-ç;s-#" },
            { "besaiguë", "b-b;e-°;s-z;ai-E;g;u-y;ë-ç" },
            { "besaiguës", "b-b;e-°;s-z;ai-E;g;u-y;ë-ç;s-#" },
            { "bisaguë", "b-b;i-i;s-z;a-a;g;u-y;ë-ç" },
            { "bisaguës", "b-b;i-i;s-z;a-a;g;u-y;ë-ç;s-#" },
            { "bisaiguë", "b-b;i-i;s-z;ai-E;g;u-y;ë-ç" },
            { "bisaiguës", "b-b;i-i;s-z;ai-E;g;u-y;ë-ç;s-#" },
            { "ciguë", "c-s;i-i;g;u-y;ë-ç" },
            { "ciguës", "c-s;i-i;g;u-y;ë-ç;s-#" },
            { "contigu", "c-k;on-§;t-t;i-i;g;u-y" },
            { "contiguë", "c-k;on-§;t-t;i-i;g;u-y;ë-ç" },
            { "contiguës", "c-k;on-§;t-t;i-i;g;u-y;ë-ç;s-#" },
            { "exigu", "e-E;x-X;i-i;g;u-y" },
            { "exiguë", "e-E;x-X;i-i;g;u-y;ë-ç" },
            { "exiguës", "e-E;x-X;i-i;g;u-y;ë-ç;s-#" },
            { "gulden", "g-g;u-y;l-l;d-d;e-E;n" },
            { "guldens", "g-g;u-y;l-l;d-d;e-E;n;s-#" },
            { "subaiguë", "s-s;u-y;b-b;ai-E;g;u-y;ë-ç" },
            { "subaiguës", "s-s;u-y;b-b;ai-E;g;u-y;ë-ç;s-#" },
            { "suraiguë", "s-s;u-y;r-R;ai-E;g;u-y;ë-ç" },
            { "suraiguës", "s-s;u-y;r-R;ai-E;g;u-y;ë-ç;s-#" },
            { "ambiguïflore", "am-@;b-b;i-i;g;u-y;ï-i;f-f;l-l;o-o;r-R;e-ç" },
            { "ambiguïsme", "am-@;b-b;i-i;g;u-y;ï-i;s-s;m-m;e-ç" },
            { "ambiguïté", "am-@;b-b;i-i;g;u-y;ï-i;t-t;é-e" },
            { "ambiguïtés", "am-@;b-b;i-i;g;u-y;ï-i;t-t;é-e;s-#" },
            { "ambisexe", "am-@;b-b;i-i;s;e-E;x-x;e-ç" },
            { "ambosexuel", "am-@;b-b;o-o;s;e-E;x-x;u-y;e-E;l-l" },
            { "amenokal", "a-a;m-m;e;n-n;o-o;k-k;a-a;l-l" },
            { "amenokals", "a-a;m-m;e;n-n;o-o;k-k;a-a;l-l;s-#" },
            { "amict", "a-a;m-m;i-i;ct-#" }, // Le caractère muet de -ct final est confirmé par Lab. 1881 (cnrtl)
            { "amicts", "a-a;m-m;i-i;ct-#;s-#" },
            { "amin", "a-a;m-m;in" },
            { "amins", "a-a;m-m;in;s-#" },
            { "amœbocytes", "a-a;m-m;œ-e;b-b;o-o;c-s;y-i;t-t;e-ç;s-#" },
            { "amschir", "am-@;sch-S;i-i;r-R" },
            { "amschirs", "am-@;sch-S;i-i;r-R;s-#" },
            { "champlevé", "ch-S;am-@;p-#;l-l;e-°;v-v;é-e" },
            { "champlevée", "ch-S;am-@;p-#;l-l;e-°;v-v;é-e;e-#" },
            { "champlevées", "ch-S;am-@;p-#;l-l;e-°;v-v;é-e;e-#;s-#" },
            { "champlevés", "ch-S;am-@;p-#;l-l;e-°;v-v;é-e;s-#" },
            { "clamp", "c-k;l-l;am-@;p-#" },
            { "hamburger", "h-#;am-@;b-b;u;r-R;g;e-2;r-R" },
            { "hamburgers", "h-#;am-@;b-b;u;r-R;g;e-2;r-R;s-#" },
            { "landamman", "l-l;an-@;d-d;a-a;mm-m;an" },
            { "landammans", "l-l;an-@;d-d;a-a;mm-m;an;s-#" },
            { "shampooina", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a" },
            { "shampooinai", "sh-S;am-@;p-p;oo-u;i-i;n-n;ai-e" },
            { "shampooinaient", "sh-S;am-@;p-p;oo-u;i-i;n-n;ai-E;ent-#" },
            { "shampooinais", "sh-S;am-@;p-p;oo-u;i-i;n-n;ai-E;s-#" },
            { "shampooinait", "sh-S;am-@;p-p;oo-u;i-i;n-n;ai-E;t-#" },
            { "shampooinâmes", "sh-S;am-@;p-p;oo-u;i-i;n-n;â-a;m-m;e-ç;s-#" },
            { "shampooinant", "sh-S;am-@;p-p;oo-u;i-i;n-n;an-@;t-#" },
            { "shampooinas", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;s-#" },
            { "shampooinasse", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;ss-s;e-ç" },
            { "shampooinassent", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;ss-s;e-ç;nt-#" },
            { "shampooinasses", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;ss-s;e-ç;s-#" },
            { "shampooinassiez", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;ss-s;i-j;ez-e" },
            { "shampooinassions", "sh-S;am-@;p-p;oo-u;i-i;n-n;a-a;ss-s;i-j;on-§;s-#" },
            { "shampooinât", "sh-S;am-@;p-p;oo-u;i-i;n-n;â-a;t-#" },
            { "shampooinâtes", "sh-S;am-@;p-p;oo-u;i-i;n-n;â-a;t-t;e-ç;s-#" },
            { "shampooine", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-ç" },
            { "shampooiné", "sh-S;am-@;p-p;oo-u;i-i;n-n;é-e" },
            { "shampooinée", "sh-S;am-@;p-p;oo-u;i-i;n-n;é-e;e-#" },
            { "shampooinées", "sh-S;am-@;p-p;oo-u;i-i;n-n;é-e;e-#;s-#" },
            { "shampooinent", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-ç;nt-#" },
            { "shampooiner", "sh-S;am-@;p-p;oo-u;i-i;n-n;er-e" },
            { "shampooinera", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;a-a" },
            { "shampooinerai", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;ai-e" },
            { "shampooineraient", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;ai-E;ent-#" },
            { "shampooinerais", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;ai-E;s-#" },
            { "shampooinerait", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;ai-E;t-#" },
            { "shampooineras", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;a-a;s-#" },
            { "shampooinèrent", "sh-S;am-@;p-p;oo-u;i-i;n-n;è-E;r-R;e-ç;nt-#" },
            { "shampooinerez", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;ez-e" },
            { "shampooineriez", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;i-j;ez-e" },
            { "shampooinerions", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;i-j;on-§;s-#" },
            { "shampooinerons", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;on-§;s-#" },
            { "shampooineront", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-°;r-R;on-§;t-#" },
            { "shampooines", "sh-S;am-@;p-p;oo-u;i-i;n-n;e-ç;s-#" },
            { "shampooinés", "sh-S;am-@;p-p;oo-u;i-i;n-n;é-e;s-#" },
            { "shampooineur", "sh-S;am-@;p-p;oo-u;i-i;n-n;eu-2;r-R" },
            { "shampooineurs", "sh-S;am-@;p-p;oo-u;i-i;n-n;eu-2;r-R;s-#" },
            { "shampooineuse", "sh-S;am-@;p-p;oo-u;i-i;n-n;eu-2;s-z;e-ç" },
            { "shampooineuses", "sh-S;am-@;p-p;oo-u;i-i;n-n;eu-2;s-z;e-ç;s-#" },
            { "shampooinez", "sh-S;am-@;p-p;oo-u;i-i;n-n;ez-e" },
            { "shampooing", "sh-S;am-@;p-p;oo-u;in-5;g-#" },
            { "shampooings", "sh-S;am-@;p-p;oo-u;in-5;g-#;s-#" },
            { "shampooiniez", "sh-S;am-@;p-p;oo-u;i-i;n-n;i-j;ez-e" },
            { "shampooinions", "sh-S;am-@;p-p;oo-u;i-i;n-n;i-j;on-§;s-#" },
            { "shampooinons", "sh-S;am-@;p-p;oo-u;i-i;n-n;on-§;s-#" },
            { "tramps", "t-t;r-R;am-@;p;s-#" },
            { "vamps", "v-v;am-@;p;s-#" },

        }; 
    }
}
