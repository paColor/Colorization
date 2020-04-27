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
        protected string s { get; }
        protected int start; // the first letter of the eléement is s[start], typically a delimiter
        protected int end { get; set; } // the last letter of the element is s[end], typically a delimiter.

        // Construct an AutoElement by passing the automata definition as a string and the index of the first letter of the element
        // The first letter is the delimiter for the element. When the element is constructed, end corresponds to the last letter
        // of the element, i.e. the closing delimiter.
        public AutomElement(string automTxt, int firstLetter)
        {
            s = automTxt;
            start = firstLetter; // It may be that we'll never use the start property... To be checked!!
        }

        // GetNextChar discards all non relevant characters like spaces, newlines and comments.
        // Start at s[pos]
        // returns the next position where a relevant character is encontered. May be pos...
        // throws an exception if end of string is reached
        public int GetNextChar(int pos)
        {
            while ((pos < s.Length) && ((s[pos] == ' ' || s[pos] == '\t' || s[pos] == '\r' || s[pos] == '\n' || s[pos] == '/')))
            {
                if (s[pos] == '/') 
                {
                    if ((pos < s.Length - 1) && (s[pos + 1] == '/'))
                    {
                        // it is a double slash, hence the beginning of a comment
                        pos = s.IndexOf('\n', pos + 1); // jump to end of line - in the worst case, the second slash is the last char in the string
                        if (pos == -1)
                        {
                            pos = s.Length;
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
            if (pos >= s.Length)
            {
                throw new ArgumentException(String.Format(
                    "Unexpected end of string in AutomElement.GetNextChar"));
            }
            return pos;
        }

        public override string ToString()
        {
            return s.Substring(start, (end - start) + 1); 
        }
    } // class AutomElement
}
