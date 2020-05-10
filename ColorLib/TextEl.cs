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
using System.Diagnostics;

namespace ColorLib
{
    public class TextEl
    {
        public TheText T { get; private set; }
        public int First { get; protected set; } // start index of the word in T
        public int Last { get; protected set; }
        private const string consonnes = "bcdfghjklmnpqrstvwxzç";
        private const string voyelles = "aeiouyœéàèùäëïöüâêîôûœ";

        public static bool EstConsonne(char c)
        {
            return consonnes.IndexOf(c) > -1;
        }

        public static bool EstVoyelle(char c)
        {
            return voyelles.IndexOf(c) > -1;
        }

        public TextEl(TheText inT, int inFirst, int inLast)
        {
            Debug.Assert(inLast >= inFirst);
            T = inT;
            First = inFirst;
            Last = inLast;
        }

        protected TextEl(TextEl te)
        {
            T = te.T;
            First = te.First;
            Last = te.Last;
        }

        public override string ToString()
        {
            return T.S.Substring(First, (Last - First) + 1);
        }

        protected void SetCharFormat(CharFormatting cf)
        {
            FormattesTextEl cte = new FormattesTextEl(this, cf);
            T.Formats.Add(cte);
        }

        public virtual void PutColor()
            // Colorize les caractères 
        {
            SetCharFormat(new CharFormatting());
        }

        // Returns a dump of all members of the object
        public virtual string AllStringInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.ToString());
            sb.Append(" --> ");
            sb.Append(String.Format("First: {0}, Last: {1}", First, Last));
            return sb.ToString();
        }

    }
}
