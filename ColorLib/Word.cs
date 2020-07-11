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
    public class Word : TextEl
    {
        private string wordToLower;

        /// <summary>
        /// Cretaes a Word as a reference to the given <see cref="TheText"/>.
        /// </summary>
        /// <remarks>
        /// <c>inLast</c> must be equal or larger than <c>inFirst</c>.
        /// </remarks>
        /// <param name="inT">The <see cref="TheText"/> which contains the <c>Word</c>.</param>
        /// <param name="inFirst">The zero based position of the first character of the <c>Word</c>
        /// in <c>inT</c>. </param>
        /// <param name="inLast">The zero based position of the last character of the <c>Word</c>
        /// in <c>inT</c>. </param></param>
        public Word(TheText inT, int inFirst, int inLast)
            : base(inT, inFirst, inLast)
        {
            wordToLower = base.ToLowerString();
        }

        /// <summary>
        /// Creates a <c>Word</c> as a copy of an other one.
        /// </summary>
        /// <param name="w">The <c>Word</c> that is copied. Cannot be <c>null</c>.</param>
        public Word(Word w)
            :base(w)
        {
            wordToLower = w.wordToLower;
        }

        /// <summary>
        /// Returns the word in small caps.
        /// </summary>
        /// <returns>Word in small caps</returns>
        public string GetWord() => wordToLower;

        /// <summary>
        /// Gives the <c>char</c> at the given position in the word.
        /// </summary>
        /// <param name="pos">Zero based position of the wanted <c>char</c>.</param>
        /// <returns>word[pos]</returns>
        public char GetChar(int pos) => wordToLower[pos - First];

        /// <summary>
        /// Makes surte that the <c>Word</c> will be formatted according to the next
        /// <see cref="CHarFormatting"/> delivered by the <c>sylConf</c> configuration
        /// of <c>config</c>.
        /// </summary>
        /// <param name="conf"></param>
        public override void PutColor(Config conf)
        {
            base.SetCharFormat(conf.sylConf.NextCF()); ;
        }
    }
}
