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

        public Word(TheText inT, int inFirst, int inLast)
            : base(inT, inFirst, inLast)
        {
            wordToLower = base.ToString().ToLower(BaseConfig.cultF);
        }

        public Word(Word w)
            :base(w)
        {
            wordToLower = w.wordToLower;
        }

        public string GetWord() => wordToLower;

        public char GetChar(int pos) => wordToLower[pos - First];
        // Retourne le caractère minuscule à la position pos dans le texte sous-jacent


        public override void PutColor(Config conf)
        {
            base.SetCharFormat(conf.sylConf.NextCF()); ;
        }
    }
}
