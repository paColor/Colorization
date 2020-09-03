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

using ColorLib.Morphalou;
using System;
using System.Collections.Generic;
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
                    while (sons[j] != '-')
                    {
                        j++;
                    }
                    int len = j - i; // j doit être > i, donc len > 0;
                    string lettres = sons.Substring(i, len); 
                    string son = sons.Substring(j + 1, 1); // si on est à la fin du string, ça
                    // va déclencher une exception et c'est très bien ainsi.

                    PhonInW piw = new PhonInW(pw, l, l + len - 1, NotationsPhon.Son2phon(son), "exception");

                    l += len;

                    while (sons[j] != ';')
                    {
                        j++;
                    }
                    i = j + 1;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Le dictionnaire contien d'un côté le mot comme clé, de l'autre la représentation sous
        /// forme de string de sa représentation en phonèmes.
        /// Le deuxième string (le contenu) a le format {lettre, {lettre} "-" son ";"}. L'ensemble
        /// des lettres doit être équivalent au mot, cad à la clé. 
        /// son est la représentation en code colorization du son, où un symbole rerpésente un son.
        /// </summary>
        private static Dictionary<string, string> except = new Dictionary<string, string>()
        {
            { "aber", "a-a;b-b;e-E;r-R" },
            { "abers", "a-a;b-b;e-E;r-R;s-#" },
        }; 
    }
}
