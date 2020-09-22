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

using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    public abstract class AutomElement
        // Element of the LireCouleur Automata
        // The class provides the basic parsing capabilities
    {        
        protected string automS { get; } // the complete automata
        protected int start; // the first letter of the eléement is s[start], typically a delimiter
        protected int end { get; set; } // the last letter of the element is s[end], typically a delimiter.

        // Construct an AutoElement by passing the automata definition as a string and the index of the first letter of the element
        // The first letter is the delimiter for the element. When the element is constructed, end corresponds to the last letter
        // of the element, i.e. the closing delimiter.
        public AutomElement(string automTxt, int firstLetter)
        {
            automS = automTxt;
            start = firstLetter; // It may be that we'll never use the start property... To be checked!!
        }

        // GetNextChar discards all non relevant characters like spaces, newlines and comments.
        // Start at s[pos]
        // returns the next position where a relevant character is encontered. May be pos...
        // throws an exception if end of string is reached
        public int GetNextChar(int pos)
        {
            while (pos < automS.Length && 
                (automS[pos] == ' ' 
                || automS[pos] == '\t' 
                || automS[pos] == '\r' 
                || automS[pos] == '\n' 
                || automS[pos] == '/'))
            {
                if (automS[pos] == '/') 
                {
                    if ((pos < automS.Length - 1) && (automS[pos + 1] == '/'))
                    {
                        // it is a double slash, hence the beginning of a comment
                        pos = automS.IndexOf('\n', pos + 1); // jump to end of line - in the worst case, the second slash is the last char in the string
                        if (pos == -1)
                        {
                            pos = automS.Length;
                        } else
                        {
                            pos++;
                        }
                    } else
                    {
                        // it is a self standing slash
                        return pos;
                    }
                } else
                {
                    pos++;
                }
            } // while
            if (pos >= automS.Length)
            {
                throw new ArgumentException(String.Format(
                    "Unexpected end of string in AutomElement.GetNextChar"));
            }
            return pos;
        }

        public override string ToString()
        {
            return automS.Substring(start, (end - start) + 1); 
        }

        /// <summary>
        /// Génère une extrait de 100 caractères du texte de l'automate autour de la position 
        /// <paramref name="pos"/>. 5 ! sont insérés à la position pour bien marquer l'endroit.
        /// </summary>
        /// <param name="pos">La position de l'erreur dans l'automate.</param>
        /// <param name="msg">Un message à afficher avant l'extrait de l'automate. </param>
        /// <returns>Un extrait de l'automate autour de <paramref name="pos"/>, précédé de
        /// <paramref name="msg"/> et "[...]" si <paramref name="msg"/> est non <c>null</c>.
        /// </returns>
        protected string ErrorExcerpt(int pos, string msg = null)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(msg))
            {
                sb.Append(msg);
                sb.Append(": [...]");
            }
            int startPos = Math.Max(0, pos - 50);
            int stopPos = Math.Min(automS.Length - 1, pos + 50);
            sb.Append(automS.Substring(startPos, pos + 1 - startPos));
            sb.Append("<--!!!!!!!!!!! ");
            if (pos < automS.Length - 1)
            {
                sb.Append(automS.Substring(pos + 1, stopPos - pos));
            }
            return sb.ToString();
        }

    } // class AutomElement
}
